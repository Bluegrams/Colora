using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Colora.Helpers;
using Colora.Properties;

namespace Colora.Model
{
    /// <summary>
    /// A class modeling screen capturing functionalities.
    /// </summary>
    public class ScreenPicker : INotifyPropertyChanged, IDisposable
    {
        MouseScreenCapture msc;
        NotifyColor currentColor;
        Image image;
        HotKey globalHotKey;

        public ScreenPicker(NotifyColor currentColor, Image image)
        {
            this.msc = new MouseScreenCapture();
            this.msc.CaptureTick += Msc_CaptureTick;
            this.currentColor = currentColor;
            this.image = image;
            // init values from settings
            this.ZoomValue = Settings.Default.ScreenPickerZoom;
            this.ColorSamplingMode = Settings.Default.ColorSamplingMode;
            this.ShortcutKeys = Settings.Default.GlobalShortcut;
        }

        /// <summary>
        /// Indicates whether capturing is on.
        /// </summary>
        public bool IsCapturing => msc.IsCapturing;

        public string PositionString
            => String.Format("X: {0} | Y: {1}", msc.MouseScreenPosition.X, msc.MouseScreenPosition.Y);

        public string InformationString { get; private set; }

        /// <summary>
        /// The screen picker zoom.
        /// </summary>
        public byte ZoomValue
        {
            get => (byte)Math.Floor(100.0 / msc.CaptureSize);
            set
            {
                msc.CaptureSize = 100 / value;
                Settings.Default.ScreenPickerZoom = value;
                propertyChanged();
            }
        }

        /// <summary>
        /// Specifies how the current color is sampled from screen.
        /// </summary>
        public ColorSamplingMode ColorSamplingMode
        {
            get => Settings.Default.ColorSamplingMode;
            set
            {
                msc.ColorSampleWindow = (byte)value;
                Settings.Default.ColorSamplingMode = value;
                propertyChanged();
            }
        }

        /// <summary>
        /// The (global) shortcut used for picking a position from screen.
        /// </summary>
        public KeyCombination ShortcutKeys
        {
            get => globalHotKey.KeyCombination;
            set
            {
                SetGlobalHotKey(value);
                propertyChanged();
            }
        }

        public event EventHandler PositionSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Toggles screen capturing.
        /// </summary>
        public void Capture()
        {
            if (!IsCapturing)
            {
                msc.StartCapturing();
                if (globalHotKey.IsRegistered)
                {
                    InformationString = String.Format(Resources.MainWindow_strShortcut,
                        ShortcutKeys);
                }
                else
                {
                    InformationString = Resources.MainWindow_strNoShortcut;
                }
                System.Diagnostics.Debug.WriteLine("Start: " + msc.MouseScreenPosition);
            }
            else
            {
                msc.StopCapturing();
                InformationString = "";
                System.Diagnostics.Debug.WriteLine("Stop: " + msc.MouseScreenPosition);
            }
            // invoke PropertyChanged for all properties
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        protected void SetGlobalHotKey(KeyCombination keys)
        {
            globalHotKey?.Unregister();
            Settings.Default.GlobalShortcut = keys;
            globalHotKey = new HotKey(keys, onHotKeyPressed, false);
            // don't register the new hot key if it's equal to None
            if (keys.Equals(KeyCombination.None))
                return;
            if (!globalHotKey.Register())
            {
                MessageBox.Show(String.Format(Resources.MainWindow_strHotKeyFailed, keys),
                    "Colora - Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void onHotKeyPressed(HotKey hotkey)
        {
            if (IsCapturing)
                PositionSelected?.Invoke(this, new EventArgs());
            else Capture();
        }

        private void Msc_CaptureTick(object sender, EventArgs e)
        {
            image.Source = msc.CaptureBitmapImage;
            currentColor.SetColor(msc.PointerPixelColor);
            // invoke PropertyChanged for all properties
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        protected void propertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public void Dispose()
        {
            globalHotKey?.Dispose();
        }
    }
}
