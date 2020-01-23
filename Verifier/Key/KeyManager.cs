using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Verifier.SaveData;

namespace Verifier.Key
{
	public class KeyManager
	{
		Dictionary<Guid, BaseKey> myRandomizedKeys = new Dictionary<Guid, BaseKey>();
		Dictionary<Guid, BaseKey> myEventKeys = new Dictionary<Guid, BaseKey>();
		Dictionary<Guid, BaseKey> mySettingKeys = new Dictionary<Guid, BaseKey>();
		Dictionary<Guid, ComplexKey> myCustomKeys = new Dictionary<Guid, ComplexKey>();

		Dictionary<string, Guid> myRandomKeyMap = new Dictionary<string, Guid>();

		private static KeyManager instance = new KeyManager();

		public static void Initialize()
		{
			instance.LoadKeys();
		}

		private void LoadKeys()
		{
			if(!SaveManager.Data.BasicKeys.ContainsKey("Random"))
			{
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

		static public void SetRandomKeyMap(Dictionary<string, Guid> randomMap)
		{
			instance.myRandomKeyMap = randomMap;
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

		static public BaseKey GetMappedRandomKey(string id)
		{
			if(instance.myRandomKeyMap.ContainsKey(id))
			{
				return GetKey(instance.myRandomKeyMap[id]);
			}

			return null;
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
