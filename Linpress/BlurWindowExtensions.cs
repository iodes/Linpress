using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Linpress
{
    static class BlurWindowExtensions
    {
        static class Interop
        {
            [DllImport("user32.dll")]
            internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttribData data);

            [StructLayout(LayoutKind.Sequential)]
            internal struct WindowCompositionAttribData
            {
                public WindowCompositionAttribute Attribute;
                public IntPtr Data;
                public int SizeOfData;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct AccentPolicy
            {
                public AccentState AccentState;
                public AccentFlags AccentFlags;
                public int GradientColor;
                public int AnimationId;
            }

            [Flags]
            internal enum AccentFlags
            {
                // ...
                DrawLeftBorder = 0x20,
                DrawTopBorder = 0x40,
                DrawRightBorder = 0x80,
                DrawBottomBorder = 0x100,
                DrawAllBorders = (DrawLeftBorder | DrawTopBorder | DrawRightBorder | DrawBottomBorder)
                // ...
            }

            internal enum WindowCompositionAttribute
            {
                // ...
                WCA_ACCENT_POLICY = 19
                // ...
            }

            internal enum AccentState
            {
                ACCENT_DISABLED = 0,
                ACCENT_ENABLE_GRADIENT = 1,
                ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
                ACCENT_ENABLE_BLURBEHIND = 3,
                ACCENT_INVALID_STATE = 4
            }
        }

        public static void EnableBlur(this Window window)
        {
            if (SystemParameters.HighContrast)
            {
                return; // Blur is not useful in high contrast mode
            }

            SetAccentPolicy(window, Interop.AccentState.ACCENT_ENABLE_BLURBEHIND);
        }

        public static void DisableBlur(this Window window)
        {
            SetAccentPolicy(window, Interop.AccentState.ACCENT_DISABLED);
        }

        private static void SetAccentPolicy(Window window, Interop.AccentState accentState)
        {
            var windowHelper = new WindowInteropHelper(window);

            var accent = new Interop.AccentPolicy();
            accent.AccentState = accentState;
            accent.AccentFlags = Interop.AccentFlags.DrawAllBorders;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new Interop.WindowCompositionAttribData();
            data.Attribute = Interop.WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            Interop.SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
    }
}
