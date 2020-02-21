using System.Collections.Generic;
using EXILED;
using Harmony;
using MEC;

namespace OhNeinSix
{
	public class Plugin : EXILED.Plugin
	{
		public static List<int> Scp096Targets = new List<int>();
		public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
		public EventHandlers EventHandlers;

		public bool Enabled;
		public float MaxRange;
		public float MaxSurfaceRange;
		public static float MaxTriggerRange;
		public static float MaxTriggerRangeSurface;
		public bool AddOnDamage;
		public float DamageResistance;
		public List<int> BlacklistedRoles;
		public float PunishDelay;
		public int PunishDamage;
		public bool ExtremePunishment;
		public float PunishMultiplier;
		public bool EnragedBypass;
		public float CooldownTime;
		public int HealAmount;

		private static int _patchCounter;
		
		public override void OnEnable()
		{
			ReloadConfig();

			if (!Enabled)
				return;
			
			EventPlugin.GhostmodePatchDisable = true;
			EventHandlers = new EventHandlers(this);
			Events.Scp096CalmEvent += EventHandlers.OnCalm;
			Events.Scp096EnrageEvent += EventHandlers.OnEnrage;
			Events.PlayerDeathEvent += EventHandlers.OnPlayerDeath;
			Events.PlayerHurtEvent += EventHandlers.OnPlayerHurt;
			Events.RoundEndEvent += EventHandlers.OnRoundEnd;
			Events.RoundStartEvent += EventHandlers.OnRoundStart;
			Events.WaitingForPlayersEvent += EventHandlers.OnWaitingForPlayers;
			_patchCounter++;
			HarmonyInstance instance = HarmonyInstance.Create($"com.joker.ohneinsix-{_patchCounter}");
			instance.PatchAll();
		}

		public override void OnDisable()
		{
			if (!Enabled)
				return;
			
			Events.Scp096CalmEvent -= EventHandlers.OnCalm;
			Events.Scp096EnrageEvent -= EventHandlers.OnEnrage;
			Events.PlayerDeathEvent -= EventHandlers.OnPlayerDeath;
			Events.PlayerHurtEvent -= EventHandlers.OnPlayerHurt;
			Events.RoundEndEvent -= EventHandlers.OnRoundEnd;
			Events.RoundStartEvent -= EventHandlers.OnRoundStart;
			Events.WaitingForPlayersEvent -= EventHandlers.OnWaitingForPlayers;
			EventHandlers = null;
		}

		public override void OnReload()
		{
			//Ignored
		}

		private void ReloadConfig()
		{
			Enabled = Config.GetBool("oh96_enabled", true);
			MaxRange = Config.GetFloat("oh96_max_range", 80f);
			DamageResistance = Config.GetFloat("oh96_damage_resistance", 0.5f);
			BlacklistedRoles = Config.GetIntList("oh96_blacklisted_roles");
			PunishDelay = Config.GetFloat("oh96_punish_delay", 5f);
			PunishDamage = Config.GetInt("oh96_punish_dmg", 35);
			PunishMultiplier = Config.GetFloat("oh96_punish_multiplier", 1.35f);
			ExtremePunishment = Config.GetBool("oh96_extreme_punishment", true);
			EnragedBypass = Config.GetBool("oh96_enraged_bypass", true);
			CooldownTime = Config.GetFloat("oh96_cooldown_time", 10f);
			HealAmount = Config.GetInt("oh96_heal_amount", 65);
			MaxSurfaceRange = Config.GetFloat("oh96_max_surface_range", 80f);
			AddOnDamage = Config.GetBool("oh96_damage_causes_target", true);
			MaxTriggerRange = Config.GetFloat("oh96_trigger_range", 20f);
			MaxTriggerRangeSurface = Config.GetFloat("oh09_trigger_range_surface", 30f);
		}

		public override string getName { get; } = "OhNeinSix";
	}
}