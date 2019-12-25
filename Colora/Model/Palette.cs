using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Colora.Model
{
    [Serializable]
    public class Palette
    {
        [XmlAttribute]
        public string Name { get; set; }
        public ObservableCollection<PColor> Colors { get; set; }

        public Palette()
        {
            Colors = new ObservableCollection<PColor>();
        }
    }

    [Serializable]
    public class PColor
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Hex { get; set; }

        public PColor() : this(0, 0, 0) { }

        public PColor(byte r, byte g, byte b) : this(r, g, b, null) { }

        public PColor(byte r, byte g, byte b, string name)
        {
            Hex = String.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
            Name = name;
        }

        public PColor(PColor color)
        {
            this.Hex = color.Hex;
            this.Name = color.Name;
        }
    }
}
