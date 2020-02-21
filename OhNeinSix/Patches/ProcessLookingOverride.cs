using Grenades;
using Harmony;
using UnityEngine;

namespace OhNeinSix.Patches
{
	[HarmonyPatch(typeof(Scp096PlayerScript), nameof(Scp096PlayerScript.ProcessLooking))]
	public class ProcessLookingOverride
	{
		public static bool Prefix(Scp096PlayerScript __instance)
		{
			if (__instance._processLookingQueue.IsEmpty())
			{
				foreach (GameObject player in PlayerManager.players)
					__instance._processLookingQueue.Enqueue(player);
			}
			else
			{
				GameObject player = __instance._processLookingQueue.Dequeue();
				if (player == null || !ReferenceHub.GetHub(player).characterClassManager.IsHuman() || player.GetComponent<FlashEffect>().blinded)
					return false;
				Transform transform = player.GetComponent<Scp096PlayerScript>().camera.transform;
				float num = __instance.lookingTolerance.Evaluate(Vector3.Distance(transform.position, __instance.camera.transform.position));
				Vector3 vector3;
				if (num >= 0.75)
				{
					Vector3 forward = transform.forward;
					vector3 = transform.position - __instance.camera.transform.position;
					Vector3 normalized = vector3.normalized;
					if (Vector3.Dot(forward, normalized) >= -(double) num)
						return false;
				}
				Vector3 position = transform.transform.position;
				vector3 = __instance.camera.transform.position - transform.position;
				Vector3 normalized1 = vector3.normalized;
				int layerMask = __instance.layerMask;
				if (!Physics.Raycast(position, normalized1, out RaycastHit raycastHit, (__instance.gameObject.transform.position.y > 500f ? Plugin.MaxTriggerRangeSurface : Plugin.MaxTriggerRange), layerMask) || (raycastHit.collider.gameObject.layer != 24 || !raycastHit.collider.GetComponentInParent<Scp096PlayerScript>() == __instance))
					return false;
				__instance.IncreaseRage(Time.fixedDeltaTime);
			}

			return false;
		}
	}
}