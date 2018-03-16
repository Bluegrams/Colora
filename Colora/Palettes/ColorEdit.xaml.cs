using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Colora.Palettes
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
