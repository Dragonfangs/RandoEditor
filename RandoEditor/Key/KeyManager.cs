using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using RandoEditor.SaveData;

namespace RandoEditor.Key
{
	public class KeyManager
	{
		Dictionary<Guid, BaseKey> myRandomizedKeys = new Dictionary<Guid, BaseKey>();
		Dictionary<Guid, BaseKey> myEventKeys = new Dictionary<Guid, BaseKey>();
		Dictionary<Guid, BaseKey> mySettingKeys = new Dictionary<Guid, BaseKey>();
		Dictionary<Guid, ComplexKey> myCustomKeys = new Dictionary<Guid, ComplexKey>();

		private static KeyManager instance = new KeyManager();

		public static void Initialize()
		{
			instance.LoadKeys();
		}

		private void LoadKeys()
		{
			if(!SaveManager.Data.BasicKeys.ContainsKey("Random"))
			{
				SetDefaultKeys();
				return;
			}

			myRandomizedKeys = SaveManager.Data.BasicKeys["Random"];
			myEventKeys = SaveManager.Data.BasicKeys["Event"];
			mySettingKeys = SaveManager.Data.BasicKeys["Setting"];

			myCustomKeys = SaveManager.Data.CustomKeys;
			foreach (var key in myCustomKeys.Values)
			{
				key.myRequirement.ConnectKeys();
			}
		}

		private void SetDefaultKeys()
		{
			var randomReader = new System.IO.StreamReader(new System.IO.MemoryStream(Properties.Resources.defaultRandomKeys));
			myRandomizedKeys = JsonConvert.DeserializeObject<Dictionary<Guid, BaseKey>>(randomReader.ReadToEnd());

			var eventReader = new System.IO.StreamReader(new System.IO.MemoryStream(Properties.Resources.defaultEventKeys));
			myEventKeys = JsonConvert.DeserializeObject<Dictionary<Guid, BaseKey>>(eventReader.ReadToEnd());

			var settingKeys = new System.IO.StreamReader(new System.IO.MemoryStream(Properties.Resources.defaultSettingKeys));
			mySettingKeys = JsonConvert.DeserializeObject<Dictionary<Guid, BaseKey>>(settingKeys.ReadToEnd());
		}

		// Don't use this, only for debug purposes
		private void GenerateKeys()
		{
			var id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Morph"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Power Grip"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Speed Booster"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Hi-Jump"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Screw Attack"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Space Jump"));

			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Missile"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Super Missile"));

			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Bombs"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Power Bombs"));

			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Long Beam"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Charge Beam"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Ice Beam"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Wave Beam"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Plasma Beam"));

			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Varia Suit"));
			id = Guid.NewGuid();
			myRandomizedKeys.Add(id, new BaseKey(id, "Gravity Suit"));

			//-------------------------------------------------------------

			id = Guid.NewGuid();
			myEventKeys.Add(id, new BaseKey(id, "Ziplines"));

			id = Guid.NewGuid();
			myEventKeys.Add(id, new BaseKey(id, "Unknown Item 1"));
			id = Guid.NewGuid();
			myEventKeys.Add(id, new BaseKey(id, "Unknown Item 2"));
			id = Guid.NewGuid();
			myEventKeys.Add(id, new BaseKey(id, "Unknown Item 3"));
			id = Guid.NewGuid();
			myEventKeys.Add(id, new BaseKey(id, "Power Grip"));

			id = Guid.NewGuid();
			myEventKeys.Add(id, new BaseKey(id, "Kraid Defeated"));
			id = Guid.NewGuid();
			myEventKeys.Add(id, new BaseKey(id, "Ridley Defeated"));
			id = Guid.NewGuid();
			myEventKeys.Add(id, new BaseKey(id, "Charlie Defeated"));

			//-------------------------------------------------------------

			id = Guid.NewGuid();
			mySettingKeys.Add(id, new BaseKey(id, "Ice Beam Not Required"));
			id = Guid.NewGuid();
			mySettingKeys.Add(id, new BaseKey(id, "Plasma Beam Not Required"));

			id = Guid.NewGuid();
			mySettingKeys.Add(id, new BaseKey(id, "Can Infinite Bomb Jump"));
			id = Guid.NewGuid();
			mySettingKeys.Add(id, new BaseKey(id, "Can Wall jump"));

			id = Guid.NewGuid();
			mySettingKeys.Add(id, new BaseKey(id, "Obtain unknown items"));
			id = Guid.NewGuid();
			mySettingKeys.Add(id, new BaseKey(id, "Remove Norfair vine"));
		}

		public static void SaveKeys()
		{
			instance.SaveKeysInternal();
		}

		private void SaveKeysInternal()
		{
			SaveManager.Data.BasicKeys["Random"] = myRandomizedKeys;
			SaveManager.Data.BasicKeys["Event"] = myEventKeys;
			SaveManager.Data.BasicKeys["Setting"] = mySettingKeys;

			SaveManager.Data.CustomKeys = myCustomKeys;
		}

		static public ICollection<BaseKey> GetRandomKeys()
		{
			return instance.myRandomizedKeys.Values;
		}

		static public ICollection<BaseKey> GetEventKeys()
		{
			return instance.myEventKeys.Values;
		}

		static public ICollection<BaseKey> GetSettingKeys()
		{
			return instance.mySettingKeys.Values;
		}

		static public ICollection<ComplexKey> GetCustomKeys()
		{
			return instance.myCustomKeys.Values;
		}

		static public void DeleteCustomKey(Guid id)
		{
			if(!instance.myCustomKeys.ContainsKey(id))
			{
				return;
			}

			instance.myCustomKeys.Remove(id);
		}

		static public void SaveCustomKey(Guid id, ComplexKey key)
		{
			instance.myCustomKeys[id] = key;
		}

		static public BaseKey GetKey(Guid id)
		{
			if(instance.myRandomizedKeys.ContainsKey(id))
			{
				return instance.myRandomizedKeys[id];
			}

			if (instance.myEventKeys.ContainsKey(id))
			{
				return instance.myEventKeys[id];
			}

			if (instance.mySettingKeys.ContainsKey(id))
			{
				return instance.mySettingKeys[id];
			}

			if (instance.myCustomKeys.ContainsKey(id))
			{
				return instance.myCustomKeys[id];
			}

			return null;
		}
	}
}
