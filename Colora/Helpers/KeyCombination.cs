using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace Colora.Helpers
{
    /// <summary>
    /// Represents a key combination out of modifiers and keys.
    /// </summary>
    [TypeConverter(typeof(KeyCombinationConverter))]
    public class KeyCombination
    {
        private static KeysConverter wFormsKeyConv = new KeysConverter();

        public static readonly KeyCombination None = new KeyCombination(Key.None, ModifierKeys.None);


        public ModifierKeys Modifiers { get; }
        public Key Key { get; }
        public string DisplayString { get; }

        public KeyCombination(Key key, ModifierKeys modifiers = ModifierKeys.None)
        {
            this.Key = key;
            this.Modifiers = modifiers;
            Keys formsKeys = ToWinFormsKeys(key, modifiers);
            this.DisplayString = wFormsKeyConv.ConvertToString(formsKeys);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is KeyCombination other))
                return false;
            return this.Key == other.Key && this.Modifiers == other.Modifiers;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() + Modifiers.GetHashCode();
        }

        public override string ToString() => this.DisplayString;

        public static explicit operator KeyCombination(KeyGesture gesture)
        {
            return new KeyCombination(gesture.Key, gesture.Modifiers);
        }

        public static explicit operator KeyGesture(KeyCombination combination)
        {
            return new KeyGesture(combination.Key, combination.Modifiers, combination.DisplayString);
        }

        internal static Keys ToWinFormsKeys(Key key, ModifierKeys modifiers)
        {
            Keys keys = (Keys)KeyInterop.VirtualKeyFromKey(key);
            if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                keys |= Keys.Alt;
            if ((modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                keys |= Keys.Control;
            if ((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                keys |= Keys.Shift;
            return keys;
        }
    }
}
