using System.Collections.Generic;
using EXILED;
using MEC;

namespace OhNeinSix
{
	public class Plugin : EXILED.Plugin
	{
		public static List<int> Scp096Targets = new List<int>();
		public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
		public EventHandlers EventHandlers;
		
		//Configs
		public bool Enabled;
		public float MaxRange;
		public float DamageResistance;
		public List<int> BlacklistedRoles;
		public float PunishDelay;
		public int PunishDamage;
		public bool ExtremePunishement;
		public float PunishMultiplier;
		public bool EnragedBypass;
		public float CooldownTime;

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
		}

		public override void OnDisable()
		{
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

		public void ReloadConfig()
		{
			Enabled = Config.GetBool("oh96_enabled", true);
			MaxRange = Config.GetFloat("oh96_max_range", 80f);
			DamageResistance = Config.GetFloat("oh96_damage_resistance", 0.5f);
			PunishDelay = Config.GetFloat("oh96_punish_delay", 5f);
			PunishDamage = Config.GetInt("oh96_punish_damage", 45);
			ExtremePunishement = Config.GetBool("oh96_extreme_punishement", true);
			PunishMultiplier = Config.GetFloat("oh95_punish_multiplier", 1.45f);
			EnragedBypass = Config.GetBool("oh96_enraged_bypass", true);
			BlacklistedRoles = Config.GetIntList("oh96_blacklisted_roles");
			CooldownTime = Config.GetFloat("oh96_cooldown_time", 10f);
		}

		public override string getName { get; } = "OhNeinSix";
	}
}