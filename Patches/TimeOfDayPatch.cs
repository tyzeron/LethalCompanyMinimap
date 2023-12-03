// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    internal class TimeOfDayPatch
    {

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void MinimapBrightnessPatch()
        {
            // Light up dark corridors on Minimap (using the sun)
            if (GameNetworkManager.Instance != null
                && GameNetworkManager.Instance.localPlayerController != null
                && GameNetworkManager.Instance.localPlayerController.isInsideFactory
                && TimeOfDay.Instance.sunDirect != null)
            {
                TimeOfDay.Instance.sunDirect.enabled = true;
                HDAdditionalLightData additionalLightData = TimeOfDay.Instance.sunDirect.GetComponent<HDAdditionalLightData>();
                if (additionalLightData != null)
                {
                    additionalLightData.lightDimmer = MinimapMod.minimapGUI.brightness;
                }
            }
        }

    }
}
