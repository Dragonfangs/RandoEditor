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

			// Current version
			Data.version = new Version(0, 1, 0, 0);
		}

		public static bool Open(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
				{
					Data = JsonConvert.DeserializeObject<Common.SaveData.SaveData>(System.IO.File.ReadAllText(fileName));
					HandleVersionUpdate();

					Properties.Settings.Default["LatestFilePath"] = fileName;
					Properties.Settings.Default.Save();

					Dirty = false;

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

		private static void HandleVersionUpdate()
		{
			//Do stuff in the future
		}

	}
}
