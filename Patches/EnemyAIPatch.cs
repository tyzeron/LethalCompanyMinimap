// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class EnemyAIPatch
    {

        //[HarmonyPatch(nameof(EnemyAI.Update))]
        //[HarmonyPostfix]
        //static void EnemyVisibilityOnMapPatch(EnemyAI __instance)
        //{
        //    // Toggle enemy visibility based on user's Minimap settings
        //    if (MinimapMod.minimapGUI.showEnemies != __instance.creatureAnimator.gameObject.activeSelf)
        //    {
        //        __instance.creatureAnimator.gameObject.SetActive(MinimapMod.minimapGUI.showEnemies);
        //    }
        //}

    }
}
