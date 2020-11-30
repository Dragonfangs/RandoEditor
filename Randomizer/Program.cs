using Common.Key;
using Common.SaveData;
using Common.Utils;
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
		static Stopwatch _Timer = new Stopwatch();

		static void Main(string[] args)
		{
			List<string> settings = new List<string>
			{
				"ObtainUnknownItems",
				"CanWalljump",
				"CanInfiniteBombJump",
				"PlasmaBeamNotRequired",
				"IceBeamNotRequired",
			};
			var result = Randomize(settings);
			var map = result.Select(pair => $"{pair.Key}:{KeyManager.GetKey(pair.Value)?.Name ?? "None"}").Aggregate((i, j) => i + $",{Environment.NewLine}" + j);
			Console.WriteLine(map);
			File.WriteAllText(Environment.CurrentDirectory + "//itemMap.txt", map);
			Console.WriteLine($"{Environment.NewLine}Time taken: {_Timer.Elapsed}");
			Console.ReadKey();
		}

		public static Dictionary<string, Guid> Randomize(List<string> settingKeys)
		{
			var fileNames = new List<string>(Directory.GetFiles(Environment.CurrentDirectory));

			var logicFile = ChooseLogic(fileNames.Where(x => Path.GetExtension(x) == ".lgc").ToList());
			if (logicFile == null)
			{
				// Log issue
				_logMessage += $"No logic file chosen{Environment.NewLine}";
				return new Dictionary<string, Guid>();
			}

			var data = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(logicFile));

			Console.WriteLine();

			KeyManager.Initialize(data);

			var traverser = new NodeTraverser();

			var inventory = new Inventory();
			inventory.myKeys = settingKeys.Select(key => TranslateKey(data, key)).ToList();
			
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

				//pool.RemoveRandomItems(92, random);

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

				//_logMessage += Environment.NewLine;
				//var fullCompleteResult = traverser.VerifyFullCompletable(data, randomMap, new Inventory(inventory));
				//_logMessage += $"100%: {fullCompleteResult}{Environment.NewLine}{traverser.GetWaveLog()}{Environment.NewLine}{Environment.NewLine}";

				/*var unreachablenodes = traverser.GetUnreachable();
				if (unreachablenodes.Any())
				{
					_logMessage += $"Unreachable: {traverser.GetUnreachable().Select(node => Utility.GetNodeName(node)).Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}{Environment.NewLine}";
				}*/

				var beatableResult = traverser.VerifyBeatable(data, randomMap, new Inventory(inventory));
				//_logMessage += $"Beatable: {beatableResult}{Environment.NewLine}{traverser.GetWaveLog()}";

				if(beatableResult)
				{
					Console.WriteLine($"Maybeeeee {count} - {seed}");
					var fullCompleteResult = traverser.VerifyFullCompletable(data, randomMap, new Inventory(inventory));
					if (fullCompleteResult)
					{
						Console.WriteLine($"woop {count} - {seed}");

						_Timer.Stop();

						return randomMap;
					}
					count++;
				}
				else
				{
					if (count % 50 == 0)
					{
						Console.WriteLine($"nope {count}");
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
	}
}
