// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace LethalCompanyMinimap
{
    public class VersionChecker
    {
        public static string latestVersion = null;

        public static async Task GetLatestVersionAsync()
        {
            string url = $"https://api.github.com/repos/{MinimapMod.modRepository}/releases/latest";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Minimap Mod");
                try
                {
                    String response = await client.GetStringAsync(url);
                    latestVersion = ParseTagNameFromJson(response);
                }
                catch (HttpRequestException e)
                {
                    MinimapMod.mls.LogError($"Failed to get the latest version of {MinimapMod.modName} Mod :(");
                }
            }
        }

        private static string ParseTagNameFromJson(string jsonString)
        {
            string tagNameToken = "\"tag_name\":\"";
            int index = jsonString.IndexOf(tagNameToken);
            if (index != -1)
            {
                int startIndex = index + tagNameToken.Length;
                int endIndex = jsonString.IndexOf("\"", startIndex);
                if (endIndex != -1)
                {
                    string versionNumber = jsonString.Substring(startIndex, endIndex - startIndex);
                    return LStrip(versionNumber, 'v');
                }
            }
            return null;
        }

        private static string LStrip(string input, char charToStrip)
        {
            int startIndex = 0;
            while (startIndex < input.Length && input[startIndex] == charToStrip)
            {
                startIndex++;
            }

            return input.Substring(startIndex);
        }
    }
}
