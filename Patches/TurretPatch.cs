// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;
using UnityEngine;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(Turret))]
    internal class TurretPatch
    {

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void TurretVisibilityOnMapPatch(ref Transform ___turretRod)
        {
            // Toggle turret visibility based on user's Minimap settings
            if (___turretRod != null)
            {
                foreach (Transform child in ___turretRod.gameObject.transform)
                {
                    if (!child.name.StartsWith("Plane"))
                    {
                        continue;
                    }
                    GameObject turretRedCone = child.gameObject;
                    if (MinimapMod.minimapGUI.showTurrets != turretRedCone.activeSelf)
                    {
                        turretRedCone.SetActive(MinimapMod.minimapGUI.showTurrets);
                    }
                }
            }
        }

    }
}
