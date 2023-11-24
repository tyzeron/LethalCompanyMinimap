// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using HarmonyLib;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        public static readonly string prefix = $"<color=#00ffffff>[{MinimapMod.modName}]</color> ";

        public static void SendClientMessage(string message)
        {
            HUDManager.Instance.AddTextToChatOnServer($"{prefix}{message}");
        }

        [HarmonyPatch("AddTextMessageServerRpc")]
        [HarmonyPrefix]
        static bool DontSendMinimapMessagesPatch(string chatMessage)
        {
            if (chatMessage.StartsWith(prefix))
            {
                return false;
            }
            return true;
        }

    }
}
