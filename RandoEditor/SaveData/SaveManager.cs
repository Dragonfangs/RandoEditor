using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandoEditor.SaveData
{
	public class SaveManager
	{
		public static SaveData Data { get; set; } = new SaveData();

		private static SaveManager instance = new SaveManager();

		public static void New()
		{
			Data.Nodes.Clear();
			Data.BasicKeys.Clear();
			Data.CustomKeys.Clear();

			Properties.Settings.Default["LatestFilePath"] = string.Empty;

			// Current version
			Data.version = new Version(0, 1, 0, 0);
		}

		public static bool Open(string fileName)
		{
			try
			{
				if (System.IO.File.Exists(fileName))
				{
					Data = JsonConvert.DeserializeObject<SaveData>(System.IO.File.ReadAllText(fileName));
					HandleVersionUpdate();

					Properties.Settings.Default["LatestFilePath"] = fileName;
					Properties.Settings.Default.Save();

					return true;
				}
			}
			catch (Exception)
			{
			}

			return false;
		}

		public static bool Save(string fileName)
		{
			try
			{
				System.IO.File.WriteAllText(fileName, JsonConvert.SerializeObject(Data, Formatting.Indented));

				Properties.Settings.Default["LatestFilePath"] = fileName;
				Properties.Settings.Default.Save();

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
