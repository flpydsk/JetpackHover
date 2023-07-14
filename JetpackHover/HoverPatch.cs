/*
MIT License

Copyright (c) 18107

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
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
	internal class HoverPatch
	{
		private static void Prefix(PLPlayerController __instance, bool ___JetpackEnabled, ref float ___yVel)
		{
			hoverSpeed = 0f;
			PLPawn myPawn = __instance.MyPawn;
			if (myPawn != null && myPawn.GetPlayer() != null && myPawn.GetPlayer().GetPlayerID() == PLNetworkManager.Instance.LocalPlayerID)
			{
				//push to hover
                if (Command.JetpackHoverModeIsPush.Value && KeybindManager.Instance.GetButtonDown("jetpackhoverbind") && !myPawn.MyCharacterController.isGrounded)
				{
					hover = !hover;
					PLServer.Instance.AddNotification("Hover " + (hover ? "on" : "off"), PLNetworkManager.Instance.LocalPlayerID, PLServer.Instance.GetEstimatedServerMs() + 6000, false);
				}

				//hold to hover
				if (!Command.JetpackHoverModeIsPush.Value && !myPawn.MyCharacterController.isGrounded)
				{
					hover = KeybindManager.Instance.GetButton("jetpackhoverbind");
                }

                float num = Mathf.Min(__instance.JetpackFuel, Time.deltaTime * 0.6f);
				if (!___JetpackEnabled && !myPawn.MyCharacterController.isGrounded)
				{
					if (__instance.JetpackFuel < 0.01f)
					{
						myPawn.ShowJetpackVisuals = false;
						return;
					}
					if (hover)
					{
						___yVel = 0f;
						__instance.JetpackFuel -= num * (myPawn.GetExosuitIsActive() ? 0.3f : 0.25f);
						myPawn.ShowJetpackVisuals = true;
						hoverSpeed = 1f;
						return;
					}
					myPawn.ShowJetpackVisuals = false;
				}

				//turn hover off if grounded
				if (hover && myPawn.MyCharacterController.isGrounded)
				{ 
					hover = false;
                    PLServer.Instance.AddNotification("Hover off", PLNetworkManager.Instance.LocalPlayerID, PLServer.Instance.GetEstimatedServerMs() + 6000, false);
                }
			}
		}

		public static float GetJetpackSpeed(float moveSpeedIn)
		{
			return Math.Max(moveSpeedIn, hoverSpeed);
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> Match = new List<CodeInstruction>
			{
				new CodeInstruction(OpCodes.Stloc_S, null),
				new CodeInstruction(OpCodes.Ldloc_S, null),
				new CodeInstruction(OpCodes.Ldloc_S, null),
				new CodeInstruction(OpCodes.Ldarg_0, null),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLPlayerController), "SmoothJetbootAlpha"))
			};
			List<CodeInstruction> Replace = new List<CodeInstruction>
			{
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HoverPatch), "GetJetpackSpeed", null, null))
			};
			return HarmonyHelpers.PatchBySequence(instructions, Match, Replace, HarmonyHelpers.PatchMode.AFTER, HarmonyHelpers.CheckMode.NONNULL, false);
		}
        public static float hoverSpeed;

		private static bool hover;
	}
}
