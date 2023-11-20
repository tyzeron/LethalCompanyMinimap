// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(QuickMenuManager))]
    internal class QuickMenuManagerPatch
    {
        private static bool isRealMenuOpen = false;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void FreezeWhenOpenedGUIPatch(ref bool ___isMenuOpen)
        {
            // Ensure that we cannot look around when our GUI is opened or when the game QuickMenu is opened
            bool expectedIsMenuOpen = MinimapMod.minimapGUI.isGUIOpen || isRealMenuOpen;
            if (___isMenuOpen != expectedIsMenuOpen)
            {
                ___isMenuOpen = expectedIsMenuOpen;
            }
        }

        [HarmonyPatch(nameof(QuickMenuManager.OpenQuickMenu))]
        [HarmonyPostfix]
        static void OpenQuickMenuPatch()
        {
            isRealMenuOpen = true;
        }

        [HarmonyPatch(nameof(QuickMenuManager.CloseQuickMenu))]
        [HarmonyPostfix]
        static void CloseQuickMenuPatch()
        {
            isRealMenuOpen = false;
        }

    }
}
