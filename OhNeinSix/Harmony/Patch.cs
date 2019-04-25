using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2.EventHandlers;
using ServerMod2.API;
using Smod2.Events;
using Smod2;
using Smod2.API;
using System.Diagnostics;
using UnityEngine.Internal;
using UnityEngine;
using UnityEngine.Networking;
using RemoteAdmin;
using OhNeinSix;



namespace OhNeinSix.Patch
{
	[HarmonyPatch(typeof(Scp096PlayerScript))]
	[HarmonyPatch("SetRage")]
	[HarmonyPatch(new Type[] { typeof(Scp096PlayerScript.RageState) })]

	class Event
	{
		[HarmonyPrefix]

		static bool Prefix(Scp096PlayerScript __instance, ref Scp096PlayerScript.RageState b)
		{
			Player096EnrageEvent ev = new Player096EnrageEvent();
			ev.Player = new SmodPlayer(__instance.gameObject);
			ev.rageProgress = 1f;
			ev.Script = __instance;
			ev.enrageState = (EnrageState)((int)b);
			EventManager.Manager.HandleEvent<IEventHandler096Enrage>(ev);
			__instance.rageProgress = ev.rageProgress;
			b = (Scp096PlayerScript.RageState)((int)ev.enrageState);

			return ev.allowEnrage;
		}
	}


	[HarmonyPatch(typeof(PlayerPositionManager))]
	[HarmonyPatch("TransmitData")]

	class Ghost
	{

		[HarmonyPrefix]

		static bool Prefix(PlayerPositionManager __instance)
		{
			// if (OhNeinSix.Raged.Count <= 0) return true;

			List<PlayerPositionData> posData = new List<PlayerPositionData>();
			List<GameObject> players = PlayerManager.singleton.players.ToList();
			bool smGhostMode = ConfigFile.GetBool("sm_enable_ghostmode", false);

			foreach (GameObject player in players)
			{
				posData.Add(new PlayerPositionData(player));
			}

			__instance.ReceiveData(posData.ToArray());

			foreach (GameObject gameObject in players)
			{
				CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();

				if (smGhostMode && gameObject != __instance.gameObject && component.curClass >= 0)
				{
					for (int i = 0; i < posData.Count; i++)
					{
						if (players[i] != gameObject)
						{
							CharacterClassManager component2 = players[i].GetComponent<CharacterClassManager>();
							if (smGhostMode && component2.smGhostMode && component2.curClass >= 0 && component2.curClass != 2 && (component.curClass != 2 || (!component2.smVisibleToSpec && component.curClass == 2)) && (!component2.smVisibleWhenTalking || (component2.smVisibleWhenTalking && !component2.GetComponent<Radio>().NetworkisTransmitting)))
							{
								posData[i] = new PlayerPositionData
								{
									position = Vector3.up * 6000f,
									rotation = 0f,
									playerID = posData[i].playerID
								};
							}
						}
					}
				}
				if (component.curClass == 16 || component.curClass == 17)
				{
					List<PlayerPositionData> posData939 = new List<PlayerPositionData>(posData);

					for (int i = 0; i < posData939.Count; i++)
					{
						CharacterClassManager component2 = players[i].GetComponent<CharacterClassManager>();
						if (posData939[i].position.y < 800f && component2.klasy[component2.curClass].team != Team.SCP && component2.klasy[component2.curClass].team != Team.RIP && !players[i].GetComponent<Scp939_VisionController>().CanSee(component.GetComponent<Scp939PlayerScript>()))
						{
							posData939[i] = new PlayerPositionData
							{
								position = Vector3.up * 6000f,
								rotation = 0f,
								playerID = posData939[i].playerID
							};
						}
					}
					__instance.CallTargetTransmit(gameObject.GetComponent<NetworkIdentity>().connectionToClient, posData939.ToArray());
				}
				else if (component.curClass == 9 && OhNeinSix.Raged.Contains(component.GetComponent<QueryProcessor>().PlayerId))
				{
					List<PlayerPositionData> posData096 = new List<PlayerPositionData>(posData);

					for (int i = 0; i < posData096.Count; i++)
					{
						var tarClass = players[i].GetComponent<CharacterClassManager>().curClass;
						if (!OhNeinSix.Targets.Contains(players[i].GetComponent<QueryProcessor>().PlayerId)
							&& tarClass != 0 && tarClass != 3 && tarClass != 5 && tarClass != 7 && tarClass != 9 && tarClass != 10 && tarClass != 14 && tarClass != 16 && tarClass != 17)
						{
							posData096[i] = new PlayerPositionData
							{
								position = Vector3.up * 6000f,
								rotation = 0f,
								playerID = posData096[i].playerID
							};
						}
					}
					__instance.CallTargetTransmit(gameObject.GetComponent<NetworkIdentity>().connectionToClient, posData096.ToArray());
				}
				else
					__instance.CallTargetTransmit(gameObject.GetComponent<NetworkIdentity>().connectionToClient, posData.ToArray());
			}

			return false;
		}
		public static void PatchMethodsUsingHarmony()
		{
			var Harmony = HarmonyInstance.Create("com.joker119.096events");
			Harmony.UnpatchAll();
			Harmony.PatchAll();
		}
	}
}
