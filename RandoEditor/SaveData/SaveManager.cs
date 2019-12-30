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

		// TODO: handle selecting your own save file
		private static string fileName = "ZeroMission.lgc";

		public static void Load()
		{
			Data = JsonConvert.DeserializeObject<SaveData>(System.IO.File.ReadAllText(fileName));
		}

		public static void Save()
		{
			System.IO.File.WriteAllText(fileName, JsonConvert.SerializeObject(Data));
		}

	}
}
