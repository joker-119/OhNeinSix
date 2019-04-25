using System;
using Smod2;
using Smod2.API;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using MEC;
using ServerMod2.API;

namespace OhNeinSix
{
	public class Methods
	{
		private readonly OhNeinSix plugin;
		public Methods(OhNeinSix plugin) => this.plugin = plugin;

		public IEnumerator<float> Punish(Player player)
		{
			yield return Timing.WaitForSeconds(plugin.PunishDelay * 2);
			int counter = 0;

			while (OhNeinSix.Raged.Contains(player.PlayerId))
			{
				if (plugin.ExtremePunishment)
				{
					plugin.Info("Extreme Punishment.");
					float multi = Mathf.Pow(plugin.PunishMultiplier, counter);
					int dmg = Mathf.FloorToInt(plugin.PunishDamage * multi);
					player.Damage(dmg);
				}
				else
				{
					plugin.Info("Punishment.");
					player.Damage(plugin.PunishDamage);
				}
				counter++;

				yield return Timing.WaitForSeconds(plugin.PunishDelay);
			}
		}

		public float Distance(Vector a, Vector b)
		{
			return Vector.Distance(a, b);
		}

		public List<int> AddTargets(Player ply)
		{
			List<Player> players = plugin.Server.GetPlayers();
			GameObject scp = (GameObject)ply.GetGameObject();
			Vector3 scpForward = scp.GetComponent<Scp049PlayerScript>().plyCam.transform.forward;
			List<int> targets = new List<int>();

			foreach (Player player in players.Where(p => plugin.Functions.Distance(ply.GetPosition(), p.GetPosition()) <= 25f))
			{
				if (plugin.BlacklistedRoles.Contains((int)player.TeamRole.Role) || player.TeamRole.Team == Smod2.API.Team.SCP)
				{
					plugin.Info("Player " + player.Name + "'s role is blacklisted: " + player.TeamRole.Role);
					continue;
				}
				if (OhNeinSix.Targets.Contains(player.PlayerId))
				{
					plugin.Info("Player " + player.Name + " is already on target list.");
					continue;
				}

				GameObject tar = (GameObject)player.GetGameObject();
				Vector3 tarForward = tar.GetComponent<Scp049PlayerScript>().plyCam.transform.forward;

				float tarAngle = Vector3.Angle(tarForward, (scp.transform.position - tar.transform.position).normalized);
				float scpAngle = Vector3.Angle(scpForward, (tar.transform.position - scp.transform.position).normalized);

				plugin.Info("Target Angle " + tarAngle);
				plugin.Info("SCP Angle: " + scpAngle);
				plugin.Info("Linecast: " + (Physics.Linecast(player.GetPosition().ToVector3(), ply.GetPosition().ToVector3())).ToString());

				if (tarAngle <= 40 && scpAngle <= 40)
				{
					if (player.PlayerId != ply.PlayerId && !Physics.Linecast(player.GetPosition().ToVector3(), ply.GetPosition().ToVector3(), kWallMask))
					{
						targets.Add(player.PlayerId);
						player.PersonalClearBroadcasts();
						player.PersonalBroadcast(5, "You are a target for SCP-096!", false);
					}
				}
			}
			plugin.Info("Adding targets: " + targets);
			return targets;
		}

		private const int kWallMask =
		1 << 30 | // Lockers
		1 << 27 | // Door
		1 << 14 | // Glass
		1 << 9 | // Pickups
		1 << 0;   // Default


		public IEnumerator<float> GetClosestPlayer(Player player)
		{
			while (OhNeinSix.Raged.Contains(player.PlayerId))
			{
				List<Player> players = plugin.Server.GetPlayers();
				Dictionary<Player, float> dist = new Dictionary<Player, float>();

				foreach (int playerID in plugin.Functions.AddTargets(player))
					OhNeinSix.Targets.Add(playerID);

				foreach (Player tar in players.Where(pl => OhNeinSix.Targets.Contains(pl.PlayerId)))
				{
					plugin.Info(tar.Name + " is a target.");
					if (tar.TeamRole.Team == Smod2.API.Team.SCP)
					{
						OhNeinSix.Targets.Remove(tar.PlayerId);
						continue;
					}

					float distance = Distance(player.GetPosition(), tar.GetPosition());

					if (distance <= plugin.MaxRange)
					{
						dist.Add(tar, distance);
					}
					else
					{
						OhNeinSix.Targets.Remove(tar.PlayerId);
					}
				}

				if (dist.Count < 1)
				{
					OhNeinSix.Raged.Remove(player.PlayerId);
					OhNeinSix.Targets.Clear();
					((GameObject)player.GetGameObject()).GetComponent<Scp096PlayerScript>().rageProgress = 0f;
					break;
				}

				KeyValuePair<Player, float> min = new KeyValuePair<Player, float>(player, 100f);
				foreach (var kvp in dist)
				{
					plugin.Info("Kvp: " + kvp.Value + "Min: " + min.Value);
					if (kvp.Value < min.Value)
						min = kvp;
				}

				double value = ((plugin.MaxRange - min.Value) / plugin.MaxRange);
				string bar = DrawBar(value);

				player.PersonalClearBroadcasts();
				player.PersonalBroadcast(1, "<size=30><color=#c50000>Distance to nearest target: </color><color=#10F110>" + bar + "</color></size> \n" + "<size=25>Targets Remaining: <color=#c50000>" + OhNeinSix.Targets.Count + "</color></size>", false);

				yield return Timing.WaitForSeconds(0.5f);
			}
		}

		public string DrawBar(double percentage)
		{
			string bar = "<color=#ffffff>(</color>";
			const int BAR_SIZE = 20;

			percentage *= 100;
			if (percentage == 0) return "(      )";

			for (double i = 0; i < 100; i += (100 / BAR_SIZE))
			{
				if (i < percentage)
					bar += "█";
				else
					bar += "<color=#c50000>█</color>";
			}

			bar += "<color=#ffffff>)</color>";

			return bar;
		}
	}
}