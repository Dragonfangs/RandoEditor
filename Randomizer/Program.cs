﻿using Common.Key;
using Common.SaveData;
using Common.Utils;
using mzmr_common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verifier;
using Verifier.Key;

namespace Randomizer
{
	class Program
	{
		static string _logMessage = string.Empty;
		static Dictionary<string, Guid> resultItemMap = new Dictionary<string, Guid>();
		static Inventory startingInventory = new Inventory();
		static Stopwatch _Timer = new Stopwatch();

		static void Main(string[] args)
		{
			Randomize();

			var fileText = "";

			var inventoryText = "";
			if (startingInventory.myKeys.Any())
			{
				inventoryText = startingInventory.myKeys.Select(key => key.Name).Aggregate((i, j) => i + "," + j) + $";{Environment.NewLine}";
				Console.WriteLine(inventoryText);

				fileText = inventoryText;
			}

			var map = resultItemMap.Select(pair => $"{pair.Key}:{KeyManager.GetKey(pair.Value)?.Name ?? "None"}").Aggregate((i, j) => i + $",{Environment.NewLine}" + j);
			Console.WriteLine(map);

			fileText += map;

			File.WriteAllText(Environment.CurrentDirectory + "//itemMap.txt", fileText);
			Console.WriteLine($"{Environment.NewLine}Time taken: {_Timer.Elapsed}");
			Console.ReadKey();
		}

		public static void Randomize()
		{
			var fileNames = new List<string>(Directory.GetFiles(Environment.CurrentDirectory));

			var logicFile = ChooseLogic(fileNames.Where(x => Path.GetExtension(x) == ".lgc").ToList());
			if (logicFile == null)
			{
				// Log issue
				_logMessage += $"No logic file chosen{Environment.NewLine}";
			}

			var data = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(logicFile));

			Console.WriteLine();

			KeyManager.Initialize(data);

			var traverser = new NodeTraverser();

			var settingsFile = ChooseSettings(fileNames.Where(x => Path.GetExtension(x) == ".txt" || Path.GetExtension(x) == ".log").ToList());
			if (settingsFile == null)
			{
				// Log issue
				_logMessage += $"No settings chosen{Environment.NewLine}";
			}

			if (!TryParseSettingsFile(data, startingInventory, settingsFile))
			{
				// Log issue
				_logMessage += "Failed to parse settings";
			}
			
			var gameCompletion = ChooseGameCompletion();

			var random = new Random();
			var seed = random.Next();
			//var seed = 669144804;

			var count = 0;
			_Timer.Start();
			while(true)
			{
				var pool = new ItemPool();
				pool.CreatePool(data);

				random = new Random(seed);

				var randomMap = StaticData.Locations.ToDictionary(location => location, location => pool.Pull(random));

				var morphKey = Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee");
				var morphItem = randomMap.FirstOrDefault(x => x.Value.Equals(morphKey));

				if (string.IsNullOrWhiteSpace(morphItem.Key))
				{
					seed = random.Next();
					continue;
				}
				
				var morphLocation = randomMap.FirstOrDefault(x => x.Key.Equals("BrinstarMorph"));

				randomMap[morphItem.Key] = morphLocation.Value;
				randomMap[morphLocation.Key] = morphKey;

				var beatableResult = false;
				if (gameCompletion == 1)
				{
					beatableResult = traverser.VerifyBeatable(data, randomMap, new Inventory(startingInventory));
				}
				else if (gameCompletion == 2)
				{
					beatableResult = traverser.VerifyFullCompletable(data, randomMap, new Inventory(startingInventory));
				}

				if (beatableResult)
				{
					Console.WriteLine($"Randomization complete after {count} attempts - Successful seed: {seed}");

					_Timer.Stop();

					resultItemMap = randomMap;
					return;
				}
				else
				{
					if (count % 50 == 0)
					{
						Console.WriteLine($"Attempts {count}");
					}
					count++;
				}

				seed = random.Next();
			}
		}

		public static BaseKey TranslateKey(SaveData data, string keyName)
		{
			var allKeys = data.BasicKeys.Values.SelectMany(keylist => keylist.Values);
			return allKeys.FirstOrDefault(key => key.Name.Replace(" ", "").Equals(keyName, StringComparison.InvariantCultureIgnoreCase));
		}

		public static int ChooseGameCompletion()
		{
			Console.WriteLine($"Game Completion?");
			Console.WriteLine($"1. Beatable (default)");
			Console.WriteLine($"2. 100%");

			var response = Console.ReadLine();

			if (int.TryParse(response, out var result))
			{
				if (result > 0 && result < 3)
				{
					return result;
				}
			}

			return 1;
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

		public static string ChooseSettings(List<string> logicFiles)
		{
			Console.WriteLine($"Choose a logic file or enter setting string:");
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

			return response;
		}

		public static bool TryParseSettingsFile(SaveData data, Inventory inventory, string settingsLine)
		{
			if (!File.Exists(settingsLine))
				return false;

			var text = File.ReadAllText(settingsLine);

			if (text.StartsWith("Seed:"))
			{
				return ParseItemLogInventory(data, inventory, text);
			}

			text = text.Replace(Environment.NewLine, "").Replace(" ", "");

			var bigSplit = text.Split(';');

			if (!bigSplit.Any())
			{
				return false;
			}

			if (bigSplit.Count() > 2)
			{
				// Incorrect format (too many semicolons)
				return false;
			}

			if(bigSplit[0].Any(c => c == ':'))
			{
				// Incorrect format (no colons allowed)
				return false;
			}

			var inventoryItems = bigSplit[0].Split(',');

			var itemData = inventoryItems.ToDictionary(item => item, item => TranslateKey(data, item));

			var incorrectItems = itemData.Where(item => item.Value == null);
			if (incorrectItems.Any())
			{
				_logMessage += incorrectItems
					.Select(item => $"ParseInventory - Key named {item.Key} not found{Environment.NewLine}")
					.Aggregate((i, j) => i + j) + Environment.NewLine;

				return false;
			}

			_logMessage += $"Starting Inventory: {itemData.Keys.Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";

			inventory.myKeys.AddRange(itemData.Values.Select(item => item).ToList());
			return true;
		}

		public static bool ParseItemLogInventory(SaveData data, Inventory inventory, String logText)
		{
			string[] lines = logText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

			var settingsLine = lines.FirstOrDefault(line => line.StartsWith("Settings:"));
			if (!String.IsNullOrEmpty(settingsLine))
			{
				if (TryParseSettings(data, inventory, settingsLine))
				{
					return true;
				}

				if(TryParseOldSettings(data, inventory, settingsLine))
				{
					return true;
				}
			}

			return false;
		}

		public static bool TryParseSettings(SaveData data, Inventory inventory, string settingsLine)
		{
			try
			{
				var settings = new Settings(settingsLine.Substring(settingsLine.LastIndexOf(' ') + 1));
				if (settings.IceNotRequired)
				{
					inventory.myKeys.Add(TranslateKey(data, "IceBeamNotRequired"));
				}

				if (settings.PlasmaNotRequired)
				{
					inventory.myKeys.Add(TranslateKey(data, "PlasmaBeamNotRequired"));
				}

				if (settings.InfiniteBombJump)
				{
					inventory.myKeys.Add(TranslateKey(data, "CanInfiniteBombJump"));
				}

				if (settings.WallJumping)
				{
					inventory.myKeys.Add(TranslateKey(data, "CanWallJump"));
				}

				if (settings.ObtainUnkItems)
				{
					inventory.myKeys.Add(TranslateKey(data, "ObtainUnknownItems"));
				}

				if (settings.ChozoStatueHints)
				{
					inventory.myKeys.Add(TranslateKey(data, "ChozoStatueHints"));
				}

				if (settings.RandoEnemies)
				{
					inventory.myKeys.Add(TranslateKey(data, "RandomizeEnemies"));
				}

				return true;
			}
			catch (Exception)
			{

			}

			return false;
		}

		public static bool TryParseOldSettings(SaveData data, Inventory inventory, string settingsLine)
		{
			try
			{
				var settings = new Settings_Old(settingsLine.Substring(settingsLine.LastIndexOf(' ') + 1));
				if (settings.iceNotRequired)
				{
					inventory.myKeys.Add(TranslateKey(data, "IceBeamNotRequired"));
				}

				if (settings.plasmaNotRequired)
				{
					inventory.myKeys.Add(TranslateKey(data, "PlasmaBeamNotRequired"));
				}

				if (settings.infiniteBombJump)
				{
					inventory.myKeys.Add(TranslateKey(data, "CanInfiniteBombJump"));
				}

				if (settings.wallJumping)
				{
					inventory.myKeys.Add(TranslateKey(data, "CanWallJump"));
				}

				if (settings.obtainUnkItems)
				{
					inventory.myKeys.Add(TranslateKey(data, "ObtainUnknownItems"));
				}

				return true;
			}
			catch (Exception)
			{

			}

			return false;
		}
	}
}
