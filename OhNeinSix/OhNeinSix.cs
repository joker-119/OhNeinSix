using Smod2;
using Smod2.API;
using Smod2.Config;
using Smod2.Events;
using Smod2.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Harmony;
using MEC;

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
		public readonly System.Random Gen = new System.Random();

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

		[ConfigOption]
		public List<int> BlacklistedRoles = new List<int>() { 14 };

		public static List<int> Targets = new List<int>();
		public static List<int> Raged = new List<int>();

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
			Patch.Ghost.PatchMethodsUsingHarmony();

			AddEventHandlers(new EventHandler(this), Smod2.Events.Priority.Normal);

			Functions = new Methods(this);
			BlacklistedRoles.Add(2);
		}
	}
}