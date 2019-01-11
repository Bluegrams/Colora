using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Colora
{
    /// <summary>
    /// An observable collection of fixed size that holds the last colors.
    /// </summary>
    public class FixedColorCollection : ObservableCollection<Color>
    {
        private ushort maxLength = ushort.MaxValue;

        public ushort MaxLength
        {
            get => maxLength;
            set
            {
                maxLength = value;
                prune();
            }
        }

        public FixedColorCollection() { }

        public FixedColorCollection(ushort maxLength)
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

        private void prune()
        {
            while (Count > MaxLength)
                this.RemoveItem(Count - 1);
        }
    }
}
