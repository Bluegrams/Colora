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
        private bool screenPickerVisible = true;
        private KeyCombination pickColorShortcut 
            = new KeyCombination(Key.C, ModifierKeys.Control | ModifierKeys.Alt);

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
        /// Determines if the screen picker groupbox is visible.
        /// </summary>
        public bool ScreenPickerVisible
        {
            get => screenPickerVisible;
            set
            {
                screenPickerVisible = value;
                propertyChanged();
            }
        }

        /// <summary>
        /// Specifies the maximal number of items in the color history list.
        /// </summary>
        public ushort ColorHistoryLength { get; set; } = 16;

        /// <summary>
        /// Specifies the global shortcut keys used for picking colors from screen.
        /// </summary>
        public KeyCombination PickColorShortcut
        {
            get => pickColorShortcut;
            set
            {
                pickColorShortcut = value;
                propertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void propertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
