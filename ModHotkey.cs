// ----------------------------------------------------------------------
// Copyright (c) Tyzeron. All Rights Reserved.
// Licensed under the GNU Affero General Public License, Version 3
// ----------------------------------------------------------------------

using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LethalCompanyMinimap
{
    public enum MouseAndKeyboard
    {
        MouseLeft = -1,
        MouseMiddle = -2,
        MouseRight = -3,
        MouseForward = -4,
        MouseBack = -5,
        None = Key.None,
        Space = Key.Space,
        Enter = Key.Enter,
        Tab = Key.Tab,
        Backquote = Key.Backquote,
        Quote = Key.Quote,
        Semicolon = Key.Semicolon,
        Comma = Key.Comma,
        Period = Key.Period,
        Slash = Key.Slash,
        Backslash = Key.Backslash,
        LeftBracket = Key.LeftBracket,
        RightBracket = Key.RightBracket,
        Minus = Key.Minus,
        Equals = Key.Equals,
        A = Key.A,
        B = Key.B,
        C = Key.C,
        D = Key.D,
        E = Key.E,
        F = Key.F,
        G = Key.G,
        H = Key.H,
        I = Key.I,
        J = Key.J,
        K = Key.K,
        L = Key.L,
        M = Key.M,
        N = Key.N,
        O = Key.O,
        P = Key.P,
        Q = Key.Q,
        R = Key.R,
        S = Key.S,
        T = Key.T,
        U = Key.U,
        V = Key.V,
        W = Key.W,
        X = Key.X,
        Y = Key.Y,
        Z = Key.Z,
        Digit1 = Key.Digit1,
        Digit2 = Key.Digit2,
        Digit3 = Key.Digit3,
        Digit4 = Key.Digit4,
        Digit5 = Key.Digit5,
        Digit6 = Key.Digit6,
        Digit7 = Key.Digit7,
        Digit8 = Key.Digit8,
        Digit9 = Key.Digit9,
        Digit0 = Key.Digit0,
        LeftShift = Key.LeftShift,
        RightShift = Key.RightShift,
        LeftAlt = Key.LeftAlt,
        RightAlt = Key.RightAlt,
        AltGr = Key.AltGr,
        LeftCtrl = Key.LeftCtrl,
        RightCtrl = Key.RightCtrl,
        LeftMeta = Key.LeftMeta,
        RightMeta = Key.RightMeta,
        LeftWindows = Key.LeftWindows,
        RightWindows = Key.RightWindows,
        LeftApple = Key.LeftApple,
        RightApple = Key.RightApple,
        LeftCommand = Key.LeftCommand,
        RightCommand = Key.RightCommand,
        ContextMenu = Key.ContextMenu,
        Escape = Key.Escape,
        LeftArrow = Key.LeftArrow,
        RightArrow = Key.RightArrow,
        UpArrow = Key.UpArrow,
        DownArrow = Key.DownArrow,
        Backspace = Key.Backspace,
        PageDown = Key.PageDown,
        PageUp = Key.PageUp,
        Home = Key.Home,
        End = Key.End,
        Insert = Key.Insert,
        Delete = Key.Delete,
        CapsLock = Key.CapsLock,
        NumLock = Key.NumLock,
        PrintScreen = Key.PrintScreen,
        ScrollLock = Key.ScrollLock,
        Pause = Key.Pause,
        NumpadEnter = Key.NumpadEnter,
        NumpadDivide = Key.NumpadDivide,
        NumpadMultiply = Key.NumpadMultiply,
        NumpadPlus = Key.NumpadPlus,
        NumpadMinus = Key.NumpadMinus,
        NumpadPeriod = Key.NumpadPeriod,
        NumpadEquals = Key.NumpadEquals,
        Numpad0 = Key.Numpad0,
        Numpad1 = Key.Numpad1,
        Numpad2 = Key.Numpad2,
        Numpad3 = Key.Numpad3,
        Numpad4 = Key.Numpad4,
        Numpad5 = Key.Numpad5,
        Numpad6 = Key.Numpad6,
        Numpad7 = Key.Numpad7,
        Numpad8 = Key.Numpad8,
        Numpad9 = Key.Numpad9,
        F1 = Key.F1,
        F2 = Key.F2,
        F3 = Key.F3,
        F4 = Key.F4,
        F5 = Key.F5,
        F6 = Key.F6,
        F7 = Key.F7,
        F8 = Key.F8,
        F9 = Key.F9,
        F10 = Key.F10,
        F11 = Key.F11,
        F12 = Key.F12,
        OEM1 = Key.OEM1,
        OEM2 = Key.OEM2,
        OEM3 = Key.OEM3,
        OEM4 = Key.OEM4,
        OEM5 = Key.OEM5,
        IMESelected = Key.IMESelected
    }

    public class ModHotkey
    {
        public MouseAndKeyboard DefaultKey { get; set; }
        public MouseAndKeyboard Key { get; set; }
        public bool KeyWasDown { get; set; }
        public bool IsSettingKey { get; set; }
        public Action OnKey { get; set; }

        public ModHotkey(MouseAndKeyboard defaultKey, Action onKey)
        {
            DefaultKey = defaultKey;
            Key = defaultKey;
            KeyWasDown = false;
            IsSettingKey = false;
            OnKey = onKey;
        }

        public void Update()
        {
            // Get Mouse and Keyboard
            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;
            if (mouse == null || keyboard == null)
            {
                return;
            }

            // Determine the ButtonControl
            ButtonControl buttonControl;
            switch (Key)
            {
                case MouseAndKeyboard.MouseLeft:
                    buttonControl = mouse.leftButton;
                    break;
                case MouseAndKeyboard.MouseMiddle:
                    buttonControl = mouse.middleButton;
                    break;
                case MouseAndKeyboard.MouseRight:
                    buttonControl = mouse.rightButton;
                    break;
                case MouseAndKeyboard.MouseForward:
                    buttonControl = mouse.forwardButton;
                    break;
                case MouseAndKeyboard.MouseBack:
                    buttonControl = mouse.backButton;
                    break;
                default:
                    buttonControl = keyboard[(Key)Key];
                    break;
            }

            // When key is pressed
            if (buttonControl.wasPressedThisFrame && !KeyWasDown && !IsSettingKey)
            {
                KeyWasDown = true;
            }

            // When key is released
            if (buttonControl.wasReleasedThisFrame && KeyWasDown)
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

        private static MouseAndKeyboard ConvertToExtendedKey(Key key)
        {
            foreach (MouseAndKeyboard extKey in Enum.GetValues(typeof(MouseAndKeyboard)))
            {
                if (Enum.GetName(typeof(Key), key) == Enum.GetName(typeof(MouseAndKeyboard), extKey))
                {
                    return extKey;
                }
            }
            return MouseAndKeyboard.None;
        }

        public bool SetHotKey(Key key)
        {
            MouseAndKeyboard extendedKey = ConvertToExtendedKey(key);
            return SetHotKey(extendedKey);
        }

        public bool SetHotKey(MouseAndKeyboard key)
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
