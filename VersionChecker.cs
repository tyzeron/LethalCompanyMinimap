// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;

namespace LethalCompanyMinimap
{
    public class VersionChecker
    {
        public static string latestVersion = null;

        private static async Task<bool> GetVersionFromUrlAsync(string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", BuildUserAgentString());
                try
                {
                    string response = await client.GetStringAsync(url);
                    latestVersion = ParseTagNameFromJson(response);
                    return true;
                }
                catch (Exception e)
                {
                    MinimapMod.mls.LogError($"Failed to get the latest version from {url} for {MinimapMod.modName} Mod. Error: {e.Message}");
                    return false;
                }
            }
        }

        public static async Task GetLatestVersionAsync()
        {
            string mainUrl = "http://lethalminimap.tyzeron.com";
            string fallbackUrl = $"https://api.github.com/repos/{MinimapMod.modRepository}/releases/latest";
            if (!await GetVersionFromUrlAsync(mainUrl))
            {
                await GetVersionFromUrlAsync(fallbackUrl);
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

        private static string BuildUserAgentString()
        {
            string osInfo = GetOperatingSystemInfo();
            string architecture = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            return $"{MinimapMod.modGUID}/{MinimapMod.modVersion} ({osInfo}; {architecture})";
        }

        private static string GetOperatingSystemInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"Windows NT {Environment.OSVersion.Version}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return $"Mac OS X {Environment.OSVersion.Version}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return $"Linux {Environment.OSVersion.Version}";
            }
            else
            {
                return $"Unknown OS {Environment.OSVersion.Version}";
            }
        }
    }
}
