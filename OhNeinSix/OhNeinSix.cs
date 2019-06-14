using Smod2;
using Smod2.Config;
using Smod2.Attributes;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;

namespace OhNeinSix
{
	[PluginDetails(
		author = "Joker119",
		name = "OhNeinSix",
		description = "A lore-friendly rework of SCP-096",
		id = "joker.OhNeinSix",
		version = "1.0.0",
		configPrefix = "oh96",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class OhNeinSix : Plugin
	{
		public Methods Functions { get; private set; }

		[ConfigOption]
		public bool Enabled = true;

		[ConfigOption]
		public bool ExtremePunishment = false;

		[ConfigOption]
		public bool EnragedBypass = true;

		[ConfigOption]
		public float PunishDelay = 5f;

		[ConfigOption]
		public float MaxRange = 80f;

		[ConfigOption]
		public float PunishMultiplier = 1.45f;

		[ConfigOption]
		public int PunishDamage = 45;

		[ConfigOption]
		public float DamageResistance = 0.5f;

		[ConfigOption] public bool ResistantPanic = true;

		[ConfigOption]
		public int[] BlacklistedRoles = { 14 };

		[ConfigOption]
		public int RewardHealth = 100;

		[ConfigOption]
		public bool RewardHeal = true;

		[ConfigOption] public bool HideAllies = false;

		[ConfigOption] public bool PersistantTargets = false;

		public static List<int> Targets = new List<int>();
		public List<int> PreviousTargets = new List<int>();
		public static List<int> Raged = new List<int>();
		public List<int> Panicked = new List<int>();
		public List<int> BlacklistedRoleList = new List<int>();

		public int KillCounter { get; internal set; }

		public override void OnDisable()
		{
			Info(Details.name + " v." + Details.version + " has been disabled.");
		}

		public override void OnEnable()
		{
			Info(Details.name + " v." + Details.version + " has been enabled.");
		}

		public override void Register()
		{
			AddEventHandlers(new EventHandler(this));

			Functions = new Methods(this);
		}
	}
}