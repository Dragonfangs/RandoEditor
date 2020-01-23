using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verifier.Key;
using Verifier.Node;
using Verifier.SaveData;

namespace Verifier
{
	class Program
	{
		static void Main(string[] args)
		{
			SaveManager.Load();

			KeyManager.Initialize();

			var randomMap = new Dictionary<string, Guid>();

			//Random test data, probably convert to unit test eventually
			/*
			// Trivial
			randomMap.Add("1", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("8", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			*/

			/*
			// Easy
			randomMap.Add("1", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("6", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			*/

			/*
			// Medium
			randomMap.Add("1", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("8", Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de")); //Space
			randomMap.Add("3", Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34")); //Screw
			randomMap.Add("4", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			*/

			
			// Hard
			randomMap.Add("1", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("8", Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de")); //Space
			randomMap.Add("3", Guid.Parse("fde4a9a5-1b38-4c4a-a918-8556a55e8e98")); //Power Bombs
			randomMap.Add("Crateria1", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			randomMap.Add("Norfair1", Guid.Parse("215858d5-c361-4863-94cd-5eb68aecdc6b")); //Grip
			

			/*
			// Trivial Impossible
			randomMap.Add("1", Guid.Parse("7d38dbe9-69e7-4cd1-b893-a219fa499129")); //Gravity
			randomMap.Add("8", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("6", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			*/

			/*
			// Easy Impossible
			randomMap.Add("1", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("8", Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de")); //Space
			*/

			/*
			// Hard Impossible
			randomMap.Add("1", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("8", Guid.Parse("7d38dbe9-69e7-4cd1-b893-a219fa499129")); //Gravity
			randomMap.Add("6", Guid.Parse("215858d5-c361-4863-94cd-5eb68aecdc6b")); //Grip
			randomMap.Add("7", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			*/

			KeyManager.SetRandomKeyMap(randomMap);

			var traverser = new NodeTraverser();

			if (traverser.VerifyBeatable())
				Console.Write("Yeah boi" + Environment.NewLine);
			else
				Console.Write("Nah fam" + Environment.NewLine);
			
		}
	}
}
