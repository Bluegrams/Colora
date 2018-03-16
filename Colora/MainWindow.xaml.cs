using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Bluegrams.Application;
using Bluegrams.Application.WPF;
using Colora.Palettes;
using System.Globalization;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;

namespace Colora
{
    public partial class MainWindow : Window
    {
        private MiniAppManager manager;
        private MouseScreenCapture msc;
        private FixedColorCollection lastColors;
        private PaletteWindow palWindow;

        public NotifyColor CurrentColor { get; set; }

        [DllImport("User32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("User32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public MainWindow()
        {
            var baseUri = BaseUriHelper.GetBaseUri(this);
            BitmapSource img = new BitmapImage(new Uri(baseUri, @"/img/colorasmall.png"));
            manager = new MiniAppManager(this, Colors.Gray, img, new Link("https://colora.sourceforge.io"), new Link("LICENSE.rtf", "BSD-2-Clause"));
            manager.SupportedCultures = new CultureInfo[] { new CultureInfo("en"), new CultureInfo("de") };
            manager.Initialize();
            manager.CheckForUpdates("https://colora.sourceforge.io/update.xml");
            InitializeComponent();
            CurrentColor = new NotifyColor(Color.FromRgb(255, 0, 0));
            this.DataContext = CurrentColor;
            msc = new MouseScreenCapture();
            msc.CaptureTick += new EventHandler(capture_Tick);
            lastColors = new FixedColorCollection();
            ((INotifyCollectionChanged)lstboxLast.Items).CollectionChanged += LastColors_CollectionChanged;
            if (Properties.Settings.Default.LatestColors != null)
                lastColors = Properties.Settings.Default.LatestColors;
            lstboxLast.ItemsSource = lastColors;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource src = HwndSource.FromHwnd(handle);
            src.AddHook(new HwndSourceHook(WndProc));
            MainWindow.RegisterHotKey(handle, this.GetHashCode(), 0x0003, (int)'C');
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // WM_HOTKEY = 0x0312
            if (msg == 0x0312)
            {
                if ((bool)butPick.IsChecked)
                    lastColors.Insert(0, CurrentColor.WpfColor);
                else
                    butPick.IsChecked = true;
            }
            return IntPtr.Zero;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                butPick.IsChecked = false;
                lastColors.Insert(0, CurrentColor.WpfColor);
            }
        }

        private void SizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (grpScreenPicker.IsVisible)
            {
                grpScreenPicker.Visibility = Visibility.Collapsed;
                grpLatest.MaxWidth = 242;
                expData.MaxWidth = 242;
                panButtonsBottom.Visibility = Visibility.Collapsed;
            }
            else
            {
                grpScreenPicker.Visibility = Visibility.Visible;
                grpLatest.MaxWidth = 346;
                expData.MaxWidth = 346;
                panButtonsBottom.Visibility = Visibility.Visible;
            }
        }

        private void SelectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            cd.FullOpen = true;
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrentColor.SetFromRGB(cd.Color.R, cd.Color.G, cd.Color.B);
                lastColors.Insert(0, CurrentColor.WpfColor);
            }
        }

        private void FirstPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            butTopmost.IsChecked = !butTopmost.IsChecked;
        }


        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            manager.ShowAboutBox();
        }

        private void menDeleteLatest_Click(object sender, RoutedEventArgs e) => lastColors.Clear();

        private void LastColors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                lstboxLast.ScrollIntoView(e.NewItems[0]);
                lstboxLast.SelectedItem = e.NewItems[0];
            }
        }

        private void butPick_Checked(object sender, RoutedEventArgs e)
        {
            grpScreenPicker.IsEnabled = true;
            msc.StartCapturing();
            statInfo.Content = Properties.Resources.MainWindow_strShortcut;
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

        private void butTopmost_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = (bool)butTopmost.IsChecked;
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

        private void lstboxLast_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstboxLast.SelectedIndex != -1)
                CurrentColor.SetColor(lastColors[lstboxLast.SelectedIndex]);
        }

        private void butAddLast_Click(object sender, RoutedEventArgs e)
        {
            lastColors.Insert(0, CurrentColor.WpfColor);
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
            msc.RealCaptureSize = 100 / (int)sldZoom.Value;
        }

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

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Palette palette = new Palette();
            foreach (Color col in lastColors)
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.LatestColors = lastColors;
            Properties.Settings.Default.Save();
        }
    }
}