﻿using Newtonsoft.Json;
using System;

namespace Verifier.SaveData
{
	public class SaveManager
	{
		public static SaveData Data { get; set; } = new SaveData();

		private static SaveManager instance = new SaveManager();

		// TODO: handle selecting your own save file
		private static string fileName = "ZeroMission.lgc";

		public static void Load()
		{
			if (System.IO.File.Exists(fileName))
			{
				Data = JsonConvert.DeserializeObject<SaveData>(System.IO.File.ReadAllText(fileName));
				HandleVersionUpdate();
			}
			else
			{
				// Current version
				Data.version = new Version(0, 1, 0, 0);
			}
		}

		public static void Save()
		{
			System.IO.File.WriteAllText(fileName, JsonConvert.SerializeObject(Data));
		}

		private static void HandleVersionUpdate()
		{
			//Do stuff in the future
		}

	}
}