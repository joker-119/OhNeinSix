using System;
using System.Collections.Generic;
using System.Linq;
using EXILED;
using MEC;
using UnityEngine;

namespace OhNeinSix
{
	public class EventHandlers
	{
		private readonly Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin; 
		public int TargetCount;
		private ReferenceHub scp096; 
	

		public void OnPlayerDeath(ref PlayerDeathEvent ev)
		{
			if (Plugin.Scp096Targets.Contains(ev.Player.queryProcessor.PlayerId))
			{
				Plugin.Scp096Targets.Remove(ev.Player.queryProcessor.PlayerId);
				TargetCount++;
			}
			else if (ev.Player.characterClassManager.CurClass == RoleType.Scp096 || ev.Player == scp096)
			{
				Timing.KillCoroutines("checkranges");
				Timing.KillCoroutines("punish");
				Plugin.Scp096Targets.Clear();
				ev.Player.serverRoles.BypassMode = false;
				scp096 = null;
			}
		}

		private const int KWallMask =
			1 << 30 | // Lockers
			1 << 27 | // Door
			1 << 14 | // Glass
			1 << 9 | // Pickups
			1 << 0;   // Default
		
		public IEnumerator<float> GetClosestPlayer(Scp096PlayerScript script, ReferenceHub player, List<ReferenceHub> players)
		{
			yield return Timing.WaitForSeconds(5.5f);
			while (script.Networkenraged == Scp096PlayerScript.RageState.Enraged)
			{
				foreach (int target in Plugin.Scp096Targets)
				{
					if (Plugin.GetPlayer(target.ToString()) == null)
						Plugin.Scp096Targets.Remove(target);
				}

				float min = 100f;
				foreach (ReferenceHub tar in players)
				{
					if (!Plugin.Scp096Targets.Contains(tar.queryProcessor.PlayerId) || tar == player)
						continue;

					float distance = Vector3.Distance(player.gameObject.transform.position, tar.gameObject.transform.position);
					
					if (distance >= plugin.MaxRange)
					{
						Plugin.Scp096Targets.Remove(tar.queryProcessor.PlayerId);
						TargetCount--;
					}
					else
					{
						if (distance < min)
							min = distance;
					}
				}

				double value = (plugin.MaxRange - min) / plugin.MaxRange;
				string bar = DrawBar(value);

				player.GetComponent<Broadcast>().TargetClearElements(player.characterClassManager.connectionToClient);
				player.Broadcast(1, "<size=30><color=#c50000>Distance to nearest target: </color><color=#10F110>" + bar + "</color></size> \n" + "<size=25>Targets Remaining: <color=#c50000>" + Plugin.Scp096Targets.Count + "</color></size>");
				

				yield return Timing.WaitForSeconds(0.5f);
			}
		}

		private IEnumerator<float> Punish(Scp096PlayerScript script, ReferenceHub rh)
		{
			while (script.Networkenraged == Scp096PlayerScript.RageState.Panic)
				yield return Timing.WaitForSeconds(0.5f);
			
			Plugin.Debug($"Starting punishment loop..");
			yield return Timing.WaitForSeconds(plugin.PunishDelay * 2);
			int counter = 0;
			for (;;)
			{
				if (script.Networkenraged != Scp096PlayerScript.RageState.Enraged)
				{
					Plugin.Debug($"No longer enraged, breaking loop.");
					break;
				}

				counter++;
				float multi = Mathf.Pow(plugin.PunishMultiplier, counter);
				int dmg = Mathf.FloorToInt(plugin.PunishDamage * multi);
				Plugin.Debug(
					plugin.ExtremePunishement ? $"Punishing for {dmg}(extreme)" : $"Punishing for {plugin.PunishDamage}");
				rh.playerStats.HurtPlayer(
					new PlayerStats.HitInfo(plugin.ExtremePunishement ? dmg : plugin.PunishDamage, rh.nicknameSync.MyNick, DamageTypes.Contain,
						rh.queryProcessor.PlayerId), rh.gameObject);
				yield return Timing.WaitForSeconds(plugin.PunishDelay);
			}
		}
		
		private static string DrawBar(double percentage)
		{
			string bar = "<color=#ffffff>(</color>";
			const int barSize = 20;

			percentage *= 100;
			if (percentage == 0) return "(      )";

			for (double i = 0; i < 100; i += 100 / barSize)
				if (i < percentage)
					bar += "█";
				else
					bar += "<color=#c50000>█</color>";

			bar += "<color=#ffffff>)</color>";

			return bar;
		}
		
		public void OnEnrage(ref Scp096EnrageEvent ev)
		{
			Plugin.Info("SCP 096 ENRAGE EVENT");
			List<ReferenceHub> hubs = Plugin.GetHubs();

			foreach (ReferenceHub hub in hubs)
			{
				if (hub == ev.Player || plugin.BlacklistedRoles.Contains((int)hub.characterClassManager.CurClass) || !hub.characterClassManager.IsHuman())
					continue;
				
				Vector3 tarPos = hub.gameObject.transform.position;
				Vector3 scpPos = ev.Player.gameObject.transform.position;

				if (Vector3.Distance(tarPos, scpPos) > plugin.MaxRange)
				{
					Plugin.Info("SCP-096: Range too high, continuing..");
					continue;
				}

				if (Physics.Linecast(tarPos, scpPos, KWallMask))
				{
					Plugin.Info("Scp-096: Linecast true, continuing..");
					continue;
				}

				Vector3 scpFwd = ev.Player.GetComponent<Scp049PlayerScript>().plyCam.transform.forward;
				Vector3 tarFwd = hub.GetComponent<Scp049PlayerScript>().plyCam.transform.forward;

				float scpAngle = Vector3.Angle(scpFwd, (tarPos - scpPos).normalized);
				float tarAngle = Vector3.Angle(tarFwd, (scpPos - tarPos).normalized);

				if (tarAngle >= 42f || scpAngle >= 42f)
				{
					Plugin.Info("SCP-096: Angle too high, continuing..");
					continue;
				}

				if (!Plugin.Scp096Targets.Contains(hub.queryProcessor.PlayerId))
				{
					Plugin.Info($"SCP-096: Adding {hub.queryProcessor.PlayerId} to targets.");
					Plugin.Scp096Targets.Add(hub.queryProcessor.PlayerId);
				}

				hub.Broadcast(5, "You are a target for SCP-096!");
			}

			if (!Plugin.Scp096Targets.Any())
			{
				ev.Script._rageProgress -= ev.Script._rageProgress * 0.1f;
				ev.Allow = false;
				ev.Script.enraged = Scp096PlayerScript.RageState.NotEnraged;
				Plugin.Info("No targets, ending OnEnrage. SCP096 should not be enraged.");
				return;
			}

			if (plugin.EnragedBypass)
				ev.Player.serverRoles.BypassMode = true;
			TargetCount = Plugin.Scp096Targets.Count;
			scp096 = ev.Player;
			plugin.Coroutines.Add(Timing.RunCoroutine(GetClosestPlayer(ev.Script, ev.Player, hubs), "checkranges"));
			plugin.Coroutines.Add(Timing.RunCoroutine(Punish(ev.Script, ev.Player), "punish"));
		}

		public void OnCalm(ref Scp096CalmEvent ev)
		{
			if (Plugin.Scp096Targets.Any())
			{
				ev.Allow = false;
				return;
			}

			Timing.KillCoroutines("punish");
			Timing.KillCoroutines("checkranges");
			ev.Script._rageProgress = 0f;
			ev.Script.Networkenraged = Scp096PlayerScript.RageState.Cooldown;
			ReferenceHub scp = Plugin.GetPlayer(ev.Script.gameObject);
			scp.serverRoles.BypassMode = false;
			int healAmount = plugin.HealAmount * TargetCount;
			Timing.RunCoroutine(plugin.EventHandlers.HealOverTime(scp, healAmount, 10f), "heal096");
			Timing.RunCoroutine(ChangeCooldown(ev.Script, plugin.CooldownTime));
			scp096 = null;
		}

		private IEnumerator<float> ChangeCooldown(Scp096PlayerScript script, float cd)
		{
			yield return Timing.WaitForSeconds(0.5f);

			script._cooldown = cd;
		}
		
		public IEnumerator<float> HealOverTime(ReferenceHub rh, int amount, float time)
		{
			float amountPerTick = amount / time;
			float tracker = time;
			do
			{
				rh.playerStats.HealHPAmount(amountPerTick);
				yield return Timing.WaitForSeconds(2f);
			} while ((tracker -= 2f) > 0);
		}

		public void OnPlayerHurt(ref PlayerHurtEvent ev)
		{
			if (ev.Player.nicknameSync.MyNick == "Dedicated Server")
			{
				Plugin.Debug("HURT: Is server, returning.");
				return;
			}
			
			if (ev.Attacker == null)
			{
				Plugin.Debug("HURT: Attacker is null!");
				return;
			}
			
			if (ev.Player.characterClassManager.CurClass == RoleType.Scp096 && ev.Player != ev.Attacker && !Plugin.Scp096Targets.Contains(ev.Attacker.queryProcessor.PlayerId))
			{
				if (ev.Player.GetComponent<Scp096PlayerScript>().enraged == Scp096PlayerScript.RageState.Enraged)
					Plugin.Scp096Targets.Add(ev.Attacker.queryProcessor.PlayerId);
				else
					Timing.KillCoroutines("heal096");
			}
			
			if (ev.Player.characterClassManager.CurClass == RoleType.Scp096 && ev.Player.gameObject.GetComponent<Scp096PlayerScript>().Networkenraged == Scp096PlayerScript.RageState.Enraged)
				if (ev.Player != ev.Attacker)
					ev.Info = new PlayerStats.HitInfo(ev.Info.Amount * plugin.DamageResistance, ev.Info.Attacker, ev.Info.GetDamageType(), ev.Info.PlyId);
		}

		public void OnWaitingForPlayers()
		{
			Plugin.Scp096Targets.Clear();
		}

		public void OnRoundStart()
		{
			
		}

		public void OnRoundEnd()
		{
			Plugin.Scp096Targets.Clear();
			foreach (CoroutineHandle handle in plugin.Coroutines)
				Timing.KillCoroutines(handle);
		}
	}
}