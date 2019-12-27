using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Bluegrams.Application;
using Bluegrams.Application.WPF;
using Colora.Model;
using Colora.Properties;
using Colora.View;

namespace Colora
{
    public partial class MainWindow : Window
    {
        private WpfWindowManager manager;
        private WpfUpdateChecker updateChecker;
        private FixedColorCollection colorHistory;
        private PaletteWindow palWindow;

        public NotifyColor CurrentColor { get; set; }

        public ScreenPicker ScreenPicker { get; set; }

        public MainWindow()
        {
            manager = new WpfWindowManager(this);
            manager.ManageDefault();
            manager.Manage(nameof(Topmost));
            manager.ApplyToSettings(Settings.Default);
            manager.Initialize();
            InitializeComponent();
            updateChecker = new WpfUpdateChecker(App.UPDATE_URL, this, App.UPDATE_MODE);
            ((INotifyCollectionChanged)lstHistory.Items).CollectionChanged += LastColors_CollectionChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Check for updates
            updateChecker.CheckForUpdates();
            // Load color history
            if (Settings.Default.LatestColors != null)
            {
                colorHistory = Settings.Default.LatestColors;
                colorHistory.MaxLength = Settings.Default.ColorHistoryLength;
            }
            else colorHistory = new FixedColorCollection(Settings.Default.ColorHistoryLength);
            lstHistory.ItemsSource = colorHistory;
            // Set current color
            CurrentColor = new NotifyColor(Settings.Default.CurrentColor);
            // Init screen picker
            ScreenPicker = new ScreenPicker(CurrentColor, imgScreen);
            ScreenPicker.PositionSelected += ScreenPicker_PositionSelected;
            // Set data context
            this.DataContext = this;
        }

        private void colorCaptured()
        {
            colorHistory.Insert(0, CurrentColor.WpfColor);
            if (Settings.Default.ClipboardAutoCopy)
                Clipboard.SetText(CurrentColor.HexString);
        }

        private void ScreenPicker_PositionSelected(object sender, EventArgs e)
            => colorCaptured();

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (ScreenPicker.IsCapturing)
                {
                    ScreenPicker.Capture();
                    colorCaptured();
                }
                e.Handled = true;
            }
        }

        private void PickFromScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScreenPicker.Capture();
        }

        private void SelectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            cd.FullOpen = true;
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentColor.SetFromRGB(cd.Color.R, cd.Color.G, cd.Color.B);
                colorHistory.Insert(0, CurrentColor.WpfColor);
            }
        }

        private void ScreenPickerVisible_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.Default.ScreenPickerVisible = !Settings.Default.ScreenPickerVisible;
        }

        private void TopMostCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
        }

        private void menDeleteLatest_Click(object sender, RoutedEventArgs e) => colorHistory.Clear();

        private void menConfigureShortcut_Click(object sender, RoutedEventArgs e)
            => App.ConfigureShortcut(this, ScreenPicker);

        private void SettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(ScreenPicker);
            settingsWindow.Owner = this;
            if (settingsWindow.ShowDialog().GetValueOrDefault())
            {
                // apply changed settings
                colorHistory.MaxLength = Settings.Default.ColorHistoryLength;
            }
        }

        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var baseUri = BaseUriHelper.GetBaseUri(this);
            BitmapSource img = new BitmapImage(new Uri(baseUri, @"/img/icon.png"));
            AboutBox aboutBox = new AboutBox(img);
            aboutBox.Owner = this;
            aboutBox.UpdateChecker = updateChecker;
            aboutBox.ShowDialog();
        }

        private void LastColors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                lstHistory.ScrollIntoView(e.NewItems[0]);
                lstHistory.SelectedItem = e.NewItems[0];
            }
        }
        
        private void inputColor_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)e.Source;
            string s = tb.Text;
            BindingOperations.ClearBinding(tb, TextBox.TextProperty);
            tb.Text = s;
        }

        private void inputColor_LostFocus(object sender, RoutedEventArgs e)
        {

            TextBox tb = (TextBox)e.Source;
            string s = tb.Text;
            Binding newbd = new Binding(tb.Tag.ToString());
            newbd.Mode = BindingMode.OneWay;
            tb.SetBinding(TextBox.TextProperty, newbd);
        }

        private void inputColor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(ch => Char.IsDigit(ch) || Char.IsControl(ch));
        }

        private void Grid_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!(e.Source as Control).IsFocused) return;
            int rVal = 0, gVal = 0, bVal = 0;
            if (int.TryParse(txtR.Text, out rVal) && int.TryParse(txtG.Text, out gVal) && int.TryParse(txtB.Text, out bVal))
            {
                CurrentColor.SetFromRGB((byte)Math.Min(255, rVal), (byte)Math.Min(255, gVal), (byte)Math.Min(255, bVal));
            }
        }

        private void txtHEX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(e.Source as Control).IsFocused) return;
            CurrentColor.SetFromHex(txtHEX.Text);
        }

        private void setColorFromHistory()
        {
            if (lstHistory.SelectedIndex != -1)
            {
                if (colorHistory[lstHistory.SelectedIndex] != CurrentColor.WpfColor)
                    CurrentColor.SetColor(colorHistory[lstHistory.SelectedIndex]);
            }
        }

        private void lstHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => setColorFromHistory();

        private void lstHistory_MouseDown(object sender, MouseButtonEventArgs e)
            => setColorFromHistory();

        private void butAddLast_Click(object sender, RoutedEventArgs e)
        {
            colorHistory.Insert(0, CurrentColor.WpfColor);
            Settings.Default.ColorHistoryVisible = true;
            lstHistory.SelectedIndex = 0;
            lstHistory.Focus();
        }

        private void butAddPalette_Click(object sender, RoutedEventArgs e)
        {
            palWindow?.InsertColor(CurrentColor.WpfColor);
        }

        private void menDeleteHistoryItem_Click(object sender, RoutedEventArgs e)
        {
            colorHistory.RemoveAt(lstHistory.SelectedIndex);
        }

        #region Advanced Color Options
        private void rgbSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (((UIElement)e.Source).IsFocused)
                CurrentColor.SetFromRGB((byte)sldR.Value, (byte)sldG.Value, (byte)sldB.Value);
        }

        private void rgbfSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (((UIElement)e.Source).IsFocused)
                CurrentColor.SetFromRGB(sldRf.Value, sldGf.Value, sldBf.Value);
        }

        private void hsbSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (((UIElement)e.Source).IsFocused)
                CurrentColor.SetFromHSB((int)sldHsbH.Value, (double)sldHsbS.Value / 100, (double)sldHsbB.Value / 100);
        }

        private void hslSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (((UIElement)e.Source).IsFocused)
                CurrentColor.SetFromHSL((int)sldHslH.Value, (double)sldHslS.Value / 100, (double)sldHslL.Value / 100);
        }

        private void CMYK_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(e.Source as Control).IsFocused) return;
            if (int.TryParse(txtCyan.Text, out int c) && int.TryParse(txtMagenta.Text, out int m)
                && int.TryParse(txtYellow.Text, out int y) && int.TryParse(txtKey.Text, out int k))
            {
                CurrentColor.SetFromCMYK(Math.Min(c, 100) / 100.0, Math.Min(m, 100) / 100.0,
                                         Math.Min(y, 100) / 100.0, Math.Min(k, 100) / 100.0);
            }
        }

        private void butCopyRGB_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(String.Format("{0}, {1}, {2}", txtR.Text, txtG.Text, txtB.Text));
        }

        private void butCopyHex_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtHEX.Text);
        }

        private void butCopyHSB_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(String.Format("{0}, {1}%, {2}%", CurrentColor.Hue, CurrentColor.SatHSB, CurrentColor.Bright));
        }

        private void butCopyHSL_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(String.Format("{0}, {1}%, {2}%", CurrentColor.Hue, CurrentColor.SatHSL, CurrentColor.Light));
        }

        private void butCopyCMYK_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(String.Format("{0}, {1}, {2}, {3}", txtCyan.Text, txtMagenta.Text, txtYellow.Text, txtKey.Text));
        }
        #endregion

        #region Palette Editor
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Palette palette = new Palette();
            foreach (Color col in colorHistory)
            {
                palette.Colors.Add(new PColor(col.R, col.G, col.B));
            }
            if (palWindow != null)
                palWindow.Close();
            if (palWindow == null || !palWindow.IsVisible)
                showNewPalette(palette);
        }

        private void PaletteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (palWindow != null && palWindow.IsVisible)
                palWindow.Focus();
            else
                showNewPalette();
        }

        private void showNewPalette(Palette palette = null)
        {
            palette = palette ?? new Palette();
            palWindow = new PaletteWindow(this, palette);
            palWindow.Closing += PalWindow_Closing;
            palWindow.ColorChanged += PalWindow_ColorChanged;
            palWindow.Show();
            butAddPalette.IsEnabled = true;
        }

        private void PalWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            butAddPalette.IsEnabled = false;
        }

        private void PalWindow_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            CurrentColor.SetColor(e.NewColor);
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ScreenPicker.Dispose();
            Settings.Default.LatestColors = colorHistory;
            Settings.Default.CurrentColor = CurrentColor.WpfColor;
            Settings.Default.Save();
        }
    }
}
