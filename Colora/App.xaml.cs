using System;
using System.Windows;
using Colora.Helpers;
using Colora.Model;
using Colora.View;

namespace Colora
{
    public partial class App : Application
    {
        public const string UPDATE_URL = "https://colora.sourceforge.io/update.xml";

#if PORTABLE
        public const string UPDATE_MODE = "portable";
#else
        public const string UPDATE_MODE = "install";
#endif

        // App-wide methods

        public static void ConfigureShortcut(Window owner, ScreenPicker screenPicker)
        {
            // remove current shortcut during shortcut key selection
            var oldKeys = screenPicker.ShortcutKeys;
            screenPicker.ShortcutKeys = KeyCombination.None;
            HotKeyInputWindow hotKeyInput = new HotKeyInputWindow(oldKeys);
            hotKeyInput.Owner = owner;
            if (hotKeyInput.ShowDialog() == true)
            {
                screenPicker.ShortcutKeys = hotKeyInput.SelectedHotKey;
            }
            else
            {
                // restore previous shortcut
                screenPicker.ShortcutKeys = oldKeys;
            }
        }
    }
}
