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
		
		public override void OnEnable()
		{
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

		public override string getName { get; } = "OhNeinSix";
	}
}