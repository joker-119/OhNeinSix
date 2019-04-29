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
					plugin.Debug("Extreme Punishment.");
					float multi = Mathf.Pow(plugin.PunishMultiplier, counter);
					int dmg = Mathf.FloorToInt(plugin.PunishDamage * multi);
					player.Damage(dmg);
				}
				else
				{
					plugin.Debug("Punishment.");
					player.Damage(plugin.PunishDamage);
				}
				counter++;

				yield return Timing.WaitForSeconds(plugin.PunishDelay);
			}
		}

		private static float Distance(Vector a, Vector b) => Vector.Distance(a, b);

		public IEnumerable<int> AddTargets(Player ply)
		{
			List<Player> players = plugin.Server.GetPlayers();
			GameObject scp = (GameObject)ply.GetGameObject();
			Vector3 scpForward = scp.GetComponent<Scp049PlayerScript>().plyCam.transform.forward;
			List<int> targets = new List<int>();

			foreach (Player player in players.Where(p => Distance(ply.GetPosition(), p.GetPosition()) <= 25f))
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

				Vector3 tarPos = tar.transform.position;
				Vector3 scpPos = scp.transform.position;
				
				float tarAngle = Vector3.Angle(tarForward, (scpPos - tarPos).normalized);
				float scpAngle = Vector3.Angle(scpForward, (tarPos - scpPos).normalized);

				plugin.Debug("Target Angle " + tarAngle);
				plugin.Debug("SCP Angle: " + scpAngle);
				plugin.Debug("Linecast: " + Physics.Linecast(player.GetPosition().ToVector3(), ply.GetPosition().ToVector3()));

				if (!(tarAngle <= 40) || !(scpAngle <= 40)) continue;
				if (player.PlayerId == ply.PlayerId || Physics.Linecast(player.GetPosition().ToVector3(),
					    ply.GetPosition().ToVector3(), KWallMask)) continue;
				
				targets.Add(player.PlayerId);
				player.PersonalClearBroadcasts();
				player.PersonalBroadcast(5, "You are a target for SCP-096!", false);
			}
			plugin.Debug("Adding targets: " + targets);
			return targets;
		}

		private const int KWallMask =
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

				foreach (int playerId in plugin.Functions.AddTargets(player))
					OhNeinSix.Targets.Add(playerId);

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
						dist.Add(tar, distance);
					else
						OhNeinSix.Targets.Remove(tar.PlayerId);
				}

				if (dist.Count < 1)
				{
					OhNeinSix.Raged.Remove(player.PlayerId);
					OhNeinSix.Targets.Clear();
					((GameObject)player.GetGameObject()).GetComponent<Scp096PlayerScript>().rageProgress = 0f;
					break;
				}

				KeyValuePair<Player, float> min = new KeyValuePair<Player, float>(player, 100f);
				foreach (KeyValuePair<Player, float> kvp in dist)
				{
					plugin.Debug("Kvp: " + kvp.Value + "Min: " + min.Value);
					if (kvp.Value < min.Value)
						min = kvp;
				}

				double value = (plugin.MaxRange - min.Value) / plugin.MaxRange;
				string bar = DrawBar(value);

				player.PersonalClearBroadcasts();
				player.PersonalBroadcast(1, "<size=30><color=#c50000>Distance to nearest target: </color><color=#10F110>" + bar + "</color></size> \n" + "<size=25>Targets Remaining: <color=#c50000>" + OhNeinSix.Targets.Count + "</color></size>", false);

				yield return Timing.WaitForSeconds(0.5f);
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
	}
}