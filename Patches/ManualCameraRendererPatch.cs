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

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void MapCameraAlwaysEnabledPatch(ref Camera ___mapCamera)
        {
            // Ensure that the Map camera is always being updated even when outside the ship
            if (___mapCamera != null)
            {
                ___mapCamera.enabled = true;
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
