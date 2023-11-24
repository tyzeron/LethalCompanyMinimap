// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;
using UnityEngine;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class EnemyAIPatch
    {

        private static GameObject FindMapDot(GameObject enemyObject)
        {
            foreach (Transform child in enemyObject.transform)
            {
                if (child.name.StartsWith("MapDot"))
                {
                    return child.gameObject;
                }
            }
            return null;
        }

        [HarmonyPatch(nameof(EnemyAI.Update))]
        [HarmonyPostfix]
        static void EnemyVisibilityOnMapPatch(EnemyAI __instance)
        {
            // Toggle enemy visibility based on user's Minimap settings
            if (__instance != null && __instance.gameObject != null)
            {
                GameObject mapDot = FindMapDot(__instance.gameObject);
                if (mapDot != null && MinimapMod.minimapGUI.showEnemies != mapDot.activeSelf)
                {
                    mapDot.SetActive(MinimapMod.minimapGUI.showEnemies);
                }
            }
        }

    }
}
