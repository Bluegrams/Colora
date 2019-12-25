using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace Colora.Helpers
{
    /// <summary>
    /// Converter class for converting between string and KeyCombination.
    /// </summary>
    public class KeyCombinationConverter : TypeConverter
    {
        private static KeyConverter keyConv = new KeyConverter();
        private static ModifierKeysConverter modifiersConv = new ModifierKeysConverter();

        private const char DELIMITER = '+';

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != null && value is string source)
            {
                if (String.IsNullOrWhiteSpace(source))
                    return KeyCombination.None;

                string modifiersString, keyString;
                // split into modifiers and key
                int index = source.LastIndexOf(DELIMITER);
                if (index > 0)
                {
                    modifiersString = source.Substring(0, index).Trim();
                    keyString = source.Substring(index + 1).Trim();
                }
                else
                {
                    modifiersString = String.Empty;
                    keyString = source.Trim();
                }
                var outKey = keyConv.ConvertFrom(context, culture, keyString);
                if (outKey != null)
                {
                    if (String.IsNullOrEmpty(modifiersString))
                        return new KeyCombination((Key)outKey);
                    else
                    {
                        var outMod = modifiersConv.ConvertFrom(context, culture, modifiersString);
                        if (outMod != null)
                        {
                            return new KeyCombination((Key)outKey, (ModifierKeys)outMod);
                        }
                    }
                }
            }
            throw GetConvertFromException(value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                    return String.Empty;

                if (value is KeyCombination combination)
                {
                    if (combination.Equals(KeyCombination.None))
                        return String.Empty;
                    // convert key and modifiers to string
                    string keyString = keyConv.ConvertTo(context, culture, combination.Key, destinationType) as string;
                    string modString = modifiersConv.ConvertTo(context, culture, combination.Modifiers, destinationType) as string;
                    return modString + DELIMITER + keyString;
                }
            }
            throw GetConvertToException(value, destinationType);
        }
    }
}
