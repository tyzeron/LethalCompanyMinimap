// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(RadarBoosterItem))]
    internal class RadarBoosterItemPatch
    {

        [HarmonyPatch(nameof(RadarBoosterItem.Update))]
        [HarmonyPostfix]
        static void RadarBoosterVisibilityPatch(RadarBoosterItem __instance)
        {
            // Toggle radar booster visibility based on user's Minimap settings
            if (MinimapMod.minimapGUI.showRadarBoosters != __instance.radarDot.activeSelf)
            {
                __instance.radarDot.SetActive(MinimapMod.minimapGUI.showRadarBoosters);
            }
        }

    }
}
