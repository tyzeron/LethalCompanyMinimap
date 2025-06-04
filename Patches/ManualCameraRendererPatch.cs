// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(ManualCameraRenderer))]
    internal class ManualCameraRendererPatch
    {
        private static Vector3 defaultEulerAngles = new Vector3(90, 315, 0);

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void MapCameraAlwaysEnabledPatch(
            ref Camera ___mapCamera, ref PlayerControllerB ___targetedPlayer, ref Image ___compassRose, //ref Light ___mapCameraLight,
            ref Transform ___shipArrowPointer, GameObject ___shipArrowUI)
        {
            if (___mapCamera != null)
            {
                // Ensure that the Map camera is always being updated even when outside the ship
                ___mapCamera.enabled = true;

                // Adjust the Minimap Zoom level based on user's Minimap settings
                if (___mapCamera.orthographicSize != MinimapMod.minimapGUI.minimapZoom)
                {
                    ___mapCamera.orthographicSize = MinimapMod.minimapGUI.minimapZoom;
                }

                // Sync the Minimap rotation with where the target player is facing if auto-rotate is on
                if (MinimapMod.minimapGUI.autoRotate && ___targetedPlayer != null && ___compassRose != null)
                {
                    ___mapCamera.transform.eulerAngles = new Vector3(
                        defaultEulerAngles.x,
                        ___targetedPlayer.transform.eulerAngles.y,
                        defaultEulerAngles.z
                    );
                    ___compassRose.enabled = false;
                    
                }
                else if (___mapCamera.transform.eulerAngles != defaultEulerAngles)
                {
                    ___mapCamera.transform.eulerAngles = defaultEulerAngles;
                }

                // Rotate Terminal Code labels based on the map orientation
                foreach (TerminalAccessibleObject terminalObject in Object.FindObjectsOfType<TerminalAccessibleObject>())
                {
                    FieldInfo mapRadarTextFieldInfo = terminalObject.GetType().GetField("mapRadarText", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (mapRadarTextFieldInfo != null)
                    {
                        TextMeshProUGUI mapRadarText = (TextMeshProUGUI)mapRadarTextFieldInfo.GetValue(terminalObject);
                        mapRadarText.transform.eulerAngles = new Vector3(
                            defaultEulerAngles.x,
                            ___mapCamera.transform.eulerAngles.y,
                            defaultEulerAngles.z
                        );
                    }
                }
            }

            /*if (___mapCameraLight != null && ___targetedPlayer != null)
            {
                // Ensure the map spotlight is always enabled
                ___mapCameraLight.enabled = ___targetedPlayer.isInsideFactory;

                // Hide the spotlight map from player's camera
                if (GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null)
                {
                    ___mapCameraLight.cullingMask = ~GameNetworkManager.Instance.localPlayerController.gameplayCamera.cullingMask;
                }
            }*/

            if (___shipArrowPointer != null && ___shipArrowUI != null && ___targetedPlayer != null)
            {
                // Handle and toggle the arrow pointing to the ship based on the user's mod settings
                if (MinimapMod.minimapGUI.showShipArrow && !___targetedPlayer.isInsideFactory
                    && Vector3.Distance(___targetedPlayer.transform.position, StartOfRound.Instance.elevatorTransform.transform.position) > 16f)
                {
                    if (StartOfRound.Instance != null)
                    {
                        ___shipArrowPointer.LookAt(StartOfRound.Instance.elevatorTransform);
                    }
                    int arrowOffset = MinimapMod.minimapGUI.autoRotate ? 45 : 0;
                    ___shipArrowPointer.eulerAngles = new Vector3(0f, ___shipArrowPointer.eulerAngles.y + arrowOffset, 0f);
                    ___shipArrowUI.SetActive(value: true);
                }
                else
                {
                    ___shipArrowUI.SetActive(value: false);
                }
            }
        }

        [HarmonyPatch("updateMapTarget")]
        [HarmonyPrefix]
        static bool RadarMapSwitchTargetPatch(int setRadarTargetIndex, ref int ___targetTransformIndex, ref IEnumerator __result)
        {
            MinimapMod.minimapGUI.realPlayerIndex = setRadarTargetIndex;

            // We don't run updateMapTarget if freezePlayerIndex setting is True
            if (MinimapMod.minimapGUI.freezePlayerIndex == true)
            {
                MinimapMod.minimapGUI.SetMinimapTarget(MinimapMod.minimapGUI.playerIndex);
                __result = DoNothingCoroutine();
                return false;
            }
            MinimapMod.minimapGUI.playerIndex = ___targetTransformIndex;
            return true;
        }

        private static IEnumerator DoNothingCoroutine()
        {
            yield break;
        }

        [HarmonyPatch(nameof(ManualCameraRenderer.RemoveTargetFromRadar))]
        [HarmonyPostfix]
        static void RemoveTargetFromMapPatch()
        {
            // We need to manually switch to next target because of our RadarMapSwitchTargetPatch
            if (MinimapMod.minimapGUI.freezePlayerIndex == true)
            {
                MinimapMod.minimapGUI.SwitchTarget();
            }
        }

        [HarmonyPatch(nameof(ManualCameraRenderer.SwitchRadarTargetForward))]
        [HarmonyPrefix]
        static bool DontSwitchTargetForwardPatch()
        {
            // Dont switch radar target if freezePlayerIndex setting is True
            if (MinimapMod.minimapGUI.freezePlayerIndex == true)
            {
                return false;
            }
            return true;
        }

    }
}
