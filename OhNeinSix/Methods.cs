using Smod2.API;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using MEC;
using TargetedGhostmode;

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

		private static float Distance(Vector a, Vector b) => (a - b).Magnitude;

		public IEnumerable<int> AddTargets(Player ply)
		{
			List<Player> players = plugin.Server.GetPlayers();
			GameObject scp = (GameObject)ply.GetGameObject();
			Vector3 scpForward = scp.GetComponent<Scp049PlayerScript>().plyCam.transform.forward;
			List<int> targets = new List<int>();

			foreach (Player target in players)
			{

				if (target.PlayerId == ply.PlayerId) continue;
				
				if (plugin.BlacklistedRoleList.Contains((int) target.TeamRole.Role) ||
				    target.TeamRole.Team == Smod2.API.Team.SCP || target.GetRankName() == "SCP-035")
				{
					plugin.Info("Target " + target.Name + "'s role is blacklisted: " + target.TeamRole.Role);
					if (plugin.HideAllies && !(ply.IsHiddenFrom(target)))
						ply.HidePlayer(target);
					
					continue;
				}

				if (OhNeinSix.Targets.Contains(target.PlayerId))
				{
					plugin.Info("Target " + target.Name + " is already on target list.");
					continue;
				}
				
				if (Distance(ply.GetPosition(), target.GetPosition()) >= plugin.MaxRange)
				{
					if (!ply.IsHiddenFrom(target))
						ply.HidePlayer(target);
					continue;
				}

				if (plugin.PreviousTargets.Contains(target.PlayerId))
				{
					plugin.Info("Target " + target.Name + " has been added as a previous target.");
					targets.Add(target.PlayerId);
					if (ply.IsHiddenFrom(target))
						ply.ShowPlayer(target);
					continue;
				}

				GameObject tar = (GameObject) target.GetGameObject();
				Vector3 tarFwd = tar.GetComponent<Scp049PlayerScript>().plyCam.transform.forward;
				Vector3 tarPos = tar.transform.position;
				Vector3 scpPos = scp.transform.position;
				float tarAngle = Vector3.Angle(tarFwd, (scpPos - tarPos).normalized);
				float scpAngle = Vector3.Angle(scpForward, (tarPos - scpPos).normalized);

				if (target.PlayerId == ply.PlayerId || Physics.Linecast(tarPos, scpPos, KWallMask) ||
				    !(tarAngle <= 40) || !(scpAngle <= 40))
				{
					if (!ply.IsHiddenFrom(target))
						ply.HidePlayer(target);
					continue;
				}

				targets.Add(target.PlayerId);
				if (ply.IsHiddenFrom(target))
					ply.ShowPlayer(target);
				target.PersonalClearBroadcasts();
				target.PersonalBroadcast(5, "You are a target for <color=#FF0000> SCP-096 </color>!", false);
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
				Dictionary<Player, float> dlist = new Dictionary<Player, float>();

				foreach (int playerId in plugin.Functions.AddTargets(player))
					OhNeinSix.Targets.Add(playerId);

				foreach (Player tar in players.Where(p => OhNeinSix.Targets.Contains(p.PlayerId)))
				{
					plugin.Info(tar.Name + " is a target.");

					float distance = Distance(player.GetPosition(), tar.GetPosition());
					plugin.Info("Distance: " + distance + " Max: " + plugin.MaxRange);

					if (distance <= plugin.MaxRange)
						dlist.Add(tar, distance);
					else
					{
						OhNeinSix.Targets.Remove(tar.PlayerId);
						if (!player.IsHiddenFrom(tar))
							player.HidePlayer(tar);
						if (plugin.PersistantTargets)
							plugin.PreviousTargets.Add(tar.PlayerId);
					}
				}

				plugin.Info("Dist count: " + dlist.Count);
				if (dlist.Count < 1)
				{
					OhNeinSix.Raged.Remove(player.PlayerId);
					foreach (Player ply in plugin.Server.GetPlayers().Where(t => player.IsHiddenFrom(t)))
						player.ShowPlayer(ply);
					OhNeinSix.Targets.Clear();
					((GameObject)player.GetGameObject()).GetComponent<Scp096PlayerScript>().rageProgress = 0f;
					break;
				}

				KeyValuePair<Player, float> min = new KeyValuePair<Player, float>(player, 100f);
				foreach (KeyValuePair<Player, float> kvp in dlist)
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

		public IEnumerator<float> CheckInvisible(Player player)
		{
			GameObject ply = (GameObject) player.GetGameObject();

			while (true)
			{
				if (player.TeamRole.Role != Role.SCP_096)
					yield break;
				
				foreach (Player tar in plugin.Server.GetPlayers())
					if (ply.GetComponent<Scp096PlayerScript>().enraged != Scp096PlayerScript.RageState.Enraged)
						player.ShowPlayer(tar);
				
				yield return Timing.WaitForSeconds(0.75f);
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