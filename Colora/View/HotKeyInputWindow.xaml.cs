using System;
using System.Windows;
using System.Windows.Input;
using Colora.Helpers;

namespace Colora.View
{
    public partial class HotKeyInputWindow : Window
    {

        public string DescriptionText
        {
            get => grpKeyGesture.Header as string;
            set => grpKeyGesture.Header = value;
        }

        /// <summary>
        /// The selected key gesture.
        /// </summary>
        public KeyCombination SelectedHotKey { get; set; }

        /// <summary>
        /// Creates a new instance of class HotKeyInpurWindow.
        /// </summary>
        /// <param name="initialKeys">The initial KeyGesture entered in the text box.</param>
        public HotKeyInputWindow(KeyCombination initialKeys = null)
        {
            this.SelectedHotKey = initialKeys;
            InitializeComponent();
            txtKeyGesture.Text = SelectedHotKey.ToString();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            txtKeyGesture.Focus();
        }

        private void TxtKeyGesture_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            // use SystemKey for Alt
            if (key == Key.System) key = e.SystemKey;
            try
            {
                // create a KeyGesture first to ensure validity
                SelectedHotKey = (KeyCombination)(new KeyGesture(key, Keyboard.Modifiers));
                txtKeyGesture.Text = SelectedHotKey.ToString();
            }
            catch (NotSupportedException) { /* invalid key combination */ }
            e.Handled = true;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
