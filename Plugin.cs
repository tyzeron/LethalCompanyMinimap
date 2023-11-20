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
        public const string modVersion = "1.0.0";
        public const string modAuthor = "Tyzeron";

        public static KeyboardShortcut defaultGuiKey = new KeyboardShortcut(KeyCode.F1);
        public static KeyboardShortcut defaultToggleMinimapKey = new KeyboardShortcut(KeyCode.F2);
        public const int defaultMinimapSize = 200;
        public const float defaultXoffset = 0f;
        public const float defaultYoffset = 0f;

        private static ConfigEntry<KeyboardShortcut> guiKeyConfig;
        private static ConfigEntry<KeyboardShortcut> toggleMinimapKeyConfig;
        private static ConfigEntry<bool> enableMinimapConfig;
        private static ConfigEntry<int> minimapSizeConfig;
        private static ConfigEntry<float> minimapXPosConfig;
        private static ConfigEntry<float> minimapYPosConfig;
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

            // Patching stuff
            harmony.PatchAll(typeof(StartofRoundPatch));
            harmony.PatchAll(typeof(ManualCameraRendererPatch));
            harmony.PatchAll(typeof(QuickMenuManagerPatch));

            // Initialize Minimap Mod Menu GUI
            GameObject minimapGUIObject = new GameObject("MinimapGUI");
            Object.DontDestroyOnLoad(minimapGUIObject);
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
            enableMinimapConfig = Config.Bind("Basic Settings", "Enable Minimap", true, "Toggles visibility of your Minimap");
            minimapSizeConfig = Config.Bind("Basic Settings", "Minimap Size", defaultMinimapSize, "Adjusts the size of your Minimap");
            minimapXPosConfig = Config.Bind("Basic Settings", "X Offset", defaultXoffset, "Shifts the Minimap position horizontally");
            minimapYPosConfig = Config.Bind("Basic Settings", "Y Offset", defaultYoffset, "Shifts the Minimap position vertically");
            freezePlayerIndexConfig = Config.Bind("Advance Settings", "Override Ship Controls", false, "Disables the ability to change the Minimap focus through the ship control panel, allowing Minimap focus changes only through the mod menu");
        }

        public void SyncGUIFromConfigs()
        {
            minimapGUI.guiKey = guiKeyConfig.Value;
            minimapGUI.toggleMinimapKey = toggleMinimapKeyConfig.Value;
            minimapGUI.enableMinimap = enableMinimapConfig.Value;
            minimapGUI.minimapSize = minimapSizeConfig.Value;
            minimapGUI.minimapXPos = minimapXPosConfig.Value;
            minimapGUI.minimapYPos = minimapYPosConfig.Value;
            minimapGUI.freezePlayerIndex = freezePlayerIndexConfig.Value;
        }

        public void SyncConfigFromGUI()
        {
            guiKeyConfig.Value = minimapGUI.guiKey;
            toggleMinimapKeyConfig.Value = minimapGUI.toggleMinimapKey;
            enableMinimapConfig.Value = minimapGUI.enableMinimap;
            minimapSizeConfig.Value = minimapGUI.minimapSize;
            minimapXPosConfig.Value = minimapGUI.minimapXPos;
            minimapYPosConfig.Value = minimapGUI.minimapYPos;
            freezePlayerIndexConfig.Value = minimapGUI.freezePlayerIndex;
        }
    }
}
