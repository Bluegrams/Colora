using System;
using System.Windows.Input;

namespace Colora
{
    static class CustomCommands
    {
        public static readonly RoutedCommand ColorDialog = new RoutedCommand("ColorDialog",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.D, ModifierKeys.Control) });

        public static readonly RoutedCommand MinimalSize = new RoutedCommand("MinimalSize",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.M, ModifierKeys.Control) });

        public static readonly RoutedCommand PaletteEditor = new RoutedCommand("PaletteEditor",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.P, ModifierKeys.Control) });

        public static readonly RoutedCommand TopMost = new RoutedCommand("TopMost",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.T, ModifierKeys.Control) });
    }
}
