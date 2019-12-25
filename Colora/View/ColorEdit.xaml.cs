using System;
using System.Windows;
using System.Windows.Media;
using Colora.Model;

namespace Colora.View
{
    public partial class ColorEdit : Window
    {
        public PColor NewColor, editColor;

        public ColorEdit(Window owner, PColor color)
        {
            this.Owner = owner;
            InitializeComponent();
            editColor = new PColor(color);
            NewColor = color;
            this.DataContext = editColor;
        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // to check if the given hex string is valid:
                ColorConverter.ConvertFromString(editColor.Hex);
            }
            catch
            {
                MessageBox.Show(this, Properties.Resources.ColorEdit_strInvalidColor, "", MessageBoxButton.OK, MessageBoxImage.Error);
                editColor = new PColor(NewColor);
                this.DataContext = editColor;
                return;
            }
            NewColor = editColor;
            this.DialogResult = true;
            this.Close();
        }
    }
}
