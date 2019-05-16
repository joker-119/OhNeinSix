using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using UnityEngine;
using MEC;

namespace OhNeinSix
{
	public class EventHandler : IEventHandlerWaitingForPlayers, IEventHandlerPlayerHurt, IEventHandlerPlayerDie, IEventHandler096Enrage, IEventHandlerSetRole
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
			ev.Player.PersonalBroadcast(10, "<color=#c50000><b>SCP-096 is heavily altered on this server. Please press ~ and review the changes if you are new to this server.</b></color>", false);
			ev.Player.SendConsoleMessage
			(
				"SCP-096 is heavily altered to change his gameplay to keep it more lore-friendly. \n" +
				"When you enrage, a list of people looking at you will be generated, and listed to you as \'targets\'. \n" +
				"These targets are the only people you will be able to see and kill. Your rage will not end until they are dead, or they get too far away. \n" +
				"However, you will take ever-increasing damage every 5sec while you stay enraged. Hunt down and kill your \'targets\' quickly. \n" +
				"During this enrage, other players can still see you, but only grenades and environmental damage (like teslas) can harm you."
			);
		}

		public void OnSetEnrage(Player096EnrageEvent ev)
		{
			GameObject scp = (GameObject)ev.Player.GetGameObject();

			switch (ev.EnrageState)
			{
				case EnrageState.Panic:
				{
					OhNeinSix.Raged.Add(ev.Player.PlayerId);
					plugin.Panicked.Add(ev.Player.PlayerId);

					foreach (int playerId in plugin.Functions.AddTargets(ev.Player)) 
						OhNeinSix.Targets.Add(playerId);

					if (OhNeinSix.Targets.Count <= 0)
					{
						ev.EnrageState = EnrageState.NotEnraged;
						OhNeinSix.Raged.Remove(ev.Player.PlayerId);
						return;
					}

					Timing.RunCoroutine(plugin.Functions.GetClosestPlayer(ev.Player));
					Timing.RunCoroutine(plugin.Functions.Punish(ev.Player));

					if (plugin.EnragedBypass)
						scp.GetComponent<ServerRoles>().BypassMode = true;
					break;
				}
				case EnrageState.Cooldown when OhNeinSix.Targets.Count > 0:
					ev.EnrageState = EnrageState.Enraged;
					ev.RageProgress = 1f;
					break;
				case EnrageState.Cooldown:
				{
					OhNeinSix.Raged.Remove(ev.Player.PlayerId);

					if (plugin.RewardHeal)
					{
						ev.Player.AddHealth(plugin.RewardHealth * plugin.KillCounter);
						plugin.KillCounter = 0;
					}

					if (plugin.EnragedBypass)
						scp.GetComponent<ServerRoles>().BypassMode = false;
					break;
				}
				case EnrageState.Enraged when OhNeinSix.Targets.Count == 0:
					ev.EnrageState = EnrageState.Cooldown;
					break;
				default:
					plugin.Panicked.Remove(ev.Player.PlayerId);
					break;
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (plugin.Panicked.Contains(ev.Player.PlayerId) && plugin.ResistantPanic)
				ev.Damage *= plugin.DamageResistance;
			
			if (ev.DamageType == DamageType.TESLA || ev.DamageType == DamageType.WALL || ev.DamageType == DamageType.NUKE || ev.DamageType == DamageType.FRAG || ev.DamageType == DamageType.DECONT) return;

			if (!OhNeinSix.Raged.Contains(ev.Player.PlayerId)) return;
			
			ev.Damage *= plugin.DamageResistance;

			if (!OhNeinSix.Targets.Contains(ev.Attacker.PlayerId))
				OhNeinSix.Targets.Add(ev.Attacker.PlayerId);
		}


		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (OhNeinSix.Targets.Contains(ev.Player.PlayerId))
			{
				OhNeinSix.Targets.Remove(ev.Player.PlayerId);

				if (OhNeinSix.Raged.Contains(ev.Killer.PlayerId) && plugin.RewardHeal)
					plugin.KillCounter++;
			}

			if (plugin.PreviousTargets.Contains(ev.Player.PlayerId))
				plugin.PreviousTargets.Remove(ev.Player.PlayerId);

			if (!OhNeinSix.Raged.Contains(ev.Player.PlayerId)) return;
			
			OhNeinSix.Raged.Remove(ev.Player.PlayerId);
			OhNeinSix.Targets.Clear();
		}
	}
}