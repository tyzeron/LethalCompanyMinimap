﻿// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using BepInEx.Configuration;
using GameNetcodeStuff;
using System.Collections.Generic;
using UnityEngine;

namespace LethalCompanyMinimap.Component
{
    public class MinimapGUI : MonoBehaviour
    {
        private const int GUI_WIDTH = 500;
        private const int GUI_HEIGHT = 500;
        private const int ITEMWIDTH = 300;

        public ModHotkey guiKey = new ModHotkey(MinimapMod.defaultGuiKey, null);
        public ModHotkey toggleMinimapKey = new ModHotkey(MinimapMod.defaultToggleMinimapKey, null);
        public ModHotkey toggleOverrideKey = new ModHotkey(MinimapMod.defaultToggleOverrideKey, null);
        public ModHotkey switchTargetKey = new ModHotkey(MinimapMod.defaultSwitchTargetKey, null);
        public HotkeyManager hotkeyManager = new HotkeyManager(4);

        public bool enableMinimap;
        public int minimapSize;
        public float minimapXPos;
        public float minimapYPos;
        public bool showLoots;
        public bool showEnemies;
        public bool showPlayers;
        public bool showDeadPlayers;
        public bool showRadarBoosters;
        public bool showTerminalCodes;
        public bool freezePlayerIndex;

        private string[] navbarStr = { "Minimap", "Icons", "Select Target", "Keybinds" };
        private readonly KeyboardShortcut escapeKey = new KeyboardShortcut(KeyCode.Escape);
        private int navbarIndex = 0;
        public bool isGUIOpen = false;
        private bool escKeyWasDown = false;
        public int playerIndex = 0;
        public int realPlayerIndex = 0;
        private bool lockPrefix = false;
        string prefix = "MONITORING";
        private int extraGUIHeight = 0;
        private Vector2 scrollPos = Vector2.zero;
        private CursorLockMode lastCursorState = Cursor.lockState;

        private GUIStyle menuStyle;
        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle tinyLabelStyle;
        private GUIStyle toggleStyle;

        private void Awake()
        {
            MinimapMod.mls.LogInfo("MinimapGUI loaded.");

            guiKey.OnKey = ToggleGUI;
            toggleMinimapKey.OnKey = () => { enableMinimap = !enableMinimap; };
            toggleOverrideKey.OnKey = () => { freezePlayerIndex = !freezePlayerIndex; };
            switchTargetKey.OnKey = SwitchTarget;

            hotkeyManager.AllHotkeys[0] = guiKey;
            hotkeyManager.AllHotkeys[1] = toggleMinimapKey;
            hotkeyManager.AllHotkeys[2] = toggleOverrideKey;
            hotkeyManager.AllHotkeys[3] = switchTargetKey;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public string GetPlayerNameAtIndex(int index)
        {
            if (StartOfRound.Instance == null)
            {
                return "n/a";
            }
            List<TransformAndName> players = StartOfRound.Instance.mapScreen.radarTargets;
            if (index < 0 || index >= players.Count)
            {
                return "n/a";
            }
            return players[index].name;
        }

        public void SetMinimapTarget(int targetTransformIndex, bool lockOn = true)
        {
            playerIndex = targetTransformIndex;
            StartOfRound.Instance.mapScreen.targetTransformIndex = playerIndex;
            StartOfRound.Instance.mapScreen.targetedPlayer = StartOfRound.Instance.mapScreen.radarTargets[playerIndex].transform.gameObject.GetComponent<PlayerControllerB>();
            StartOfRound.Instance.mapScreenPlayerName.text = $"{prefix}: {StartOfRound.Instance.mapScreen.radarTargets[playerIndex].name}";
        }

        private void IntitializeMenu()
        {
            if (menuStyle == null)
            {
                menuStyle = new GUIStyle(GUI.skin.box);
                buttonStyle = new GUIStyle(GUI.skin.button);
                labelStyle = new GUIStyle(GUI.skin.label);
                tinyLabelStyle = new GUIStyle(GUI.skin.label);
                toggleStyle = new GUIStyle(GUI.skin.toggle);

                menuStyle.normal.textColor = Color.white;
                menuStyle.normal.background = MakeTex(2, 2, new Color(0.19f, 0.2f, 0.22f, .9f));
                menuStyle.fontSize = 30;
                menuStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

                buttonStyle.normal.textColor = Color.white;
                buttonStyle.fontSize = 18;

                labelStyle.normal.textColor = Color.white;
                labelStyle.normal.background = MakeTex(2, 2, new Color(0.19f, 0.2f, 0.22f, 0.0f));
                labelStyle.fontSize = 18;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

                tinyLabelStyle.normal.textColor = Color.white;
                tinyLabelStyle.normal.background = MakeTex(2, 2, new Color(0.19f, 0.2f, 0.22f, 0.0f));
                tinyLabelStyle.fontSize = 11;
                tinyLabelStyle.alignment = TextAnchor.MiddleCenter;
                tinyLabelStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

                toggleStyle.normal.textColor = Color.white;
                toggleStyle.fontSize = 18;
            }
        }

        public int CalculateValidTargetIndex(int setRadarTargetIndex)
        {
            // If setRadarTargetIndex is not valid, calculate the next valid one
            List<TransformAndName> radarTargets = StartOfRound.Instance.mapScreen.radarTargets;
            if (radarTargets.Count <= setRadarTargetIndex)
            {
                setRadarTargetIndex = radarTargets.Count - 1;
            }
            PlayerControllerB component;
            for (int i = 0; i < radarTargets.Count; i++)
            {
                if (radarTargets[setRadarTargetIndex] == null)
                {
                    setRadarTargetIndex = (setRadarTargetIndex + 1) % radarTargets.Count;
                    continue;
                }
                component = radarTargets[setRadarTargetIndex].transform.gameObject.GetComponent<PlayerControllerB>();
                if (!(component != null) || component.isPlayerControlled || component.isPlayerDead)
                {
                    break;
                }
                setRadarTargetIndex = (setRadarTargetIndex + 1) % radarTargets.Count;
            }
            return setRadarTargetIndex;
        }

        private void SwitchTarget()
        {
            List<TransformAndName> players = StartOfRound.Instance != null ? StartOfRound.Instance.mapScreen.radarTargets : new List<TransformAndName>();
            if (!freezePlayerIndex || players.Count < 1)
            {
                return;
            }
            int nextIndex = (playerIndex + 1) % players.Count;
            nextIndex = CalculateValidTargetIndex(nextIndex);
            SetMinimapTarget(nextIndex);
        }

        private void ToggleGUI()
        {
            if (!isGUIOpen)
            {
                isGUIOpen = true;
                Cursor.visible = true;
                lastCursorState = Cursor.lockState;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                isGUIOpen = false;
                Cursor.visible = false;
                Cursor.lockState = lastCursorState;
            }
        }

        public void Update()
        {
            // Manage hotkeys (handle key being pressed/released)
            hotkeyManager.Update();

            // When ESC key is pressed
            if (escapeKey.IsDown())
            {
                escKeyWasDown = true;
                if (isGUIOpen)
                {
                    if (hotkeyManager.AnyHotkeyWasSettingKey())
                    {
                        Cursor.lockState = CursorLockMode.Confined;
                    }
                    else
                    {
                        isGUIOpen = false;
                        Cursor.visible = false;
                    }
                }
            }

            // When ESC key is released
            if (escapeKey.IsUp() && escKeyWasDown)
            {
                escKeyWasDown = false;
                if (hotkeyManager.AnyHotkeyWasSettingKey())
                {
                    hotkeyManager.ResetWasSettingKey();
                }
            }

            // Update the text prefix
            if (freezePlayerIndex != lockPrefix)
            {
                if (StartOfRound.Instance != null)
                {
                    lockPrefix = freezePlayerIndex;
                    prefix = freezePlayerIndex ? "LOCKED" : "MONITORING";
                    StartOfRound.Instance.mapScreenPlayerName.text = $"{prefix}: {StartOfRound.Instance.mapScreen.radarTargets[playerIndex].name}";
                }
            }

            // Sync Minimap Target with the rest
            if (!freezePlayerIndex && (playerIndex != realPlayerIndex))
            {
                realPlayerIndex = CalculateValidTargetIndex(realPlayerIndex);
                SetMinimapTarget(realPlayerIndex, false);
            }
        }

        public void OnGUI()
        {
            if (menuStyle == null)
            {
                IntitializeMenu();
            }

            if (isGUIOpen)
            {
                float guiXpos = (Screen.width / 2) - (GUI_WIDTH / 2);
                float guiYpos = (Screen.height / 2) - (GUI_HEIGHT / 2);
                float guiCenterX = guiXpos + ((GUI_WIDTH / 2) - (ITEMWIDTH / 2));

                GUI.Box(new Rect(guiXpos, guiYpos, GUI_WIDTH, GUI_HEIGHT + extraGUIHeight), $"\n{MinimapMod.modName} Mod\n", menuStyle);
                GUI.Label(new Rect(guiCenterX, guiYpos + 60, ITEMWIDTH, 30), $"v{MinimapMod.modVersion}\t\t\tby {MinimapMod.modAuthor}", tinyLabelStyle);
                navbarIndex = GUI.Toolbar(new Rect(guiXpos, guiYpos - 30, GUI_WIDTH, 30), navbarIndex, navbarStr, buttonStyle);

                guiYpos += 20;

                switch (navbarIndex)
                {
                    case 0:
                        enableMinimap = GUI.Toggle(new Rect(guiCenterX, guiYpos + 90, ITEMWIDTH, 30), enableMinimap, "Toggle Minimap", toggleStyle);

                        GUI.Label(new Rect(guiCenterX, guiYpos + 130, ITEMWIDTH, 30), $"Minimap Size: {minimapSize}", labelStyle);
                        minimapSize = (int)GUI.HorizontalSlider(new Rect(guiCenterX, guiYpos + 160, ITEMWIDTH, 30), minimapSize, 0, 1000);

                        GUI.Label(new Rect(guiCenterX, guiYpos + 180, ITEMWIDTH, 30), $"X Offset: {minimapXPos}", labelStyle);
                        minimapXPos = GUI.HorizontalSlider(new Rect(guiCenterX, guiYpos + 210, ITEMWIDTH, 30), minimapXPos, -1000, 1000);

                        GUI.Label(new Rect(guiCenterX, guiYpos + 230, ITEMWIDTH, 30), $"Y Offset: {minimapYPos}", labelStyle);
                        minimapYPos = GUI.HorizontalSlider(new Rect(guiCenterX, guiYpos + 260, ITEMWIDTH, 30), minimapYPos, -1000, 1000);

                        if (GUI.Button(new Rect(guiCenterX, guiYpos + 300, ITEMWIDTH, 30), "Reset to Default Size"))
                        {
                            minimapSize = 200;
                        }
                        if (GUI.Button(new Rect(guiCenterX, guiYpos + 340, ITEMWIDTH, 30), "Reset to Default Position"))
                        {
                            minimapXPos = 0;
                            minimapYPos = 0;
                        }
                        extraGUIHeight = 0;
                        break;
                    case 1:
                        showLoots = GUI.Toggle(new Rect(guiCenterX, guiYpos + 90, ITEMWIDTH, 30), showLoots, "Show Loots", toggleStyle);
                        showEnemies = GUI.Toggle(new Rect(guiCenterX, guiYpos + 130, ITEMWIDTH, 30), showEnemies, "Show Enemies", toggleStyle);
                        showPlayers = GUI.Toggle(new Rect(guiCenterX, guiYpos + 170, ITEMWIDTH, 30), showPlayers, "Show Players", toggleStyle);
                        showDeadPlayers = GUI.Toggle(new Rect(guiCenterX, guiYpos + 210, ITEMWIDTH, 30), showDeadPlayers, "Show Dead Players", toggleStyle);
                        showRadarBoosters = GUI.Toggle(new Rect(guiCenterX, guiYpos + 250, ITEMWIDTH, 30), showRadarBoosters, "Show Radar Boosters", toggleStyle);
                        showTerminalCodes = GUI.Toggle(new Rect(guiCenterX, guiYpos + 290, ITEMWIDTH, 30), showTerminalCodes, "Show Terminal Codes", toggleStyle);
                        break;
                    case 2:
                        List<TransformAndName> players = StartOfRound.Instance != null ? StartOfRound.Instance.mapScreen.radarTargets : new List<TransformAndName>();

                        GUI.Label(new Rect(guiCenterX, guiYpos + 90, ITEMWIDTH, 30), $"Selected Target: {GetPlayerNameAtIndex(playerIndex)}", labelStyle);
                        freezePlayerIndex = GUI.Toggle(new Rect(guiCenterX, guiYpos + 140, ITEMWIDTH, 30), freezePlayerIndex, "Override Ship Controls", toggleStyle);
                        int buttonCount = 0;
                        if (freezePlayerIndex)
                        {
                            float baseYpos = guiYpos + 180;
                            PlayerControllerB component;
                            scrollPos = GUI.BeginScrollView(new Rect(guiCenterX, baseYpos, ITEMWIDTH, 300), scrollPos, new Rect(0, 0, ITEMWIDTH - 10, 40 * players.Count));
                            for (int i = 0; i < players.Count; i++)
                            {
                                component = players[i].transform.gameObject.GetComponent<PlayerControllerB>();
                                if (component != null && !component.isPlayerControlled && !component.isPlayerDead)
                                {
                                    continue;
                                }
                                if (GUI.Button(new Rect(0, (40 * buttonCount), ITEMWIDTH - 30, 30), players[i].name))
                                {
                                    SetMinimapTarget(i);
                                }
                                buttonCount += 1;
                            }
                            GUI.EndScrollView();
                        }
                        break;
                    case 3:
                        string guiKeyButtonLabel = guiKey.IsSettingKey ? "Press a Key..." : $"Open Mod Menu: {guiKey.Key}";
                        if (GUI.Button(new Rect(guiCenterX, guiYpos + 90, ITEMWIDTH, 30), guiKeyButtonLabel))
                        {
                            hotkeyManager.ResetSettingKey();
                            guiKey.IsSettingKey = true;
                            guiKey.WasSettingKey = true;
                        }
                        string toggleMinimapKeyButtonLabel = toggleMinimapKey.IsSettingKey ? "Press a Key..." : $"Toggle Minimap: {toggleMinimapKey.Key}";
                        if (GUI.Button(new Rect(guiCenterX, guiYpos + 130, ITEMWIDTH, 30), toggleMinimapKeyButtonLabel))
                        {
                            hotkeyManager.ResetSettingKey();
                            toggleMinimapKey.IsSettingKey = true;
                            toggleMinimapKey.WasSettingKey = true;
                        }
                        string toggleOverrideKeyButtonLabel = toggleOverrideKey.IsSettingKey ? "Press a Key..." : $"Override Ship Controls: {toggleOverrideKey.Key}";
                        if (GUI.Button(new Rect(guiCenterX, guiYpos + 170, ITEMWIDTH, 30), toggleOverrideKeyButtonLabel))
                        {
                            hotkeyManager.ResetSettingKey();
                            toggleOverrideKey.IsSettingKey = true;
                            toggleOverrideKey.WasSettingKey = true;
                        }
                        string switchTargetKeyButtonLabel = switchTargetKey.IsSettingKey ? "Press a Key..." : $"Switch Minimap Target: {switchTargetKey.Key}";
                        if (GUI.Button(new Rect(guiCenterX, guiYpos + 210, ITEMWIDTH, 30), switchTargetKeyButtonLabel))
                        {
                            hotkeyManager.ResetSettingKey();
                            switchTargetKey.IsSettingKey = true;
                            switchTargetKey.WasSettingKey = true;
                        }
                        if (GUI.Button(new Rect(guiCenterX, guiYpos + 280, ITEMWIDTH, 30), "Reset to Default Keybinds"))
                        {
                            hotkeyManager.ResetToDefaultKey();
                        }

                        if (hotkeyManager.AnyHotkeyIsSettingKey())
                        {
                            Event e = Event.current;
                            if (e.isKey)
                            {
                                if (e.keyCode == KeyCode.Escape) { }
                                else
                                {
                                    foreach (ModHotkey hotkey in hotkeyManager.AllHotkeys)
                                    {
                                        if (hotkey.IsSettingKey)
                                        {
                                            hotkey.Key = new KeyboardShortcut(e.keyCode);
                                            break;
                                        }
                                    }
                                }
                                hotkeyManager.ResetIsSettingKey();
                            }
                        }
                        extraGUIHeight = 0;
                        break;
                }

                MinimapMod.Instance.SyncConfigFromGUI();
            }
        }
    }
}
