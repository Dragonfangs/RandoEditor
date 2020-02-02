using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifierTests
{
	public class TestData
	{
		public static Dictionary<string, Dictionary<string, Guid>> Data = new Dictionary<string, Dictionary<string, Guid>>
		{
			{ "TrivialBeatable", new Dictionary<string, Guid>
				{
					{ "BrinstarMorph", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee") }, // Morph
					{ "BrinstarFirstMissile", Guid.Parse("71c295d4-ba02-410b-8e4d-a53f8cec36fa") }, // Missile
					{ "BrinstarHives", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609") }, // Bombs
					{ "BrinstarBelowHives", Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34") }, // Screw
				}
			},
			{ "EasyBeatable", new Dictionary<string, Guid>
				{
					{ "BrinstarMorph", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee") }, // Morph
					{ "BrinstarLongBeam", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609") }, // Bombs
					{ "BrinstarFirstMissile", Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34") }, // Screw
				}
			},
			{ "MediumBeatable", new Dictionary<string, Guid>
				{
					{ "BrinstarMorph", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee") }, // Morph
					{ "BrinstarFirstMissile", Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de") }, // Long
					{ "BrinstarLongBeam", Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de") }, // Space
					{ "BrinstarCeiling", Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34") }, // Screw
					{ "BrinstarAboveSuper", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609") }, // Bombs
				}
			},
			{ "HardBeatable", new Dictionary<string, Guid>
				{
					{ "BrinstarMorph", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee") }, // Morph
					{ "BrinstarFirstMissile", Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de") }, // Space
					{ "BrinstarCeiling", Guid.Parse("fde4a9a5-1b38-4c4a-a918-8556a55e8e98") }, // Power Bombs
					{ "CrateriaUnderwater", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609") }, // Bombs
					{ "NorfairGripMissile", Guid.Parse("215858d5-c361-4863-94cd-5eb68aecdc6b") }, // Grip
				}
			},
			{ "ResourceSuccess", new Dictionary<string, Guid>
				{
					{ "BrinstarMorph", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee") }, // Morph
					{ "BrinstarFirstMissile", Guid.Parse("71c295d4-ba02-410b-8e4d-a53f8cec36fa") }, // Missile
					{ "BrinstarWorm", Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34") }, // Screw
					{ "BrinstarHives", Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de") }, // Space
					{ "BrinstarMissileAfterHives", Guid.Parse("54d8e47c-b29b-440d-9a8b-bab7bc995f1d") }, // E-tank
					{ "BrinstarETankAfterHives", Guid.Parse("54d8e47c-b29b-440d-9a8b-bab7bc995f1d") }, // E-tank
					{ "BrinstarBombs", Guid.Parse("6708fc69-861c-4b59-9cef-fab82697ed0d") }, // Hi-jump
					{ "BrinstarVariaEtank", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609") }, // Bombs
				}
			},
			{ "ResourceImpossible", new Dictionary<string, Guid>
				{
					{ "BrinstarMorph", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee") }, // Morph
					{ "BrinstarFirstMissile", Guid.Parse("71c295d4-ba02-410b-8e4d-a53f8cec36fa") }, // Missile
					{ "BrinstarWorm", Guid.Parse("f1d75bb6-7f26-453c-b4b4-878153644a34") }, // Screw
					{ "BrinstarHives", Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de") }, // Space
					{ "BrinstarMissileAfterHives", Guid.Parse("54d8e47c-b29b-440d-9a8b-bab7bc995f1d") }, // E-tank
					{ "BrinstarETankAfterHives", Guid.Parse("71c295d4-ba02-410b-8e4d-a53f8cec36fa") }, // Missile
					{ "BrinstarBombs", Guid.Parse("6708fc69-861c-4b59-9cef-fab82697ed0d") }, // Hi-jump
					{ "BrinstarVariaEtank", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609") }, // Bombs
				}
			},
			{ "TrivialImpossible", new Dictionary<string, Guid>
				{
					{ "BrinstarMorph", Guid.Parse("7d38dbe9-69e7-4cd1-b893-a219fa499129") }, // Gravity
					{ "BrinstarLongBeam", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee") }, // Morph
					{ "BrinstarFirstMissile", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609") }, // Bombs
				}
			},
			{ "EasyImpossible", new Dictionary<string, Guid>
				{
					{ "BrinstarMorph", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee") }, // Morph
					{ "BrinstarFirstMissile", Guid.Parse("d1ec655a-b97d-4760-b860-5085d87975de") }, // Space
				}
			},
			{ "HardImpossible", new Dictionary<string, Guid>
				{
					{ "BrinstarMorph", Guid.Parse("905940c7-fcef-4e24-b662-1cb2bc9e3eee") }, // Morph
					{ "BrinstarFirstMissile", Guid.Parse("7d38dbe9-69e7-4cd1-b893-a219fa499129") }, // Gravity
					{ "BrinstarLongBeam", Guid.Parse("215858d5-c361-4863-94cd-5eb68aecdc6b") }, // Grip
					{ "BrinstarShaftTop", Guid.Parse("f1f716df-f21a-4293-beed-55df0e13b609") }, // Bombs
				}
			},
		};
	}
}
