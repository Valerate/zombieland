﻿using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace ZombieLand
{
	[StaticConstructorOnStartup]
	public static class Constants
	{
		static Constants()
		{
			var settingsPath = Tools.GetModRootDirectory() + Path.DirectorySeparatorChar + "About" + Path.DirectorySeparatorChar + "Settings.txt";
			File.ReadAllLines(settingsPath)
				.Select(line => line.Trim())
				.Where(line => line.StartsWith("/", StringComparison.Ordinal) == false && line.Length > 0)
				.Select(line =>
				{
					var parts = line.Split('=').Select(part => part.Trim()).ToList();
					parts.Insert(0, line);
					return parts.ToArray();
				})
				.Do(parts =>
				{
					if (parts.Length != 3)
						Log.Error("Unexpected line: " + parts[0]);
					else
					{
						var field = parts[1];
						var value = parts[2];
						var constant = typeof(Constants).Field(field);

						switch (constant.FieldType.Name)
						{
							case "Boolean":
								{
									if (bool.TryParse(value, out var result))
										constant.SetValue(null, result);
									else
										Log.Error("Cannot parse boolean '" + value + "' of constant " + field);
									break;
								}
							case "Int32":
								{
									if (int.TryParse(value, out var result))
										constant.SetValue(null, result);
									else
										Log.Error("Cannot parse int '" + value + "' of constant " + field);
									break;
								}
							case "Single":
								{
									if (float.TryParse(value, out var result))
										constant.SetValue(null, result);
									else
										Log.Error("Cannot parse float '" + value + "' of constant " + field);
									break;
								}
							default:
								Log.Error("Zombieland constant '" + field + "' with unknown type " + constant.FieldType.Name);
								break;
						}
					}
				});
		}

		// general debugging and testing
		//
		public static bool DEBUGGRID;
		public static bool USE_SOUND = true;
		public static int DEBUG_MAX_ZOMBIE_COUNT = -1;

		// timing
		//
		public static float PHEROMONE_FADEOFF = 180f;
		public static float TICKMANAGER_RECALCULATE_DELAY = 5f;
		public static float TICKMANAGER_AVOIDGRID_DELAY = 0.25f;
		public static int EAT_DELAY_TICKS = 1800;

		// zombie spawning
		// the following hours continue after 23h with 24, 25, 26...
		//
		public static int HOUR_START_OF_DUSK = 18;
		public static int HOUR_START_OF_NIGHT = 22;
		public static int HOUR_END_OF_NIGHT = 28;
		public static int HOUR_START_OF_DAWN = 32;

		// zombie stats
		//
		public static float ANIMAL_PHEROMONE_RADIUS = 2f;
		public static float HUMAN_PHEROMONE_RADIUS = 4f;
		public static float ZOMBIE_HIT_CHANCE_IDLE = 0.2f;
		public static float ZOMBIE_HIT_CHANCE_TRACKING = 0.7f;
		public static int NUMBER_OF_TOP_MOVEMENT_PICKS = 3;
		public static float CLUSTER_AVOIDANCE_CHANCE = 0.25f;
		public static float DIVERTING_FROM_RAGE = 0.1f;
		public static int ZOMBIE_CLOGGING_FACTOR = 10000;
		public static float KILL_CIRCLE_RADIUS_MULTIPLIER;
		public static float BASE_MUZZLE_FLASH_VALUE = 9f;
		public static float SURROUNDING_ZOMBIES_TO_TRIGGER_RAGE = 14;
		public static float COLONISTS_HIT_ZOMBIES_CHANCE = 0.9f;

		// zombie incidents
		//
		public static int SPAWN_INCIDENT_RADIUS = 10;
		public static int MIN_ZOMBIE_SPAWN_CELL_COUNT = 6;

		// rubble
		//
		public static int MIN_DELTA_TICKS = 20;
		public static int MAX_DELTA_TICKS = 4;
		public static int RUBBLE_AMOUNT = 50;
		public static float MAX_HEIGHT = 0.6f;
		public static float MIN_SCALE = 0.05f;
		public static float MAX_SCALE = 0.3f;
		public static float ZOMBIE_LAYER = Altitudes.AltitudeFor(AltitudeLayer.Pawn) - 0.005f;
		public static float EMERGE_DELAY = 0.8f;

		// misc
		//
		public static float MIN_WEAPON_RANGE = 2f;
		public static float MAX_WEAPON_RANGE = 30f;
		public static float MIN_ATTACKDISTANCE_SQUARED = 2.25f;
		public static Material RAGE_EYE = MaterialPool.MatFrom("RageEye", ShaderDatabase.Mote);
		public static Material BOMB_LIGHT = MaterialPool.MatFrom("BombLight", ShaderDatabase.MoteGlow);
		public static Dictionary<CameraZoomRange, Material> RAGE_AURAS = new Dictionary<CameraZoomRange, Material> {
			{ CameraZoomRange.Closest, MaterialPool.MatFrom("RageAura", ShaderDatabase.Mote, new Color(1f, 1f, 1f, 0.15f)) },
			{ CameraZoomRange.Close, MaterialPool.MatFrom("RageAura", ShaderDatabase.Mote, new Color(1f, 1f, 1f, 0.175f)) },
			{ CameraZoomRange.Middle, MaterialPool.MatFrom("RageAura", ShaderDatabase.Mote, new Color(1f, 1f, 1f, 0.2f)) },
			{ CameraZoomRange.Far, MaterialPool.MatFrom("RageAura", ShaderDatabase.Mote, new Color(1f, 1f, 1f, 0.25f)) },
			{ CameraZoomRange.Furthest, MaterialPool.MatFrom("RageAura", ShaderDatabase.Mote, new Color(1f, 1f, 1f, 0.3f)) }
		};
		public static Material[] TOXIC_AURAS = new Material[] {
			MaterialPool.MatFrom("ToxicAura", ShaderDatabase.Mote, new Color(1f, 1f, 1f, 0.3f)),
			MaterialPool.MatFrom("ToxicAura", ShaderDatabase.Mote, new Color(1f, 1f, 1f, 0.5f)),
			MaterialPool.MatFrom("ToxicAura", ShaderDatabase.Mote, new Color(1f, 1f, 1f, 0.7f)),
			MaterialPool.MatFrom("ToxicAura", ShaderDatabase.Mote, new Color(1f, 1f, 1f, 0.9f)),
			MaterialPool.MatFrom("ToxicAura", ShaderDatabase.Mote, new Color(1f, 1f, 1f, 1.0f))
		};
		public static Material RAGING = MaterialPool.MatFrom("Rage", ShaderDatabase.Cutout);
		public static Material RUBBLE = MaterialPool.MatFrom("Rubble", ShaderDatabase.Cutout);
		public static Material BRRAINZ = MaterialPool.MatFrom("Brrainz", ShaderDatabase.Cutout);
		public static Material EATING = MaterialPool.MatFrom("Eating", ShaderDatabase.Cutout);
		public static System.Random random = new System.Random();
	}
}