using Common.Key;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandoEditor.SaveData
{
	public class SaveManager
	{
		public static Common.SaveData.SaveData Data { get; set; } = new Common.SaveData.SaveData();

		public static Version CurrentVersion { get; set; } = new Version(0, 2, 0, 0);

		private static SaveManager instance = new SaveManager();

		public static bool localDirty;
		public static bool Dirty
		{
			get
			{
				return localDirty;
			}
			set
			{
				if (localDirty != value)
				{
					localDirty = value;
					instance.OnDirtyChanged(EventArgs.Empty);
				}
			}
		}

		public static event EventHandler DirtyChanged;

		protected virtual void OnDirtyChanged(EventArgs e)
		{
			EventHandler handler = DirtyChanged;
			handler?.Invoke(this, e);
		}

		public static string CurrentFile()
		{
			var current = (string)Properties.Settings.Default["LatestFilePath"];
			return (!string.IsNullOrEmpty(current) ? Path.GetFileName(current) : "Untitled");
		}

		public static void New()
		{
			Data.Nodes.Clear();
			Data.BasicKeys.Clear();
			Data.CustomKeys.Clear();

			Properties.Settings.Default["LatestFilePath"] = string.Empty;

			Dirty = false;

			Data.version = CurrentVersion;
		}

		public static bool Open(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
				{
					Data = JsonConvert.DeserializeObject<Common.SaveData.SaveData>(System.IO.File.ReadAllText(fileName));
					var versionUpdated = HandleVersionUpdate();

					Properties.Settings.Default["LatestFilePath"] = fileName;
					Properties.Settings.Default.Save();

					Dirty = versionUpdated;

					return true;
				}
			}
			catch (Exception)
			{
			}

			return false;
		}

		public static bool Save()
		{
			return Save((string)Properties.Settings.Default["LatestFilePath"]);
		}

		public static bool Save(string fileName)
		{
			try
			{
				File.WriteAllText(fileName, JsonConvert.SerializeObject(Data, Formatting.Indented));

				Properties.Settings.Default["LatestFilePath"] = fileName;
				Properties.Settings.Default.Save();

				Dirty = false;

				return true;
			}
			catch (Exception)
			{
			}

			return false;
		}

		private static bool HandleVersionUpdate()
		{
			var change = false;
			if(Data.version < new Version(0,2,0,0))
			{
				Handle0200Update();
				change = true;
			}

			Data.version = CurrentVersion;

			return change;
		}

		private static void Handle0200Update()
		{
			// Set specified keys to static
			var staticKeys = new List<Guid>
			{
				new Guid("e49b2720-00a8-4fa0-9036-bebfe971f73f"), // Game Start
				new Guid("6d247e3c-7bae-4609-8655-794868537ec3"), // Game Finish
				new Guid("0ba7dceb-ef55-4063-8132-6e4bea5114e7"), // Ice Beam Not Required
				new Guid("d734910e-a0eb-4f29-b81d-c129c4085355"), // Plasma Beam Not Required
				new Guid("ed4a621c-0703-4ed8-af77-40b8d74facb5"), // Obtain Unknown Items
			};

			var basicKeys = Data.BasicKeys.Values.SelectMany(collection => collection.Values);
			var keysToUpdate = basicKeys.Where(key => staticKeys.Contains(key.Id));
			foreach (var key in keysToUpdate)
			{
				key.Static = true;
			}

			// Update name of power grip event
			var powerGripKey = basicKeys.FirstOrDefault(key => key.Id == new Guid("a434f9c0-fa1f-4154-9094-5da245776db9"));
			if(powerGripKey != null)
			{
				powerGripKey.Name = "Power Grip (Event)";
			}

			// Add New Keys
			var enemyRandomizationKey = new BaseKey(new Guid("784c6b79-e1a3-4ad8-a3fa-1b2495171d39"), "Randomize Enemies");
			enemyRandomizationKey.Static = true;
			Data.BasicKeys["Setting"][enemyRandomizationKey.Id] = enemyRandomizationKey;

			var chozoStatuesHintsKey = new BaseKey(new Guid("399e6355-c2d2-42e5-bc89-33175c5bb3a9"), "Chozo Statue Hints");
			chozoStatuesHintsKey.Static = true;
			Data.BasicKeys["Setting"][chozoStatuesHintsKey.Id] = chozoStatuesHintsKey;
		}
	}
}
