using Common.Key;
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
				inventoryText = startingInventory.myKeys.Select(key => key.Name).Aggregate((i, j) => i + "," + j) + ";";
				Console.WriteLine($"{Environment.NewLine}{inventoryText}");

				fileText = inventoryText;
			}

			var map = resultItemMap.Select(pair => $"{pair.Key}:{KeyManager.GetKey(pair.Value)?.Name ?? "None"}").Aggregate((i, j) => i + $",{Environment.NewLine}" + j);
			Console.WriteLine($"{Environment.NewLine}{map}");

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

			var randomizationType = ChooseRandomization();
			var gameCompletion = ChooseGameCompletion();

			var random = new Random();
			var seed = random.Next();
			//var seed = 669144804;

			var count = 0;
			_Timer.Start();
			while(true)
			{
				if (randomizationType == 1)
				{
					var placer = new ItemPlacer();
					var options = new FillOptions();
					options.GameCompletion = gameCompletion;
					options.noEarlyPbs = true;
					var result = false;

					ItemPool pool = null;
					while (true)
					{
						pool = new ItemPool();
						
						pool.CreatePool(data);
						//pool.RemoveRandomItems(85, random);

						var testInventory = new Inventory(startingInventory);

						testInventory.myKeys.AddRange(pool.AvailableItems().Where(key => key != Guid.Empty && (!options.noEarlyPbs || key != StaticKeys.PowerBombs)).Select(id => KeyManager.GetKey(id)));

						if (traverser.VerifyBeatable(data, new Dictionary<string, Guid>(), testInventory))
						{
							break;
						}
					}

					var randomMap = placer.FillLocations(data, options, pool, startingInventory, random);

					if (gameCompletion == 1)
						result = traverser.VerifyBeatable(data, randomMap, new Inventory(startingInventory));
					else if (gameCompletion == 2)
						result = traverser.VerifyFullCompletable(data, randomMap, new Inventory(startingInventory));

					if (result)
					{
						Console.WriteLine($"{Environment.NewLine}Randomization complete after {count} attempts - Successful seed: {seed}");

						Console.WriteLine(traverser.GetWaveLog());

						_Timer.Stop();

						resultItemMap = randomMap;
						return;
					}

					Console.WriteLine($"Attempts {count}");
				}
				else if (randomizationType == 2)
				{
					var pool = new ItemPool();
					pool.CreatePool(data);

					random = new Random(seed);

					var randomMap = StaticData.Locations.ToDictionary(location => location, location => pool.Pull(random));

					var morphItem = randomMap.FirstOrDefault(x => x.Value.Equals(StaticKeys.Morph));

					if (string.IsNullOrWhiteSpace(morphItem.Key))
					{
						seed = random.Next();
						continue;
					}

					var morphLocation = randomMap.FirstOrDefault(x => x.Key.Equals("BrinstarMorph"));

					randomMap[morphItem.Key] = morphLocation.Value;
					randomMap[morphLocation.Key] = StaticKeys.Morph;

					var result = false;
					if (gameCompletion == 1)
						result = traverser.VerifyBeatable(data, randomMap, new Inventory(startingInventory));
					else if (gameCompletion == 2)
						result = traverser.VerifyFullCompletable(data, randomMap, new Inventory(startingInventory));

					if (result)
					{
						Console.WriteLine($"Randomization complete after {count} attempts - Successful seed: {seed}");

						_Timer.Stop();

						resultItemMap = randomMap;
						return;
					}

					if (count % 50 == 0)
					{
						Console.WriteLine($"Attempts {count}");
					}
				}

				count++;

				seed = random.Next();
			}
		}

		public static BaseKey TranslateKey(SaveData data, string keyName)
		{
			var allKeys = data.BasicKeys.Values.SelectMany(keylist => keylist.Values);
			return allKeys.FirstOrDefault(key => key.Name.Replace(" ", "").Equals(keyName, StringComparison.InvariantCultureIgnoreCase));
		}

		public static int ChooseRandomization()
		{
			Console.WriteLine($"Randomization type?");
			Console.WriteLine($"1. Smart placement (default)");
			Console.WriteLine($"2. Chaos");

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

		public static string ChooseSettings(List<string> settingFiles)
		{
			if(settingFiles.Any())
			{ 
				Console.WriteLine($"Choose a setting file or enter setting string(optional):");
				Console.WriteLine(settingFiles
					.Select((file, i) => $"{i + 1}: {Path.GetFileName(file)}")
					.Aggregate((i, j) => i + Environment.NewLine + j));
			}
			else
			{
				Console.WriteLine($"Enter setting string(optional):");
			}

			var response = Console.ReadLine();

			if (int.TryParse(response, out var result))
			{
				if (result < 1 || result > settingFiles.Count() + 1)
				{
					_logMessage += $"{result} is not a valid index{Environment.NewLine}";
					return null;
				}

				return settingFiles[result - 1];
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
