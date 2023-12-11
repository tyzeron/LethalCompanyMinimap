// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LethalCompanyMinimap.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        public static readonly string prefix = $"<color=#00ffffff>[{MinimapMod.modName}]</color> ";
        public static readonly string prefixForBroadcast = "Tyzeron.Minimap";
        private static readonly Regex parserRegex = new Regex(@"\A<size=0>" + Regex.Escape(prefixForBroadcast) + @"/([ -~]+)/([ -~]+)/([ -~]+)</size>\z", RegexOptions.Compiled);
        private static IDictionary<string, string> myBroadcasts = new Dictionary<string, string>();
        private static string lastMessage = "";

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

        public static void SendMinimapBroadcast(string signature, string data = "null")
        {
            int clientId = (int)GameNetworkManager.Instance.localPlayerController.playerClientId;
            if (!myBroadcasts.ContainsKey(signature))
            {
                myBroadcasts.Add(signature, data);
            }
            else
            {
                myBroadcasts[signature] = data;
            }
            HUDManager.Instance.AddTextToChatOnServer($"<size=0>{prefixForBroadcast}/{clientId}/{signature}/{data}</size>");
        }

    }
}
