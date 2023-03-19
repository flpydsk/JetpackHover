//Copyright (c) 2021-Present 18107
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using PulsarModLoader.Keybinds;
using PulsarModLoader.Patches;
using UnityEngine;

namespace JetpackHover
{
	[HarmonyPatch(typeof(PLPlayerController), "HandleMovement")]
	internal class HandleMovement
	{
		private static void Prefix(PLPlayerController __instance, bool ___JetpackEnabled, ref float ___yVel)
		{
			HandleMovement.hoverSpeed = 0f;
			PLPawn myPawn = __instance.MyPawn;
			if (myPawn != null && myPawn.GetPlayer() != null && myPawn.GetPlayer().GetPlayerID() == PLNetworkManager.Instance.LocalPlayerID)
			{
                if (KeybindManager.Instance.GetButtonDown("jetpackhoverbind") && (HandleMovement.hover || !myPawn.MyCharacterController.isGrounded))
				{
					HandleMovement.hover = !HandleMovement.hover;
					PLServer.Instance.AddNotification("Hover " + (HandleMovement.hover ? "on" : "off"), PLNetworkManager.Instance.LocalPlayerID, PLServer.Instance.GetEstimatedServerMs() + 6000, false);
				}
				float num = Mathf.Min(__instance.JetpackFuel, Time.deltaTime * 0.6f);
				if (!___JetpackEnabled && !myPawn.MyCharacterController.isGrounded)
				{
					if (__instance.JetpackFuel < 0.01f)
					{
						myPawn.ShowJetpackVisuals = false;
						return;
					}
					if (HandleMovement.hover)
					{
						___yVel = 0f;
						__instance.JetpackFuel -= num * (myPawn.GetExosuitIsActive() ? 0.3f : 0.25f);
						myPawn.ShowJetpackVisuals = true;
						HandleMovement.hoverSpeed = 1f;
						return;
					}
					myPawn.ShowJetpackVisuals = false;
				}
				if (myPawn.MyCharacterController.isGrounded && HandleMovement.hover)
				{ 
					HandleMovement.hover = false;
                    PLServer.Instance.AddNotification("Hover off", PLNetworkManager.Instance.LocalPlayerID, PLServer.Instance.GetEstimatedServerMs() + 6000, false);
                }
			}
		}

		public static float GetJetpackSpeed(float moveSpeedIn)
		{
			return Math.Max(moveSpeedIn, HandleMovement.hoverSpeed);
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = new List<CodeInstruction>
			{
				new CodeInstruction(OpCodes.Stloc_S, null),
				new CodeInstruction(OpCodes.Ldloc_S, null),
				new CodeInstruction(OpCodes.Ldloc_S, null),
				new CodeInstruction(OpCodes.Ldarg_0, null),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLPlayerController), "SmoothJetbootAlpha"))
			};
			List<CodeInstruction> list2 = new List<CodeInstruction>
			{
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HandleMovement), "GetJetpackSpeed", null, null))
			};
			return HarmonyHelpers.PatchBySequence(instructions, list, list2, (HarmonyHelpers.PatchMode)1, (HarmonyHelpers.CheckMode)1, false);
		}

		public static float hoverSpeed;

		private static bool hover;
	}
}
