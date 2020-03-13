
using Common.Key;
using Common.Node;
using Common.SaveData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Verifier;
using Verifier.Key;

namespace VerifierClient
{
	class Program
	{
		static string _logMessage = string.Empty;

		static void Main(string[] args)
		{
			DoTest();
			Console.WriteLine(_logMessage);
			File.WriteAllText(Environment.CurrentDirectory + "//verifier.log", _logMessage);
			Console.ReadKey();
		}

		public static void DoTest()
		{
			var fileNames = new List<string>(Directory.GetFiles(Environment.CurrentDirectory));

			var logicFile = ChooseLogic(fileNames.Where(x => Path.GetExtension(x) == ".lgc").ToList());
			if (logicFile == null)
			{
				// Log issue
				_logMessage += $"No logic file chosen{Environment.NewLine}";
				return;
			}

			var itemFile = ChooseItems(fileNames.Where(x => Path.GetExtension(x) == ".txt").ToList());
			if (itemFile == null)
			{
				// Log issue
				_logMessage += $"No item file chosen{Environment.NewLine}";
				return;
			}

			var data = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(logicFile));

			var inventoryKeys = ParseInventory(data, itemFile);

			if(inventoryKeys == null)
			{
				// Error logged in parser
				return;
			}

			var randomMap = ParseItemMap(data, itemFile);

			if (randomMap == null)
			{
				// Error logged in parser
				return;
			}

			Console.WriteLine();

			var traverser = new NodeTraverser();

			var inventory = new Inventory();
			inventory.myKeys = inventoryKeys;

			_logMessage += Environment.NewLine;
			var fullCompleteResult = traverser.VerifyFullCompletable(data, randomMap, new Inventory(inventory));
			_logMessage += $"100%: {fullCompleteResult}{Environment.NewLine}{traverser.GetWaveLog()}{Environment.NewLine}{Environment.NewLine}";

			var beatableResult = traverser.VerifyBeatable(data, randomMap, new Inventory(inventory));
			_logMessage += $"Beatable: {beatableResult}{Environment.NewLine}{traverser.GetWaveLog()}";
		}

		public static string ChooseLogic(List<string> logicFiles)
		{
			if (logicFiles.Count < 2)
				return logicFiles.FirstOrDefault();

			Console.WriteLine($"Choose a logic file:");
			Console.WriteLine(logicFiles
				.Select((file, i) => $"{i + 1}: {Path.GetFileName(file)}")
				.Aggregate((i, j) => i + Environment.NewLine + j));
			
			var response = Console.ReadLine();

			if (int.TryParse(response, out var result))
			{
				if (result < 1 || result > logicFiles.Count() + 1)
				{
					_logMessage += $"{result} is not a valid index{Environment.NewLine}";
					return null;
				}

				return logicFiles[result - 1];
			}
			
			_logMessage += $"{response} is not a number{Environment.NewLine}";
			return null;
		}

		public static string ChooseItems(List<string> itemFiles)
		{
			if (itemFiles.Count < 2)
				return itemFiles.FirstOrDefault();

			Console.WriteLine($"Choose an item file:");
			Console.WriteLine(itemFiles
				.Select((file, i) => $"{i + 1}: {Path.GetFileName(file)}")
				.Aggregate((i, j) => i + Environment.NewLine + j));

			var response = Console.ReadLine();

			if (int.TryParse(response, out var result))
			{
				if (result < 1 || result > itemFiles.Count() + 1)
				{
					_logMessage += $"{result} is not a valid index{Environment.NewLine}";
					return null;
				}

				return itemFiles[result - 1];
			}

			_logMessage += $"{response} is not a number{Environment.NewLine}";
			return null;
		}

		public static List<BaseKey> ParseInventory(SaveData data, string itemFileName)
		{
			var text = File.ReadAllText(itemFileName);

			text = text.Replace(Environment.NewLine, "").Replace(" ", "");

			var bigSplit = text.Split(';');

			if (bigSplit.Count() > 2)
			{
				// Incorrect format (too many semicolons)
				return null;
			}

			if (bigSplit.Count() < 2)
			{
				_logMessage += $"No Inventory set{Environment.NewLine}";
				return new List<BaseKey>();
			}

			var inventory = bigSplit[0].Split(',');

			var itemData = inventory.ToDictionary(item => item, item => TranslateKey(data, item));

			var incorrectItems = itemData.Where(item => item.Value == null);
			if (incorrectItems.Any())
			{
				_logMessage += incorrectItems
					.Select(item => $"ParseInventory - Key named {item.Key} not found{Environment.NewLine}")
					.Aggregate((i, j) => i + j) + Environment.NewLine;

				return null;
			}

			_logMessage += $"Starting Inventory: {itemData.Keys.Aggregate((i,j) => i + ", " + j)}{Environment.NewLine}";
			return itemData.Values.Select(item => item).ToList();
		}

		public static Dictionary<string, Guid> ParseItemMap(SaveData data, string itemFileName)
		{
			var text = File.ReadAllText(itemFileName);

			text = text.Replace(Environment.NewLine, "").Replace(" ", "");

			var bigSplit = text.Split(';');

			if (bigSplit.Count() > 2)
			{
				_logMessage += $"Item file in incorrect format (at most one semicolon expected){Environment.NewLine}";
				return null;
			}

			var itemMap = bigSplit.Count() > 1 ? bigSplit[1] : bigSplit[0];

			var itemPairs = itemMap.Split(',').ToList();

			var itemSplits = itemPairs.Select(pair => pair.Split(':').ToList());

			var incorrectPairs = itemSplits.Where(splits => splits.Count() != 2);
			if (incorrectPairs.Any())
			{
				_logMessage += incorrectPairs.Select(pair => pair.Aggregate((i, j) => i + ',' + j) + $" - Every item pair must be exactly two items{Environment.NewLine}")
					.Aggregate((i,j) => i + j);

				return null;
			}

			var itemData = itemSplits.ToDictionary(splits => splits[0], splits => TranslateKey(data, splits[1]));

			var incorrectItems = itemData.Where(item => item.Value == null);
			if(incorrectItems.Any())
			{
				_logMessage += incorrectItems
					.Select(item => $"ParseItemMap - Key named {item.Key} not found in logic")
					.Aggregate((i, j) => i + Environment.NewLine + j);

				return null;
			}

			var nodeCollection = new NodeCollection();
			nodeCollection.InitializeNodes(data);

			var randomLocations = nodeCollection.myNodes.Where(node => node is RandomKeyNode).Select(node => (node as RandomKeyNode).myRandomKeyIdentifier);

			var incorrectLocations = itemData.Where(item => !randomLocations.Contains(item.Key));
			if (incorrectLocations.Any())
			{
				_logMessage += incorrectLocations
					.Select(item => $"ParseItemMap - Location named {item.Key} not found in logic")
					.Aggregate((i, j) => i + Environment.NewLine + j);

				return null;
			}

			return itemData.ToDictionary(item => item.Key, item => item.Value.Id);
		}

		public static BaseKey TranslateKey(SaveData data, string keyName)
		{
			var allKeys = data.BasicKeys.Values.SelectMany(keylist => keylist.Values);
			return allKeys.FirstOrDefault(key => key.Name.Replace(" ", "").Equals(keyName, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
