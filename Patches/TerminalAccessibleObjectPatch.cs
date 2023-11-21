// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;
using TMPro;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(TerminalAccessibleObject))]
    internal class TerminalAccessibleObjectPatch
    {

        [HarmonyPatch(nameof(TerminalAccessibleObject.InitializeValues))]
        [HarmonyPostfix]
        static void TerminalCodeVisibilityPatch(ref TextMeshProUGUI ___mapRadarText)
        {
            // Terminal code visibility based on user's Minimap settings
            ___mapRadarText.gameObject.SetActive(MinimapMod.minimapGUI.showTerminalCodes);
        }

    }
}
