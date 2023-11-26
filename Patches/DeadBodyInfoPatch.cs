// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;
using UnityEngine;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(DeadBodyInfo))]
    internal class DeadBodyInfoPatch
    {

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void MapCameraAlwaysEnabledPatch(ref Transform ___radarDot)
        {
            // Toggle dead player visibility based on user's Minimap settings
            if (MinimapMod.minimapGUI.showDeadPlayers != ___radarDot.gameObject.activeSelf)
            {
                ___radarDot.gameObject.SetActive(MinimapMod.minimapGUI.showDeadPlayers);
            }
        }

    }
}
