// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using GameNetcodeStuff;
using HarmonyLib;
using System;
using UnityEngine.UI;
using UnityEngine;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        private const int padding = -5;
        private static RawImage minimap;
        private static RectTransform tooltips;
        private static Vector2 tooltipsOriginalPos;

        [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
        [HarmonyPostfix]
        static void DisplayMinimapPatch()
        {
            // Check for updates
            string message = null;
            if (VersionChecker.latestVersion == null)
            {
                message = "<color=red>Failed to check for latest version</color>";
            }
            else if (new Version(MinimapMod.modVersion) < new Version(VersionChecker.latestVersion))
            {
                message = $"<color=white>There is a new version available: </color><color=green>{VersionChecker.latestVersion}</color>";
            }
            if (message != null)
            {
                HUDManagerPatch.SendClientMessage(message);
            }

            // Get the Minimap size from the settings
            int size = MinimapMod.minimapGUI.minimapSize;

            if (minimap == null)
            {
                // Create the Minimap object
                minimap = new GameObject("minimap").AddComponent<RawImage>();

                // Position and resize the Minimap based on config, default (anchor): top right
                minimap.rectTransform.anchorMin = new Vector2(1, 1);
                minimap.rectTransform.anchorMax = new Vector2(1, 1);
                minimap.rectTransform.pivot = new Vector2(1f, 1f);
                minimap.rectTransform.sizeDelta = new Vector2(size, size);
                minimap.rectTransform.anchoredPosition = new Vector2(
                    MinimapMod.minimapGUI.minimapXPos, MinimapMod.minimapGUI.minimapYPos + padding
                );
            }

            // Assign the Map Screen RenderTexture to it
            minimap.texture = StartOfRound.Instance.mapScreen.cam.targetTexture;

            // Add the Minimap to Player Screen GameObject
            minimap.transform.SetParent(HUDManager.Instance.playerScreenTexture.transform, false);

            // Move the Tooltips below the Minimap
            tooltips = HUDManager.Instance.Tooltips.canvasGroup.gameObject.GetComponent<RectTransform>();
            if (tooltipsOriginalPos == null)
            {
                tooltipsOriginalPos = tooltips.anchoredPosition;
            }
            tooltips.anchoredPosition -= new Vector2(0, size);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdateMinimapPatch(PlayerControllerB __instance)
        {
            // Toggle player visibility based on user's Minimap settings
            if (MinimapMod.minimapGUI.showPlayers != __instance.mapRadarDotAnimator.gameObject.activeSelf)
            {
                __instance.mapRadarDotAnimator.gameObject.SetActive(MinimapMod.minimapGUI.showPlayers);
            }

            // Minimap stuff
            if (minimap != null)
            {
                // Toggle Minimap visibility
                if (MinimapMod.minimapGUI.enableMinimap != minimap.gameObject.activeSelf)
                {
                    minimap.gameObject.SetActive(MinimapMod.minimapGUI.enableMinimap);
                    if (MinimapMod.minimapGUI.enableMinimap)
                    {
                        tooltips.anchoredPosition -= new Vector2(0, MinimapMod.minimapGUI.minimapSize);
                    }
                    else
                    {
                        tooltips.anchoredPosition = tooltipsOriginalPos;
                    }
                }
                // Resize Minimap
                if (MinimapMod.minimapGUI.minimapSize != minimap.rectTransform.sizeDelta.y)
                {
                    int size = MinimapMod.minimapGUI.minimapSize;
                    minimap.rectTransform.sizeDelta = new Vector2(size, size);
                    tooltips.anchoredPosition = tooltipsOriginalPos - new Vector2(0, size);
                }
                // Move Minimap
                if (MinimapMod.minimapGUI.minimapXPos != minimap.rectTransform.anchoredPosition.x
                    || MinimapMod.minimapGUI.minimapYPos != minimap.rectTransform.anchoredPosition.y)
                {
                    minimap.rectTransform.anchoredPosition = new Vector2(
                        MinimapMod.minimapGUI.minimapXPos, MinimapMod.minimapGUI.minimapYPos + padding
                    );
                }
                // Rotate Minimap
                //minimap.rectTransform.rotation = Quaternion.Euler(0, 0, -StartOfRound.Instance.mapScreen.targetedPlayer.turnCompass.eulerAngles.y % 360);
            }
        }

    }
}
