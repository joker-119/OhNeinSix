using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using ServerMod2.API;
using Smod2.Events;
using UnityEngine;
using UnityEngine.Networking;
using RemoteAdmin;


namespace OhNeinSix
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
			ev.RageProgress = 1f;
			ev.Script = __instance;
			ev.EnrageState = (EnrageState)((int)b);
			EventManager.Manager.HandleEvent<IEventHandler096Enrage>(ev);
			__instance.rageProgress = ev.RageProgress;
			b = (Scp096PlayerScript.RageState)((int)ev.EnrageState);

			return ev.AllowEnrage;
		}
		
		internal static void PatchMethodsUsingHarmony()
		{
			HarmonyInstance harmony = HarmonyInstance.Create("com.joker119.096events");
			harmony.PatchAll();
		}
	}
}
