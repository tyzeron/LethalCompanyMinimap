// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using GameNetcodeStuff;
using HarmonyLib;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(ShipTeleporter))]
    internal class ShipTeleporterPatch
    {
        private static PlayerControllerB playerToBeamUp = null;

        [HarmonyPatch("beamUpPlayer")]
        [HarmonyPrefix]
        static void BeforeTeleportPatch()
        {
            // Focus map to the real player so the teleporter don't teleport the player the Minimap is locked on
            int realPlayerIndex = MinimapMod.minimapGUI.realPlayerIndex;
            if (StartOfRound.Instance.mapScreen.targetTransformIndex != realPlayerIndex)
            {
                realPlayerIndex = MinimapMod.minimapGUI.CalculateValidTargetIndex(realPlayerIndex);
                playerToBeamUp = StartOfRound.Instance.mapScreen.radarTargets[realPlayerIndex].transform.gameObject.GetComponent<PlayerControllerB>();
                StartOfRound.Instance.mapScreen.targetedPlayer = playerToBeamUp;
            }
        }

        [HarmonyPatch("SetPlayerTeleporterId")]
        [HarmonyPrefix]
        static void DuringTeleportPatch()
        {
            // Immediately switch back to the player the Minimap was locked to
            if (playerToBeamUp != null)
            {
                playerToBeamUp = null;
                MinimapMod.minimapGUI.SetMinimapTarget(MinimapMod.minimapGUI.playerIndex);
            }
        }

    }
}
