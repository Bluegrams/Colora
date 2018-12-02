using System;
using System.Windows;
using System.Windows.Input;

namespace Colora.Capturing
{
    public partial class HotKeyInputWindow : Window
    {
        public KeyCombination HotKey { get; private set; }

        public HotKeyInputWindow(KeyCombination initialKeys)
        {
            InitializeComponent();
            // Add keys
            for (int i = 34; i <= 69; i++)
                comKeys.Items.Add((Key)i);
            for (int i = 90; i <= 101; i++)
                comKeys.Items.Add((Key)i);
            // Set key
            HotKey = initialKeys;
            DataContext = HotKey;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (HotKey.Modifiers == ModifierKeys.None)
            {
                MessageBox.Show(Properties.Resources.HotKeyInputWindow_strModifierRequired,
                    "Colora - Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            this.DialogResult = true;
            this.Close();
        }
    }
}
