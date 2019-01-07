using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Colora.Palettes
{
    public partial class PaletteWindow : Window
    {
        private Palette palette;
        private bool isSaved = false, openedFile = false;

        public static RoutedCommand EditColor => editColor;
        public static RoutedUICommand editColor = new RoutedUICommand();
        

        public PaletteWindow(Window owner) : this(owner, new Palette()) {}

        public PaletteWindow(Window owner, Palette palette)
        {
            editColor.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
            InitializeComponent();
            this.Owner = owner;
            Owner_LocationChanged(this, null);
            this.Owner.LocationChanged += Owner_LocationChanged;
            this.palette = palette;
            this.DataContext = this.palette;
            ((INotifyCollectionChanged)lstPalette.Items).CollectionChanged += Colors_CollectionChanged;
        }

        private void Colors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            isSaved = openedFile;
            openedFile = false;
        }

        public event EventHandler<ColorChangedEventArgs> ColorChanged;

        public void InsertColor(Color color)
        {
            palette.Colors.Add(new PColor(color.R, color.G, color.B));
        }

        private void Owner_LocationChanged(object sender, EventArgs e)
        {
            this.Left = Owner.Left + Owner.Width;
            this.Top = Owner.Top;
        }

        private void lstPalette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPalette.SelectedIndex != -1)
                ColorChanged?.Invoke(this, new ColorChangedEventArgs((Color)ColorConverter.ConvertFromString(((PColor)lstPalette.SelectedItem).Hex)));
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txtName.Text))
                palette.Name = txtName.Text;
            isSaved = PaletteFile.SavePalette(palette);
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Palette palette;
            openedFile = PaletteFile.OpenPalette(out palette);
            if (openedFile)
            {
                isSaved = true;
                this.palette = palette;
                this.DataContext = palette;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isSaved && palette.Colors.Count > 0)
            {
                if (MessageBox.Show(this, Properties.Resources.PaletteWindow_strNotSaved, "", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else this.Owner.Focus();
            }
        }

        private void DeleteColor_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show(this, Properties.Resources.PaletteWindow_strDelete, "", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                palette.Colors.RemoveAt(lstPalette.SelectedIndex);
            }
        }

        private void EditColor_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (lstPalette.SelectedIndex == -1) return;
            ColorEdit colEdit = new ColorEdit(this, (PColor)lstPalette.SelectedItem);
            if (colEdit.ShowDialog().Value)
            {
                palette.Colors[lstPalette.SelectedIndex] = colEdit.NewColor;
            }
        }

        private void lstPalette_MouseDoubleClick(object sender, MouseButtonEventArgs e) => EditColor_Executed(sender, null);
    }

    public class ColorChangedEventArgs : EventArgs
    {
        public Color NewColor { get; private set; }

        public ColorChangedEventArgs(Color newColor)
        {
            NewColor = newColor;
        }
    }
}
