// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    internal class TimeOfDayPatch
    {

        [HarmonyPatch(nameof(TimeOfDay.SetInsideLightingDimness))]
        [HarmonyPostfix]
        static void LightUpMinimapIndoorsPatch()
        {
            // Light up dark corridors on Minimap (using the sun)
            if (GameNetworkManager.Instance.localPlayerController.isInsideFactory)
            {
                TimeOfDay.Instance.sunDirect.enabled = true;
            }
        }

    }
}
