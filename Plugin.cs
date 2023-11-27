// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalCompanyMinimap.Component;
using LethalCompanyMinimap.Patches;
using UnityEngine;

namespace LethalCompanyMinimap
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class MinimapMod: BaseUnityPlugin
    {
        public const string modGUID = "LethalCompanyMinimap";
        public const string modName = "Minimap";
        public const string modVersion = "1.0.4";
        public const string modAuthor = "Tyzeron";
        public const string modRepository = "tyzeron/LethalCompanyMinimap";

        public static KeyboardShortcut defaultGuiKey = new KeyboardShortcut(KeyCode.F1);
        public static KeyboardShortcut defaultToggleMinimapKey = new KeyboardShortcut(KeyCode.F2);
        public static KeyboardShortcut defaultToggleOverrideKey = new KeyboardShortcut(KeyCode.F3);
        public static KeyboardShortcut defaultSwitchTargetKey = new KeyboardShortcut(KeyCode.F4);
        public const int defaultMinimapSize = 200;
        public const float defaultXoffset = 0f;
        public const float defaultYoffset = 0f;
        public const float defaultMapZoom = 19.7f;

        private static ConfigEntry<KeyboardShortcut> guiKeyConfig;
        private static ConfigEntry<KeyboardShortcut> toggleMinimapKeyConfig;
        private static ConfigEntry<KeyboardShortcut> toggleOverrideKeyConfig;
        private static ConfigEntry<KeyboardShortcut> switchTargetKeyConfig;
        private static ConfigEntry<bool> enableMinimapConfig;
        private static ConfigEntry<int> minimapSizeConfig;
        private static ConfigEntry<float> minimapXPosConfig;
        private static ConfigEntry<float> minimapYPosConfig;
        private static ConfigEntry<bool> showLootsConfig;
        private static ConfigEntry<bool> showEnemiesConfig;
        private static ConfigEntry<bool> showPlayersConfig;
        private static ConfigEntry<bool> showDeadPlayersConfig;
        private static ConfigEntry<bool> showRadarBoostersConfig;
        private static ConfigEntry<bool> showTerminalCodesConfig;
        private static ConfigEntry<bool> freezePlayerIndexConfig;

        public static MinimapMod Instance;
        public static ManualLogSource mls;
        private readonly Harmony harmony = new Harmony(modGUID);
        public static MinimapGUI minimapGUI;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo($"{modName} {modVersion} loaded!");

            // Start the Task of getting the latest version
            _ = VersionChecker.GetLatestVersionAsync();

            // Patching stuff
            harmony.PatchAll(typeof(ManualCameraRendererPatch));
            harmony.PatchAll(typeof(QuickMenuManagerPatch));
            harmony.PatchAll(typeof(ShipTeleporterPatch));
            harmony.PatchAll(typeof(GrabbableObjectPatch));
            harmony.PatchAll(typeof(TerminalAccessibleObjectPatch));
            harmony.PatchAll(typeof(EnemyAIPatch));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(RadarBoosterItemPatch));
            harmony.PatchAll(typeof(HUDManagerPatch));
            harmony.PatchAll(typeof(TimeOfDayPatch));
            harmony.PatchAll(typeof(DeadBodyInfoPatch));

            // Initialize Minimap Mod Menu GUI
            GameObject minimapGUIObject = new GameObject("MinimapGUI");
            DontDestroyOnLoad(minimapGUIObject);
            minimapGUIObject.hideFlags = HideFlags.HideAndDontSave;
            minimapGUIObject.AddComponent<MinimapGUI>();
            minimapGUI = (MinimapGUI)minimapGUIObject.GetComponent("MinimapGUI");

            // Sync Configuration file and Mod GUI
            SetBindings();
            SyncGUIFromConfigs();
        }

        private void SetBindings()
        {
            guiKeyConfig = Config.Bind("Hotkeys", "Open Mod Menu", defaultGuiKey, "Hotkey to open the Minimap mod menu");
            toggleMinimapKeyConfig = Config.Bind("Hotkeys", "Toggle Minimap", defaultToggleMinimapKey, "Hotkey to toggle the visibility of your Minimap");
            toggleOverrideKeyConfig = Config.Bind("Hotkeys", "Toggle Override Ship", defaultToggleOverrideKey, "Hotkey to toggle the override ship controls");
            switchTargetKeyConfig = Config.Bind("Hotkeys", "Switch Minimap Focus", defaultSwitchTargetKey, "Hotkey to switch the Minimap focus");
            enableMinimapConfig = Config.Bind("Basic Settings", "Enable Minimap", true, "Toggles visibility of your Minimap");
            minimapSizeConfig = Config.Bind("Basic Settings", "Minimap Size", defaultMinimapSize, "Adjusts the size of your Minimap");
            minimapXPosConfig = Config.Bind("Basic Settings", "X Offset", defaultXoffset, "Shifts the Minimap position horizontally");
            minimapYPosConfig = Config.Bind("Basic Settings", "Y Offset", defaultYoffset, "Shifts the Minimap position vertically");
            minimapZoomConfig = Config.Bind("Basic Settings", "Map Zoom", defaultMapZoom, "Adjust the Map zoom level");
            showLootsConfig = Config.Bind("Minimap Icons", "Show Loots", true, "Toggles visibility of loots (small triangles) on your Minimap");
            showEnemiesConfig = Config.Bind("Minimap Icons", "Show Enemies", true, "Toggles visibility of enemies (red circles) on your Minimap");
            showPlayersConfig = Config.Bind("Minimap Icons", "Show Players", true, "Toggles visibility of players (cyan circles) on your Minimap");
            showDeadPlayersConfig = Config.Bind("Minimap Icons", "Show Dead Players", true, "Toggles visibility of dead players (greyed-out cyan circles) on your Minimap");
            showRadarBoostersConfig = Config.Bind("Minimap Icons", "Show Radar Boosters", true, "Toggles visibility of radar boosters (blue circles) on your Minimap");
            showTerminalCodesConfig = Config.Bind("Minimap Icons", "Show Terminal Codes", true, "Toggles visibility of terminal codes on your Minimap");
            freezePlayerIndexConfig = Config.Bind("Advance Settings", "Override Ship Controls", false, "Disables the ability to change the Minimap focus through the ship control panel, allowing Minimap focus changes only through the mod menu");
        }

        public void SyncGUIFromConfigs()
        {
            minimapGUI.guiKey.Key = guiKeyConfig.Value;
            minimapGUI.toggleMinimapKey.Key = toggleMinimapKeyConfig.Value;
            minimapGUI.toggleOverrideKey.Key = toggleOverrideKeyConfig.Value;
            minimapGUI.switchTargetKey.Key = switchTargetKeyConfig.Value;
            minimapGUI.enableMinimap = enableMinimapConfig.Value;
            minimapGUI.minimapSize = minimapSizeConfig.Value;
            minimapGUI.minimapXPos = minimapXPosConfig.Value;
            minimapGUI.minimapYPos = minimapYPosConfig.Value;
            minimapGUI.minimapZoom = minimapZoomConfig.Value;
            minimapGUI.showLoots = showLootsConfig.Value;
            minimapGUI.showEnemies = showEnemiesConfig.Value;
            minimapGUI.showPlayers = showPlayersConfig.Value;
            minimapGUI.showDeadPlayers = showDeadPlayersConfig.Value;
            minimapGUI.showRadarBoosters = showRadarBoostersConfig.Value;
            minimapGUI.showTerminalCodes = showTerminalCodesConfig.Value;
            minimapGUI.freezePlayerIndex = freezePlayerIndexConfig.Value;
        }

        public void SyncConfigFromGUI()
        {
            guiKeyConfig.Value = minimapGUI.guiKey.Key;
            toggleMinimapKeyConfig.Value = minimapGUI.toggleMinimapKey.Key;
            toggleOverrideKeyConfig.Value = minimapGUI.toggleOverrideKey.Key;
            switchTargetKeyConfig.Value = minimapGUI.switchTargetKey.Key;
            enableMinimapConfig.Value = minimapGUI.enableMinimap;
            minimapSizeConfig.Value = minimapGUI.minimapSize;
            minimapXPosConfig.Value = minimapGUI.minimapXPos;
            minimapYPosConfig.Value = minimapGUI.minimapYPos;
            minimapZoomConfig.Value = minimapGUI.minimapZoom;
            showLootsConfig.Value = minimapGUI.showLoots;
            showEnemiesConfig.Value = minimapGUI.showEnemies;
            showPlayersConfig.Value = minimapGUI.showPlayers;
            showDeadPlayersConfig.Value = minimapGUI.showDeadPlayers;
            showRadarBoostersConfig.Value = minimapGUI.showRadarBoosters;
            showTerminalCodesConfig.Value = minimapGUI.showTerminalCodes;
            freezePlayerIndexConfig.Value = minimapGUI.freezePlayerIndex;
        }
    }
}
