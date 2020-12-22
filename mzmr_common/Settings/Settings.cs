﻿using mzmr_common.Items;
using mzmr_common.Utils;
using System;
using System.Collections.Generic;

namespace mzmr_common
{
	public enum Swap { Unchanged, LocalPool, GlobalPool }
	public enum Change { Unchanged, Shuffle, Random }
	public enum GameCompletion { NoLogic, Beatable, AllItems }

	public class Settings
	{
		public bool SwapOrRemoveItems
		{
			get
			{
				return AbilitySwap > Swap.Unchanged ||
					TankSwap > Swap.Unchanged ||
					NumItemsRemoved > 0;
			}
		}
		public int NumTanksRemoved
		{
			get { return NumItemsRemoved - NumAbilitiesRemoved.Value; }
		}
		public bool RemoveSpecificItems => NumAbilitiesRemoved != null;
		public bool RandomPalettes
		{
			get
			{
				return TilesetPalettes ||
				  EnemyPalettes || SamusPalettes || BeamPalettes;
			}
		}

		// items
		public Swap AbilitySwap;
		public Swap TankSwap;
		public int NumItemsRemoved;
		public int? NumAbilitiesRemoved;
		public GameCompletion Completion;
		public bool IceNotRequired;
		public bool PlasmaNotRequired;
		public bool NoPBsBeforeChozodia;
		public bool ChozoStatueHints;
		public bool InfiniteBombJump;
		public bool WallJumping;

		// locations
		public Dictionary<int, ItemType> CustomAssignments;

		// enemies
		public bool RandoEnemies;

		// palettes
		public bool TilesetPalettes;
		public bool EnemyPalettes;
		public bool SamusPalettes;
		public bool BeamPalettes;
		public int HueMinimum;
		public int HueMaximum;

		// music
		public Change MusicChange;
		public Swap MusicRooms;
		public Swap MusicBosses;
		public Swap MusicOthers;

		// tweaks
		public bool EnableItemToggle;
		public bool ObtainUnkItems;
		public bool HardModeAvailable;
		public bool PauseScreenInfo;
		public bool RemoveCutscenes;
		public bool SkipSuitless;
		public bool SkipDoorTransitions;

		// constructor
		public Settings(string config = null)
		{
			SetDefaults();

			if (string.IsNullOrEmpty(config)) { return; }

			BinaryTextReader btr;
			try
			{
				btr = new BinaryTextReader(config);
			}
			catch (FormatException)
			{
				throw new FormatException("Config string is not valid.");
			}

			// check version
			int major = btr.ReadNumber(4);
			int minor = btr.ReadNumber(4);
			int patch = btr.ReadNumber(4);
			string configVer = $"{major}.{minor}.{patch}";
			// convert settings
			switch (configVer)
			{
				case "1.4.0":
					LoadSettings(btr);
					break;
				case "1.3.0":
				case "1.3.1":
				case "1.3.2":
					LoadSettings_1_3_2(btr);
					break;
				default:
					throw new FormatException("Config string is not valid.");
			}
		}

		private void LoadSettings(BinaryTextReader btr)
		{
			// items
			AbilitySwap = (Swap)btr.ReadNumber(2);
			TankSwap = (Swap)btr.ReadNumber(2);
			if (btr.ReadBool())
			{
				NumItemsRemoved = btr.ReadNumber(7);
				if (btr.ReadBool())
					NumAbilitiesRemoved = btr.ReadNumber(4);
			}
			if (SwapOrRemoveItems)
			{
				Completion = (GameCompletion)btr.ReadNumber(2);
				IceNotRequired = btr.ReadBool();
				PlasmaNotRequired = btr.ReadBool();
				NoPBsBeforeChozodia = btr.ReadBool();
				ChozoStatueHints = btr.ReadBool();
				InfiniteBombJump = btr.ReadBool();
				WallJumping = btr.ReadBool();
			}

			// locations
			if (btr.ReadBool())
			{
				int count = btr.ReadNumber(7);
				for (int i = 0; i < count; i++)
				{
					int locNum = btr.ReadNumber(7);
					ItemType item = (ItemType)btr.ReadNumber(5);
					CustomAssignments[locNum] = item;
				}
			}

			// enemies
			RandoEnemies = btr.ReadBool();

			// palettes
			TilesetPalettes = btr.ReadBool();
			EnemyPalettes = btr.ReadBool();
			SamusPalettes = btr.ReadBool();
			BeamPalettes = btr.ReadBool();
			if (RandomPalettes)
			{
				if (btr.ReadBool())
					HueMinimum = btr.ReadNumber(8);
				if (btr.ReadBool())
					HueMaximum = btr.ReadNumber(8);
			}

			// music
			MusicChange = (Change)btr.ReadNumber(2);
			if (MusicChange != Change.Unchanged)
			{
				MusicRooms = (Swap)btr.ReadNumber(2);
				MusicBosses = (Swap)btr.ReadNumber(2);
				MusicOthers = (Swap)btr.ReadNumber(2);
			}

			// misc
			EnableItemToggle = btr.ReadBool();
			ObtainUnkItems = btr.ReadBool();
			HardModeAvailable = btr.ReadBool();
			PauseScreenInfo = btr.ReadBool();
			RemoveCutscenes = btr.ReadBool();
			SkipSuitless = btr.ReadBool();
			SkipDoorTransitions = btr.ReadBool();
		}

		private void LoadSettings_1_3_2(BinaryTextReader btr)
		{
			// items
			bool randAbilities = btr.ReadBool();
			bool randTanks = btr.ReadBool();
			if (randAbilities && randTanks)
			{
				AbilitySwap = Swap.GlobalPool;
				TankSwap = Swap.GlobalPool;
			}
			else if (randAbilities) AbilitySwap = Swap.LocalPool;
			else if (randTanks) TankSwap = Swap.LocalPool;

			if (btr.ReadBool())
			{
				NumItemsRemoved = btr.ReadNumber(7);
			}
			if (SwapOrRemoveItems)
			{
				Completion = (GameCompletion)btr.ReadNumber(2);
				IceNotRequired = btr.ReadBool();
				PlasmaNotRequired = btr.ReadBool();
				NoPBsBeforeChozodia = btr.ReadBool();
				ChozoStatueHints = btr.ReadBool();
				InfiniteBombJump = btr.ReadBool();
				WallJumping = btr.ReadBool();
			}

			// locations
			if (btr.ReadBool())
			{
				int count = btr.ReadNumber(7);
				for (int i = 0; i < count; i++)
				{
					int locNum = btr.ReadNumber(7);
					ItemType item = (ItemType)btr.ReadNumber(5);
					CustomAssignments[locNum] = item;
				}
			}

			// enemies
			RandoEnemies = btr.ReadBool();

			// palettes
			TilesetPalettes = btr.ReadBool();
			EnemyPalettes = btr.ReadBool();
			BeamPalettes = btr.ReadBool();
			if (RandomPalettes)
			{
				if (btr.ReadBool())
				{
					HueMinimum = btr.ReadNumber(8);
				}
				if (btr.ReadBool())
				{
					HueMaximum = btr.ReadNumber(8);
				}
			}

			// misc
			EnableItemToggle = btr.ReadBool();
			ObtainUnkItems = btr.ReadBool();
			HardModeAvailable = btr.ReadBool();
			PauseScreenInfo = btr.ReadBool();
			RemoveCutscenes = btr.ReadBool();
			SkipSuitless = btr.ReadBool();
			SkipDoorTransitions = btr.ReadBool();
		}

		private void SetDefaults()
		{
			// items
			AbilitySwap = Swap.Unchanged;
			TankSwap = Swap.Unchanged;
			NumItemsRemoved = 0;
			NumAbilitiesRemoved = null;
			Completion = GameCompletion.Beatable;
			IceNotRequired = false;
			PlasmaNotRequired = false;
			NoPBsBeforeChozodia = false;
			ChozoStatueHints = false;
			InfiniteBombJump = true;
			WallJumping = true;

			// locations
			CustomAssignments = new Dictionary<int, ItemType>();

			// enemies
			RandoEnemies = false;

			// palettes
			TilesetPalettes = false;
			EnemyPalettes = false;
			SamusPalettes = false;
			BeamPalettes = false;
			HueMinimum = 0;
			HueMaximum = 180;

			// music
			MusicChange = Change.Unchanged;
			MusicRooms = Swap.Unchanged;
			MusicBosses = Swap.Unchanged;
			MusicOthers = Swap.Unchanged;

			// misc
			EnableItemToggle = true;
			ObtainUnkItems = false;
			HardModeAvailable = true;
			PauseScreenInfo = false;
			RemoveCutscenes = true;
			SkipSuitless = false;
			SkipDoorTransitions = false;
		}
	}
}
