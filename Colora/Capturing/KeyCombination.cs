using System;
using System.Linq;
using System.Windows.Input;

namespace Colora.Capturing
{
    /// <summary>
    /// Represents a key combination out of modifiers and keys.
    /// </summary>
    [Serializable]
    public class KeyCombination
    {
        public ModifierKeys Modifiers { get; set; }
        public Key Key { get; set; }

        public bool Control
        {
            get => Modifiers.HasFlag(ModifierKeys.Control);
            set => setFlag(ModifierKeys.Control, value);
        }
        public bool Alt
        {
            get => Modifiers.HasFlag(ModifierKeys.Alt);
            set => setFlag(ModifierKeys.Alt, value);
        }
        public bool Shift
        {
            get => Modifiers.HasFlag(ModifierKeys.Shift);
            set => setFlag(ModifierKeys.Shift, value);
        }

        public KeyCombination()
        {
            Key = Key.None;
            Modifiers = ModifierKeys.None;
        }

        public KeyCombination(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        private void setFlag(ModifierKeys modifier, bool set)
        {
            Modifiers = set ? Modifiers | modifier : Modifiers & ~modifier;
        }

        public override string ToString()
        {
            var conv = new ModifierKeysConverter();
            var modifierString = conv.ConvertToString(Modifiers);
            return modifierString + "+" + Key.ToString();
        }
    }
}
