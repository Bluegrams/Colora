using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Colora.Capturing;

namespace Colora
{
    [Serializable]
    public class Settings : INotifyPropertyChanged
    {
        private bool iconBarVisible = true;
        private bool colorHistoryVisible = true;

        /// <summary>
        /// Determines if the main icon bar is visible.
        /// </summary>
        public bool IconBarVisible
        {
            get => iconBarVisible;
            set
            {
                iconBarVisible = value;
                propertyChanged();
            }
        }

        /// <summary>
        /// Determines if the color history list is visible.
        /// </summary>
        public bool ColorHistoryVisible
        {
            get => colorHistoryVisible;
            set
            {
                colorHistoryVisible = value;
                propertyChanged();
            }
        }

        /// <summary>
        /// Specifies the maximal number of items in the color history list.
        /// </summary>
        public int ColorHistoryLength { get; set; } = 16;

        /// <summary>
        /// Specifies the global shortcut keys used for picking colors from screen.
        /// </summary>
        public KeyCombination PickColorShortcut { get; set; }
            = new KeyCombination(Key.C, ModifierKeys.Control | ModifierKeys.Alt);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void propertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
