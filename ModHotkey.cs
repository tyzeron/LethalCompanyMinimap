// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LethalCompanyMinimap
{
    public class ModHotkey
    {
        public Key DefaultKey { get; set; }
        public Key Key { get; set; }
        public bool KeyWasDown { get; set; }
        public bool IsSettingKey { get; set; }
        public Action OnKey { get; set; }

        public ModHotkey(Key defaultKey, Action onKey)
        {
            DefaultKey = defaultKey;
            Key = defaultKey;
            KeyWasDown = false;
            IsSettingKey = false;
            OnKey = onKey;
        }

        public void Update()
        {
            // Get Keyboard
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }
            KeyControl keyControl = keyboard[Key];

            // When key is pressed
            if (keyControl.wasPressedThisFrame && !KeyWasDown && !IsSettingKey)
            {
                KeyWasDown = true;
            }

            // When key is released
            if (keyControl.wasReleasedThisFrame && KeyWasDown)
            {
                KeyWasDown = false;
                OnKey?.Invoke();
            }
        }
    }

    public class HotkeyManager
    {
        public ModHotkey[] AllHotkeys;

        public HotkeyManager(int numberOfHotkeys)
        {
            AllHotkeys = new ModHotkey[numberOfHotkeys];
        }

        public bool AnyHotkeyIsSettingKey()
        {
            foreach (ModHotkey hotkey in AllHotkeys)
            {
                if (hotkey.IsSettingKey)
                {
                    return true;
                }
            }
            return false;
        }

        public bool SetHotKey(Key key)
        {
            foreach (ModHotkey hotkey in AllHotkeys)
            {
                if (hotkey.IsSettingKey)
                {
                    hotkey.Key = key;
                    ResetIsSettingKey();
                    return true;
                }
            }
            return false;
        }

        public void ResetIsSettingKey()
        {
            foreach (ModHotkey hotkey in AllHotkeys)
            {
                hotkey.IsSettingKey = false;
            }
        }

        public void ResetSettingKey()
        {
            foreach (ModHotkey hotkey in AllHotkeys)
            {
                hotkey.IsSettingKey = false;
            }
        }

        public void ResetToDefaultKey()
        {
            foreach (ModHotkey hotkey in AllHotkeys)
            {
                hotkey.Key = hotkey.DefaultKey;
            }
        }

        public void Update()
        {
            foreach (ModHotkey hotkey in AllHotkeys)
            {
                hotkey.Update();
            }
        }
    }
}
