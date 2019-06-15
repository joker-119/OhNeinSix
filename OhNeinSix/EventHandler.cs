using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using UnityEngine;
using MEC;
using TargetedGhostmode;

namespace OhNeinSix
{
	public class EventHandler : IEventHandlerWaitingForPlayers, IEventHandlerPlayerHurt, IEventHandlerPlayerDie,
		IEventHandlerSetRole, IEventHandlerScp096Panic, IEventHandlerScp096Enrage, IEventHandlerScp096CooldownStart
	{
		private readonly OhNeinSix plugin;
		public EventHandler(OhNeinSix plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!plugin.Enabled)
				PluginManager.Manager.DisablePlugin(plugin);

			OhNeinSix.Raged.Clear();
			OhNeinSix.Targets.Clear();
			foreach (int i in plugin.BlacklistedRoles)
				plugin.BlacklistedRoleList.Add(i);
			
			plugin.BlacklistedRoleList.Add(2);
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (ev.Role != Role.SCP_096) return;
			
			ev.Player.PersonalClearBroadcasts();
			ev.Player.PersonalBroadcast(10, "<color=#c50000><b>La forma en la que se juega con SCP-096 es muy diferente en este servidor. Por favor, revisa los cambios dandole la Ñ si eres nuevo.</b></color>", false);
			ev.Player.SendConsoleMessage
			(
				"La forma en la que se juega conSCP-096 ha sido drasticamente cambiada para mantenerlo mas fiel al material original. \n" +
				"Cuando te enfadas, se generará una lista de jugadores marcados como tus \'objetivos\'. \n" +
				"Estas seran las unicas personas a las que podras eliminar. El enfado no terminara hasta que tus objetivos no sean eliminados. \n" +
				"Sin embargo, por cada 5 segundos que estes enfadado el daño que sufres aumentará. Elimina rapido a tus objetivos. \n" +
				"Durante el enfado, los demas jugadores seguirán pudiendo verte, pero solo las granadas y elementos ambientales (como los teslas) pueden hacerte daño."
			);
			Timing.RunCoroutine(plugin.Functions.CheckInvisible(ev.Player));
		}

		

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (plugin.Panicked.Contains(ev.Player.PlayerId) && plugin.ResistantPanic)
				ev.Damage *= plugin.DamageResistance;
			
			if (ev.DamageType == DamageType.TESLA || ev.DamageType == DamageType.WALL || ev.DamageType == DamageType.NUKE || ev.DamageType == DamageType.FRAG || ev.DamageType == DamageType.DECONT) return;

			if (!OhNeinSix.Raged.Contains(ev.Player.PlayerId)) return;
			
			ev.Damage *= plugin.DamageResistance;

			if (!OhNeinSix.Targets.Contains(ev.Attacker.PlayerId))
			{
				OhNeinSix.Targets.Add(ev.Attacker.PlayerId);
				ev.Attacker.ShowPlayer(ev.Player);
			}
		}


		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (OhNeinSix.Targets.Contains(ev.Player.PlayerId))
			{
				OhNeinSix.Targets.Remove(ev.Player.PlayerId);
				if (ev.Killer.IsHiddenFrom(ev.Player))
					ev.Killer.ShowPlayer(ev.Player);

				if (OhNeinSix.Raged.Contains(ev.Killer.PlayerId) && plugin.RewardHeal)
					plugin.KillCounter++;
			}

			if (plugin.PreviousTargets.Contains(ev.Player.PlayerId))
				plugin.PreviousTargets.Remove(ev.Player.PlayerId);

			if (!OhNeinSix.Raged.Contains(ev.Player.PlayerId)) return;
			
			OhNeinSix.Raged.Remove(ev.Player.PlayerId);
			foreach (Player ply in plugin.Server.GetPlayers().Where(p => ev.Player.IsHiddenFrom(p)))
				ev.Player.ShowPlayer(ply);
			OhNeinSix.Targets.Clear();
		}

		public void OnScp096Panic(Scp096PanicEvent ev)
		{
			GameObject scp = (GameObject) ev.Player.GetGameObject();
			
			OhNeinSix.Raged.Add(ev.Player.PlayerId);
			plugin.Panicked.Add(ev.Player.PlayerId);
			
			foreach (int playerId in plugin.Functions.AddTargets(ev.Player))
				OhNeinSix.Targets.Add(playerId);

			if (OhNeinSix.Targets.Count <= 0)
			{
				ev.Allow = false;
				OhNeinSix.Raged.Remove(ev.Player.PlayerId);
				return;
			}
			
			Timing.RunCoroutine(plugin.Functions.GetClosestPlayer(ev.Player));
			Timing.RunCoroutine(plugin.Functions.Punish(ev.Player));

			if (plugin.EnragedBypass)
				scp.GetComponent<ServerRoles>().BypassMode = true;
		}

		public void OnScp096Enrage(Scp096EnrageEvent ev)
		{
			GameObject scp = (GameObject) ev.Player.GetGameObject();
			
			if (OhNeinSix.Targets.Count == 0)
			{
				ev.Allow = false;
				scp.GetComponent<Scp096PlayerScript>().enraged = Scp096PlayerScript.RageState.Cooldown;
			}

			plugin.Panicked.Remove(ev.Player.PlayerId);
		}

		public void OnScp096CooldownStart(Scp096CooldownStartEvent ev)
		{
			GameObject scp = (GameObject) ev.Player.GetGameObject();

			if (OhNeinSix.Targets.Count > 0)
			{
				ev.Allow = false;
				scp.GetComponent<Scp096PlayerScript>().enraged = Scp096PlayerScript.RageState.Enraged;
				return;
			}

			plugin.Panicked.Remove(ev.Player.PlayerId);
			OhNeinSix.Raged.Remove(ev.Player.PlayerId);

			if (plugin.RewardHeal)
			{
				ev.Player.AddHealth(plugin.RewardHealth * plugin.KillCounter);
				plugin.KillCounter = 0;
			}

			if (plugin.EnragedBypass)
				scp.GetComponent<ServerRoles>().BypassMode = false;
		}
	}
}
