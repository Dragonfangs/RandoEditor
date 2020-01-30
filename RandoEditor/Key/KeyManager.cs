using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using RandoEditor.SaveData;

namespace RandoEditor.Key
{
	public class KeyManager
	{
		Dictionary<string, Dictionary<Guid, BaseKey>> myBasicKeys = new Dictionary<string, Dictionary<Guid, BaseKey>>();
		Dictionary<Guid, ComplexKey> myCustomKeys = new Dictionary<Guid, ComplexKey>();

		private static KeyManager instance = new KeyManager();

		public static void Initialize()
		{
			instance.LoadKeys();
		}

		private void LoadKeys()
		{
			if(!SaveManager.Data.BasicKeys.ContainsKey("Random") ||
				!SaveManager.Data.BasicKeys.ContainsKey("Event") ||
				!SaveManager.Data.BasicKeys.ContainsKey("Setting"))
			{
				SetDefaultKeys();
				return;
			}

			myBasicKeys = SaveManager.Data.BasicKeys;;

			myCustomKeys = SaveManager.Data.CustomKeys;
			foreach (var key in myCustomKeys.Values)
			{
				key.myRequirement.ConnectKeys();
			}
		}

		private void SetDefaultKeys()
		{
			var resourceReader = new System.IO.StreamReader(new System.IO.MemoryStream(Properties.Resources.defaultKeys));
			myBasicKeys = JsonConvert.DeserializeObject< Dictionary<string, Dictionary<Guid, BaseKey>>>(resourceReader.ReadToEnd());
		}

		public static void SaveKeys()
		{
			instance.SaveKeysInternal();
		}

		private void SaveKeysInternal()
		{
			SaveManager.Data.BasicKeys = myBasicKeys;

			SaveManager.Data.CustomKeys = myCustomKeys;
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
	}
}
