using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Media;

namespace Colora
{
    /// <summary>
    /// An observable collection of fixed size that holds the last colors.
    /// </summary>
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class FixedColorCollection : ObservableCollection<Color>
    {
        public int MaxLength { get; private set; }

        public FixedColorCollection(int maxLength)
        {
            MaxLength = maxLength;
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
