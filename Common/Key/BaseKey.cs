﻿using System;

namespace Common.Key
{
	[Serializable]
	public class BaseKey
	{	
		public BaseKey(Guid id, string name)
		{
			Id = id;
			Name = name;
		}

		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool Static { get; set; }
		public bool Repeatable { get; set; }
	}
}
