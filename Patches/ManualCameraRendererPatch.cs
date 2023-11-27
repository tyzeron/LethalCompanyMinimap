// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;
using UnityEngine;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(ManualCameraRenderer))]
    internal class ManualCameraRendererPatch
    {
        private static Vector3 defaultEulerAngles = new Vector3(90, 315, 0);

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void MapCameraAlwaysEnabledPatch(ref Camera ___mapCamera)
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

                // Sync the Minimap rotation with where player is facing if auto-rotate is on
                if (MinimapMod.minimapGUI.autoRotate)
                {
                    ___mapCamera.transform.eulerAngles = new Vector3(
                        defaultEulerAngles.x,
                        GameNetworkManager.Instance.localPlayerController.transform.eulerAngles.y,
                        defaultEulerAngles.z
                    );
                } else if (___mapCamera.transform.eulerAngles != defaultEulerAngles)
                {
                    ___mapCamera.transform.eulerAngles = defaultEulerAngles;
                }
            }
        }

        [HarmonyPatch("updateMapTarget")]
        [HarmonyPrefix]
        static bool RadarMapTargetPatch(int setRadarTargetIndex, ref int ___targetTransformIndex)
        {
            MinimapMod.minimapGUI.realPlayerIndex = setRadarTargetIndex;

            // We don't run updateMapTarget if freezePlayerIndex setting is True
            if (MinimapMod.minimapGUI.freezePlayerIndex == true)
            {
                MinimapMod.minimapGUI.SetMinimapTarget(MinimapMod.minimapGUI.playerIndex);
                return false;
            }
            MinimapMod.minimapGUI.playerIndex = ___targetTransformIndex;
            return true;
        }

    }
}
