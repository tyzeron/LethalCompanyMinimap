// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using GameNetcodeStuff;
using HarmonyLib;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlayerVisibilityOnMapPatch(PlayerControllerB __instance)
        {
            // Toggle player visibility based on user's Minimap settings
            if (MinimapMod.minimapGUI.showPlayers != __instance.mapRadarDotAnimator.gameObject.activeSelf)
            {
                __instance.mapRadarDotAnimator.gameObject.SetActive(MinimapMod.minimapGUI.showPlayers);
            }
        }

    }
}
