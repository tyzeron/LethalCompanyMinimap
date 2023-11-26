// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using BepInEx.Configuration;
using DunGen;
using System;

namespace LethalCompanyMinimap
{
    public class ModHotkey
    {
        public KeyboardShortcut DefaultKey { get; set; }
        public KeyboardShortcut Key { get; set; }
        public bool KeyWasDown { get; set; }
        public bool IsSettingKey { get; set; }
        public bool WasSettingKey { get; set; }
        public Action OnKey { get; set; }

        public ModHotkey(KeyboardShortcut defaultKey, Action onKey)
        {
            DefaultKey = defaultKey;
            Key = defaultKey;
            KeyWasDown = false;
            IsSettingKey = false;
            WasSettingKey = false;
            OnKey = onKey;
        }

        public void Update()
        {
            // When key is pressed
            if (Key.IsDown() && !KeyWasDown && !IsSettingKey)
            {
                KeyWasDown = true;
            }

            // When key is released
            if (Key.IsUp() && KeyWasDown)
            {
                KeyWasDown = false;
                if (WasSettingKey)
                {
                    WasSettingKey = false;
                }
                else
                {
                    OnKey?.Invoke();
                }
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

        public bool AnyHotkeyWasSettingKey()
        {
            foreach (ModHotkey hotkey in AllHotkeys)
            {
                if (hotkey.WasSettingKey)
                {
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

        public void ResetWasSettingKey()
        {
            foreach (ModHotkey hotkey in AllHotkeys)
            {
                hotkey.WasSettingKey = false;
            }
        }

        public void ResetSettingKey()
        {
            foreach (ModHotkey hotkey in AllHotkeys)
            {
                hotkey.IsSettingKey = false;
                hotkey.WasSettingKey = false;
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
