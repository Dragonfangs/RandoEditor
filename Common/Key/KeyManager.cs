using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Common.SaveData;

namespace Common.Key
{
	public class KeyManager
	{
		private Dictionary<string, Dictionary<Guid, BaseKey>> myBasicKeys = new Dictionary<string, Dictionary<Guid, BaseKey>>();
		private Dictionary<Guid, ComplexKey> myCustomKeys = new Dictionary<Guid, ComplexKey>();

		private Dictionary<string, Guid> myRandomKeyMap = new Dictionary<string, Guid>();

		protected static KeyManager instance = new KeyManager();

		public static void Initialize(SaveData.SaveData someData)
		{
			instance.LoadKeys(someData);
		}

		private void LoadKeys(SaveData.SaveData someData)
		{
			if(!someData.BasicKeys.ContainsKey("Random") ||
				!someData.BasicKeys.ContainsKey("Event") ||
				!someData.BasicKeys.ContainsKey("Setting"))
			{
				SetDefaultKeys(someData);
			}

			myBasicKeys = someData.BasicKeys;

			myCustomKeys = someData.CustomKeys;
		}

		static public void SetRandomKeyMap(Dictionary<string, Guid> randomMap)
		{
			instance.myRandomKeyMap = randomMap;
		}

		private void SetDefaultKeys(SaveData.SaveData someData)
		{
			var resourceReader = new System.IO.StreamReader(new System.IO.MemoryStream(Properties.Resources.defaultKeys));
			someData.BasicKeys = JsonConvert.DeserializeObject< Dictionary<string, Dictionary<Guid, BaseKey>>>(resourceReader.ReadToEnd());
		}

		static public ICollection<BaseKey> GetRandomKeys()
		{
			return instance.myBasicKeys["Random"].Values;
		}

		static public ICollection<BaseKey> GetEventKeys()
		{
			return instance.myBasicKeys["Event"].Values;
		}

		static public ICollection<BaseKey> GetSettingKeys()
		{
			return instance.myBasicKeys["Setting"].Values;
		}

		static public bool IsSetting(Guid keyId)
		{
			return instance.myBasicKeys["Setting"].ContainsKey(keyId);
		}

		static public ICollection<ComplexKey> GetCustomKeys()
		{
			return instance.myCustomKeys.Values;
		}

		static public void DeleteKey(Guid id)
		{
			if (instance.myCustomKeys.ContainsKey(id))
			{
				instance.myCustomKeys.Remove(id);
				return;
			}

			foreach (var collection in instance.myBasicKeys.Values)
			{
				if (collection.ContainsKey(id))
				{
					collection.Remove(id);
					return;
				}
			}
		}

		static public void SaveCustomKey(Guid id, ComplexKey key)
		{
			instance.myCustomKeys[id] = key;
		}

		static public void SaveEventKey(Guid id, BaseKey key)
		{
			instance.myBasicKeys["Event"][id] = key;
		}

		static public void SaveSettingKey(Guid id, BaseKey key)
		{
			instance.myBasicKeys["Setting"][id] = key;
		}

		static public BaseKey GetKey(Guid id)
		{
			if (instance.myCustomKeys.ContainsKey(id))
			{
				return instance.myCustomKeys[id];
			}

			return instance.myBasicKeys.SelectMany(keys => keys.Value).Where(key => key.Key == id).Select(x => x.Value).FirstOrDefault();
		}

        static public string GetKeyName(Guid id)
        {
            return GetKey(id)?.Name ?? "Blank";
        }

        public static BaseKey GetKeyFromName(string keyName)
        {
            var allKeys = instance.myBasicKeys.Values.SelectMany(keylist => keylist.Values);
            return allKeys.FirstOrDefault(key => key.Name.Replace(" ", "").Equals(keyName.Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase));
        }

        static public string GetMappedRandomKeyName(string id)
        {
            if (instance.myRandomKeyMap.ContainsKey(id))
            {
                return GetKeyName(instance.myRandomKeyMap[id]);
            }

            return "Blank";
        }

        static public BaseKey GetMappedRandomKey(string id)
		{
			if (instance.myRandomKeyMap.ContainsKey(id))
			{
				return GetKey(instance.myRandomKeyMap[id]);
			}

			return null;
		}
	}
}
