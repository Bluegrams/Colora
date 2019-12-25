using System;
using System.Windows.Input;
using Colora.Helpers;

namespace Colora
{
    static class CustomCommands
    {
        public static readonly RoutedCommand PickFromScreen = new RoutedCommand("PickFromScreen",
            typeof(CustomCommands));

        public static readonly RoutedCommand ColorDialog = new RoutedCommand("ColorDialog",
            typeof(CustomCommands),
            new InputGestureCollection() { CreateGesture(Key.D, ModifierKeys.Control) });

        public static readonly RoutedCommand MinimalSize = new RoutedCommand("MinimalSize",
            typeof(CustomCommands),
            new InputGestureCollection() { CreateGesture(Key.M, ModifierKeys.Control) });

        public static readonly RoutedCommand PaletteEditor = new RoutedCommand("PaletteEditor",
            typeof(CustomCommands),
            new InputGestureCollection() { CreateGesture(Key.P, ModifierKeys.Control) });

        public static readonly RoutedCommand TopMost = new RoutedCommand("TopMost",
            typeof(CustomCommands),
            new InputGestureCollection() { CreateGesture(Key.T, ModifierKeys.Control) });

        public static readonly RoutedCommand PaletteEditColor = new RoutedCommand("PaletteEditColor",
            typeof(CustomCommands),
            new InputGestureCollection() { CreateGesture(Key.E, ModifierKeys.Control) });

        internal static KeyGesture CreateGesture(Key key, ModifierKeys modifiers)
        {
            return (KeyGesture)(new KeyCombination(key, modifiers));
        }
    }
}
