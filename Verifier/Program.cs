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

			var traverser = new NodeTraverser();

			//Random test data, probably convert to unit test eventually
			
			// Trivial
			randomMap.Add("BrinstarMorph",			Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("BrinstarFirstMissile",	Guid.Parse("71c295d4-ba02-410b-8e4d-a53f8cec36fa")); //Missile
			randomMap.Add("BrinstarHiveRoom",		Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			randomMap.Add("BrinstarBelowHives",		Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34")); //Screw

			KeyManager.SetRandomKeyMap(randomMap);
			if (!traverser.VerifyBeatable())
			{
				Console.Write("Nah fam 1" + Environment.NewLine);
				return;
			}
			randomMap.Clear();

			// Easy
			randomMap.Add("BrinstarMorph",			Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("BrinstarLongBeam",		Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			randomMap.Add("BrinstarFirstMissile",	Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34")); //Screw

			KeyManager.SetRandomKeyMap(randomMap);
			if (!traverser.VerifyBeatable())
			{
				Console.Write("Nah fam 2" + Environment.NewLine);
				return;
			}
			randomMap.Clear();
						
			// Medium
			randomMap.Add("BrinstarMorph",			Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("BrinstarFirstMissile",	Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de")); //Long
			randomMap.Add("BrinstarLongBeam",		Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de")); //Space
			randomMap.Add("BrinstarCeilingTank",	Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34")); //Screw
			randomMap.Add("BrinstarAboveSuper",		Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			
			KeyManager.SetRandomKeyMap(randomMap);
			if (!traverser.VerifyBeatable())
			{
				Console.Write("Nah fam 3" + Environment.NewLine);
				return;
			}
			randomMap.Clear();

			// Hard
			randomMap.Add("BrinstarMorph",			Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("BrinstarFirstMissile",	Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de")); //Space
			randomMap.Add("BrinstarCeilingTank",	Guid.Parse("fde4a9a5-1b38-4c4a-a918-8556a55e8e98")); //Power Bombs
			randomMap.Add("CrateriaUnderwater",		Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			randomMap.Add("NorfairGripMissile",		Guid.Parse("215858d5-c361-4863-94cd-5eb68aecdc6b")); //Grip

			KeyManager.SetRandomKeyMap(randomMap);
			if (!traverser.VerifyBeatable())
			{
				Console.Write("Nah fam 4" + Environment.NewLine);
				return;
			}
			randomMap.Clear();

			// Resource Success
			randomMap.Add("BrinstarMorph",				Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("BrinstarFirstMissile",		Guid.Parse("71c295d4-ba02-410b-8e4d-a53f8cec36fa")); //Missile
			randomMap.Add("BrinstarWorm",				Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34")); //Screw
			randomMap.Add("BrinstarHiveRoom",			Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de")); //Space
			randomMap.Add("BrinstarMissileAfterHives",	Guid.Parse("54d8e47c-b29b-440d-9a8b-bab7bc995f1d")); //E-tank
			randomMap.Add("BrinstarEtankAfterHives",	Guid.Parse("54d8e47c-b29b-440d-9a8b-bab7bc995f1d")); //E-tank
			randomMap.Add("BrinstarBombs",				Guid.Parse("6708fc69-861c-4b59-9cef-fab82697ed0d")); //Hi-jump
			randomMap.Add("BrinstarAcidTank",			Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs

			KeyManager.SetRandomKeyMap(randomMap);
			if (!traverser.VerifyBeatable())
			{
				Console.Write("Nah fam 5" + Environment.NewLine);
				return;
			}
			randomMap.Clear();

			// Resource Impossible
			randomMap.Add("BrinstarMorph",				Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("BrinstarFirstMissile",		Guid.Parse("71c295d4-ba02-410b-8e4d-a53f8cec36fa")); //Missile
			randomMap.Add("BrinstarWorm",				Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34")); //Screw
			randomMap.Add("BrinstarHiveRoom",			Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de")); //Space
			randomMap.Add("BrinstarMissileAfterHives",	Guid.Parse("54d8e47c-b29b-440d-9a8b-bab7bc995f1d")); //E-tank
			randomMap.Add("BrinstarEtankAfterHives",	Guid.Parse("71c295d4-ba02-410b-8e4d-a53f8cec36fa")); //Missile
			randomMap.Add("BrinstarBombs",				Guid.Parse("6708fc69-861c-4b59-9cef-fab82697ed0d")); //Hi-jump
			randomMap.Add("BrinstarAcidTank",			Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs

			KeyManager.SetRandomKeyMap(randomMap);
			if (traverser.VerifyBeatable())
			{
				Console.Write("Nah fam 6" + Environment.NewLine);
				return;
			}
			randomMap.Clear();

			// Trivial Impossible
			randomMap.Add("BrinstarMorph",			Guid.Parse("7d38dbe9-69e7-4cd1-b893-a219fa499129")); //Gravity
			randomMap.Add("BrinstarLongBeam",		Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("BrinstarFirstMissile",	Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs
			
			KeyManager.SetRandomKeyMap(randomMap);
			if (traverser.VerifyBeatable())
			{
				Console.Write("Nah fam 7" + Environment.NewLine);
				return;
			}
			randomMap.Clear();

			// Easy Impossible
			randomMap.Add("BrinstarMorph",			Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("BrinstarFirstMissile",	Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de")); //Space

			KeyManager.SetRandomKeyMap(randomMap);
			if (traverser.VerifyBeatable())
			{
				Console.Write("Nah fam 8" + Environment.NewLine);
				return;
			}
			randomMap.Clear();

			// Hard Impossible
			randomMap.Add("BrinstarMorph",			Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee")); //Morph
			randomMap.Add("BrinstarFirstMissile",	Guid.Parse("7d38dbe9-69e7-4cd1-b893-a219fa499129")); //Gravity
			randomMap.Add("BrinstarLongBeam",		Guid.Parse("215858d5-c361-4863-94cd-5eb68aecdc6b")); //Grip
			randomMap.Add("BrinstarTopShaft",		Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609")); //Bombs

			KeyManager.SetRandomKeyMap(randomMap);
			if (traverser.VerifyBeatable())
			{
				Console.Write("Nah fam 9" + Environment.NewLine);
				return;
			}
			randomMap.Clear();
			
			Console.Write("Yeah boi" + Environment.NewLine);
		}
	}
}
