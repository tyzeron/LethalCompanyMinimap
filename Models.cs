// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using System.Collections.Generic;

namespace LethalCompanyMinimap
{
    public class ModUser
    {
        public string Version { get; set; }
        public bool Comply { get; set; }

        public ModUser(string version = null, bool comply = false)
        {
            this.Version = version;
            this.Comply = comply;
        }
    }

    public enum SyncSettingsAction
    {
        AlwaysAsk,
        Allow,
        Deny
    }

    public enum SettingAsHostAction
    {
        DontSync,
        SyncIcons
    }

    public class HostSettingsToSync
    {
        public bool ShowLoots { get; set; }
        public bool ShowEnemies { get; set; }
        public bool ShowLivePlayers { get; set; }
        public bool ShowDeadPlayers { get; set; }
        public bool ShowRadarBoosters { get; set; }
        public bool ShowTerminalCodes { get; set; }

        public HostSettingsToSync()
        {
            ShowLoots = true;
            ShowEnemies = true;
            ShowLivePlayers = true;
            ShowDeadPlayers = true;
            ShowRadarBoosters = true;
            ShowTerminalCodes = true;
        }

        public void Sync()
        {
            MinimapMod.minimapGUI.showLoots = ShowLoots;
            MinimapMod.minimapGUI.showEnemies = ShowEnemies;
            MinimapMod.minimapGUI.showLivePlayers = ShowLivePlayers;
            MinimapMod.minimapGUI.showDeadPlayers = ShowDeadPlayers;
            MinimapMod.minimapGUI.showRadarBoosters = ShowRadarBoosters;
            MinimapMod.minimapGUI.showTerminalCodes = ShowTerminalCodes;
        }

        public bool IsSync()
        {
            return (
                MinimapMod.minimapGUI.showLoots == ShowLoots
                && MinimapMod.minimapGUI.showEnemies == ShowEnemies
                && MinimapMod.minimapGUI.showLivePlayers == ShowLivePlayers
                && MinimapMod.minimapGUI.showDeadPlayers == ShowDeadPlayers
                && MinimapMod.minimapGUI.showRadarBoosters == ShowRadarBoosters
                && MinimapMod.minimapGUI.showTerminalCodes == ShowTerminalCodes
            );
        }

        public static HostSettingsToSync Parse(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            string[] parsedSettings = data.Split(';');
            if (parsedSettings.Length != 6)
            {
                return null;
            }
            HostSettingsToSync hostSettings = new HostSettingsToSync();
            HashSet<string> expectedSettings = new HashSet<string>
            {
                "loots",
                "enemies",
                "livePlayers",
                "deadPlayers",
                "radarBoosters",
                "terminalCodes"
            };
            foreach (string setting in parsedSettings)
            {
                string[] parsedSetting = setting.Split('=');
                if (parsedSetting.Length != 2)
                {
                    return null;
                }
                string rawSettingKey = parsedSetting[0].Trim();
                string rawSettingValue = parsedSetting[1].Trim();
                if (!expectedSettings.Remove(rawSettingKey))
                {
                    return null;  // Key is either not expected or duplicated
                }
                bool settingValue;
                if (rawSettingValue == "show")
                {
                    settingValue = true;
                }
                else if (rawSettingValue == "hide")
                {
                    settingValue= false;
                }
                else
                {
                    return null;
                }
                switch (rawSettingKey)
                {
                    case "loots":
                        hostSettings.ShowLoots = settingValue;
                        break;
                    case "enemies":
                        hostSettings.ShowEnemies = settingValue;
                        break;
                    case "livePlayers":
                        hostSettings.ShowLivePlayers = settingValue;
                        break;
                    case "deadPlayers":
                        hostSettings.ShowDeadPlayers = settingValue;
                        break;
                    case "radarBoosters":
                        hostSettings.ShowRadarBoosters = settingValue;
                        break;
                    case "terminalCodes":
                        hostSettings.ShowTerminalCodes = settingValue;
                        break;
                    default:
                        return null;
                }
            }
            if (expectedSettings.Count > 0)
            {
                return null;
            }
            return hostSettings;
        }
    }
}
