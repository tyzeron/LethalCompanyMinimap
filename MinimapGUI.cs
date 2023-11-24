// ----------------------------------------------------------------------
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

        public KeyboardShortcut guiKey;
        public KeyboardShortcut toggleMinimapKey;
        public bool enableMinimap;
        public int minimapSize;
        public float minimapXPos;
        public float minimapYPos;
        public bool showLoots;
        public bool showEnemies;
        public bool showPlayers;
        public bool showRadarBoosters;
        public bool showTerminalCodes;
        public bool freezePlayerIndex;

        private string[] navbarStr = { "Minimap", "Icons", "Select Target", "Keybinds" };
        private readonly KeyboardShortcut escapeKey = new KeyboardShortcut(KeyCode.Escape);
        private int navbarIndex = 0;
        public bool isGUIOpen = false;
        private bool guiKeyWasDown = false;
        private bool escKeyWasDown = false;
        private bool toggleMinimapKeyWasDown = false;
        public int playerIndex = 0;
        public int realPlayerIndex = 0;
        private bool lockPrefix = false;
        private bool isSettingGUIKey = false;
        private bool isSettingToggleMinimapKey = false;
        private bool wasSettingGUIKey = false;
        private bool wasSettingToggleMinimapKey = false;
        private int extraGUIHeight = 0;

        private GUIStyle menuStyle;
        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle tinyLabelStyle;
        private GUIStyle toggleStyle;

        private void Awake()
        {
            MinimapMod.mls.LogInfo("MinimapGUI loaded.");
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
            List<TransformAndName> players = StartOfRound.Instance.mapScreen.radarTargets;
            if (index < 0 || index >= players.Count)
            {
                return "n/a";
            }
            return players[index].name;
        }

        public void SetMinimapTarget(int targetTransformIndex, bool lockOn = true)
        {
            string prefix = "MONITORING";
            lockPrefix = false;
            if (lockOn)
            {
                prefix = "LOCKED";
                lockPrefix = true;
            }
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

        public void Update()
        {
            // When GUI key is pressed
            if (guiKey.IsDown() && !guiKeyWasDown)
            {
                guiKeyWasDown = true;
            }
            // When GUI key is released
            if (guiKey.IsUp() && guiKeyWasDown)
            {
                guiKeyWasDown = false;
                if (!isGUIOpen)
                {
                    isGUIOpen = true;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                }
                else if (wasSettingGUIKey)
                {
                    wasSettingGUIKey = false;
                }
                else
                {
                    isGUIOpen = false;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            // When Toggle Minimap key is pressed
            if (toggleMinimapKey.IsDown() && !toggleMinimapKeyWasDown && !isSettingToggleMinimapKey)
            {
                toggleMinimapKeyWasDown = true;
            }
            // When Toggle Minimap key is released
            if (toggleMinimapKey.IsUp() && toggleMinimapKeyWasDown)
            {
                toggleMinimapKeyWasDown = false;
                if (wasSettingToggleMinimapKey)
                {
                    wasSettingToggleMinimapKey = false;
                }
                else
                {
                    enableMinimap = !enableMinimap;
                }
            }
            // When ESC key is pressed
            if (escapeKey.IsDown())
            {
                escKeyWasDown = true;
                if (isGUIOpen)
                {
                    if (wasSettingGUIKey || wasSettingToggleMinimapKey)
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
                if (wasSettingGUIKey || wasSettingToggleMinimapKey)
                {
                    wasSettingGUIKey = false;
                    wasSettingToggleMinimapKey = false;
                }
            }
            // Sync Minimap Target with the rest
            if (!freezePlayerIndex && (lockPrefix || playerIndex != realPlayerIndex))
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
                        showRadarBoosters = GUI.Toggle(new Rect(guiCenterX, guiYpos + 210, ITEMWIDTH, 30), showRadarBoosters, "Show Radar Boosters", toggleStyle);
                        showTerminalCodes = GUI.Toggle(new Rect(guiCenterX, guiYpos + 250, ITEMWIDTH, 30), showTerminalCodes, "Show Terminal Codes", toggleStyle);
                        break;
                    case 2:
                        List<TransformAndName> players = StartOfRound.Instance.mapScreen.radarTargets;

                        GUI.Label(new Rect(guiCenterX, guiYpos + 90, ITEMWIDTH, 30), $"Selected Target: {GetPlayerNameAtIndex(playerIndex)}", labelStyle);
                        freezePlayerIndex = GUI.Toggle(new Rect(guiCenterX, guiYpos + 140, ITEMWIDTH, 30), freezePlayerIndex, "Override Ship Controls", toggleStyle);
                        int buttonCount = 0;
                        if (freezePlayerIndex)
                        {
                            float baseYpos = guiYpos + 180;
                            PlayerControllerB component;

                            for (int i = 0; i < players.Count; i++)
                            {
                                component = players[i].transform.gameObject.GetComponent<PlayerControllerB>();
                                if (component != null && !component.isPlayerControlled && !component.isPlayerDead)
                                {
                                    continue;
                                }
                                if (GUI.Button(new Rect(guiCenterX, baseYpos + (40 * buttonCount), ITEMWIDTH, 30), players[i].name))
                                {
                                    SetMinimapTarget(i);
                                }
                                buttonCount += 1;
                            }
                        }
                        extraGUIHeight = buttonCount > 7 ? (buttonCount - 7) * 40 : 0;
                        break;
                    case 3:
                        string guiKeyButtonLabel = isSettingGUIKey ? "Press a Key..." : $"Open Mod Menu: {guiKey}";
                        if (GUI.Button(new Rect(guiCenterX, guiYpos + 90, ITEMWIDTH, 30), guiKeyButtonLabel))
                        {
                            isSettingGUIKey = true;
                            wasSettingGUIKey = true;
                            isSettingToggleMinimapKey = false;
                            wasSettingToggleMinimapKey = false;
                        }
                        string toggleMinimapKeyButtonLabel = isSettingToggleMinimapKey ? "Press a Key..." : $"Toggle Minimap: {toggleMinimapKey}";
                        if (GUI.Button(new Rect(guiCenterX, guiYpos + 130, ITEMWIDTH, 30), toggleMinimapKeyButtonLabel))
                        {
                            isSettingGUIKey = false;
                            wasSettingGUIKey = false;
                            isSettingToggleMinimapKey = true;
                            wasSettingToggleMinimapKey = true;
                        }
                        if (GUI.Button(new Rect(guiCenterX, guiYpos + 200, ITEMWIDTH, 30), "Reset to Default Keybinds"))
                        {
                            guiKey = MinimapMod.defaultGuiKey;
                            toggleMinimapKey = MinimapMod.defaultToggleMinimapKey;
                        }

                        if (isSettingGUIKey || isSettingToggleMinimapKey)
                        {
                            Event e = Event.current;
                            if (e.isKey)
                            {
                                if (e.keyCode == KeyCode.Escape) { }
                                else if (isSettingGUIKey)
                                {
                                    guiKey = new KeyboardShortcut(e.keyCode);
                                }
                                else if (isSettingToggleMinimapKey)
                                {
                                    toggleMinimapKey = new KeyboardShortcut(e.keyCode);
                                }
                                isSettingGUIKey = false;
                                isSettingToggleMinimapKey = false;
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
