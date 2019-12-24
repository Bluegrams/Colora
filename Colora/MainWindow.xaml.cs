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
using Colora.Palettes;
using Colora.Capturing;

namespace Colora
{
    public partial class MainWindow : Window
    {
        private WpfWindowManager manager;
        private WpfUpdateChecker updateChecker;
        private MouseScreenCapture msc;
        private FixedColorCollection colorHistory;
        private PaletteWindow palWindow;
        private HotKey pickColorHotKey;

        public Settings Settings { get; set; }
        public NotifyColor CurrentColor { get; set; }

        public MainWindow()
        {
            Settings = new Settings();
            manager = new WpfWindowManager(this);
            manager.ManageDefault();
            manager.Manage(nameof(Topmost));
            manager.Manage(nameof(Settings),
                System.Configuration.SettingsSerializeAs.Xml, roamed: true);
            manager.ApplyToSettings(Properties.Settings.Default);
            manager.Initialize();
            InitializeComponent();
            updateChecker = new WpfUpdateChecker(App.UPDATE_URL, this, App.UPDATE_MODE);
            msc = new MouseScreenCapture();
            msc.CaptureTick += new EventHandler(capture_Tick);
            ((INotifyCollectionChanged)lstHistory.Items).CollectionChanged += LastColors_CollectionChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Check for updates
            updateChecker.CheckForUpdates();
            // Set global shortcut
            setNewHotKey(Settings.PickColorShortcut);
            // Load color history
            if (Properties.Settings.Default.LatestColors != null)
            {
                colorHistory = Properties.Settings.Default.LatestColors;
                colorHistory.MaxLength = Settings.ColorHistoryLength;
            }
            else colorHistory = new FixedColorCollection(Settings.ColorHistoryLength);
            lstHistory.ItemsSource = colorHistory;
            // Set current color
            CurrentColor = new NotifyColor(Properties.Settings.Default.CurrentColor);
            this.DataContext = this;
            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void onHotKeyPressed(HotKey hotKey)
        {
            if ((bool)butPick.IsChecked)
                colorHistory.Insert(0, CurrentColor.WpfColor);
            else
                butPick.IsChecked = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                butPick.IsChecked = false;
                colorHistory.Insert(0, CurrentColor.WpfColor);
            }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.ScreenPickerVisible))
            {
                // Hide the screen picker groupbox and adjust some UI sizes.
                if (!Settings.ScreenPickerVisible)
                {
                    grpScreenPicker.Visibility = Visibility.Collapsed;
                    grpLatest.MaxWidth = 242;
                    expData.MaxWidth = 242;
                }
                else
                {
                    grpScreenPicker.Visibility = Visibility.Visible;
                    grpLatest.MaxWidth = 346;
                    expData.MaxWidth = 346;
                }
            }
        }

        private void ScreenPickerVisible_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.ScreenPickerVisible = !Settings.ScreenPickerVisible;
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

        private void TopMostCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
        }

        private void menDeleteLatest_Click(object sender, RoutedEventArgs e) => colorHistory.Clear();

        private void menConfigureShortcut_Click(object sender, RoutedEventArgs e)
        {
            HotKeyInputWindow hotKeyInput = new HotKeyInputWindow(pickColorHotKey.KeyCombination);
            hotKeyInput.Owner = this;
            if (hotKeyInput.ShowDialog() == true)
            {
                setNewHotKey(hotKeyInput.HotKey);
            }
        }

        private void setNewHotKey(KeyCombination keys)
        {
            pickColorHotKey?.Unregister();
            Settings.PickColorShortcut = keys;
            pickColorHotKey = new HotKey(keys, onHotKeyPressed, false);
            if (!pickColorHotKey.Register())
            {
                MessageBox.Show(String.Format(Properties.Resources.MainWindow_strHotKeyFailed, keys),
                    "Colora - Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var baseUri = BaseUriHelper.GetBaseUri(this);
            BitmapSource img = new BitmapImage(new Uri(baseUri, @"/img/colora.png"));
            AboutBox aboutBox = new AboutBox(img);
            aboutBox.Owner = this;
            aboutBox.UpdateChecker = updateChecker;
            aboutBox.Show();
        }

        private void LastColors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                lstHistory.ScrollIntoView(e.NewItems[0]);
                lstHistory.SelectedItem = e.NewItems[0];
            }
        }

        private void butPick_Checked(object sender, RoutedEventArgs e)
        {
            grpScreenPicker.IsEnabled = true;
            msc.StartCapturing();
            statInfo.Content = String.Format(Properties.Resources.MainWindow_strShortcut,
                pickColorHotKey.KeyCombination);
            System.Diagnostics.Debug.WriteLine("Start: " + msc.MouseScreenPosition);       
        }

        private void butPick_Unchecked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Stop: " + msc.MouseScreenPosition);
            msc.StopCapturing();
            grpScreenPicker.IsEnabled = false;
            statInfo.Content = "";
        }

        private void capture_Tick(object sender, EventArgs e)
        {
            imgScreen.Source = msc.CaptureBitmapImage;
            CurrentColor.SetColor(msc.PointerPixelColor);
            lblScreenCoord.Content = String.Format("X: {0} | Y: {1}", msc.MouseScreenPosition.X, msc.MouseScreenPosition.Y);
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

        private void lstHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstHistory.SelectedIndex != -1)
                CurrentColor.SetColor(colorHistory[lstHistory.SelectedIndex]);
        }

        private void butAddLast_Click(object sender, RoutedEventArgs e)
        {
            colorHistory.Insert(0, CurrentColor.WpfColor);
        }

        private void butAddPalette_Click(object sender, RoutedEventArgs e)
        {
            palWindow?.InsertColor(CurrentColor.WpfColor);
        }

        private void MenuPickScreen_Click(object sender, RoutedEventArgs e)
        {
            butPick.IsChecked = !butPick.IsChecked;
        }

        private void sldZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (msc == null) return;
            msc.CaptureSize = 100 / (int)sldZoom.Value;
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
            pickColorHotKey?.Dispose();
            Properties.Settings.Default.LatestColors = colorHistory;
            Properties.Settings.Default.CurrentColor = CurrentColor.WpfColor;
            Properties.Settings.Default.Save();
        }
    }
}
