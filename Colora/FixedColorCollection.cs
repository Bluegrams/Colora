using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Media;

namespace Colora
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class FixedColorCollection : ObservableCollection<Color>
    {
        public int MaxLength { get; private set; }

        public FixedColorCollection() : base()
        {
            MaxLength = 10;
        }

        public new void Add(Color item)
        {
            if (this.Count >= MaxLength)
                this.RemoveAt(0);
            base.Add(item);
        }

        public new void Insert(int index, Color item)
        {
            if (this.Count >= MaxLength)
                this.RemoveAt(this.Count - 1);
            base.Insert(index, item);
        }
    }
}
