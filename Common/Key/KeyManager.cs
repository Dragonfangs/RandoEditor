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
				SetDefaultKeys();
				return;
			}

			myBasicKeys = someData.BasicKeys;;

			myCustomKeys = someData.CustomKeys;
		}

		static public void SetRandomKeyMap(Dictionary<string, Guid> randomMap)
		{
			instance.myRandomKeyMap = randomMap;
		}

		private void SetDefaultKeys()
		{
			var resourceReader = new System.IO.StreamReader(new System.IO.MemoryStream(Properties.Resources.defaultKeys));
			myBasicKeys = JsonConvert.DeserializeObject< Dictionary<string, Dictionary<Guid, BaseKey>>>(resourceReader.ReadToEnd());
		}

		public static void SaveKeys(SaveData.SaveData someData)
		{
			instance.SaveKeysInternal(someData);
		}

		private void SaveKeysInternal(SaveData.SaveData someData)
		{
			someData.BasicKeys = myBasicKeys;

			someData.CustomKeys = myCustomKeys;
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
			if (instance.myCustomKeys.ContainsKey(id))
			{
				return instance.myCustomKeys[id];
			}

			return instance.myBasicKeys.SelectMany(keys => keys.Value).Where(key => key.Key == id).Select(x => x.Value).FirstOrDefault();
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
