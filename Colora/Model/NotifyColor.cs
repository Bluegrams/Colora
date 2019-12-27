using System;
using System.ComponentModel;
using System.Windows.Media;
using Colora.Helpers;

namespace Colora.Model
{
    public class NotifyColor : INotifyPropertyChanged
    {
        private Color color;
        private double[] exHsb, exHsl, exRGB, exCMYK;

        public byte Red => color.R;
        public byte Green => color.G;
        public byte Blue => color.B;
        public byte Alpha => color.A;
        public double RedF => exRGB[0];
        public double GreenF => exRGB[1];
        public double BlueF => exRGB[2];
        public string HexString { get { return String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B); } }
        public int Hue { get { return (int)Math.Round(exHsb[0]); } }
        public int Bright { get { return (int)Math.Round(exHsb[2] * 100); } }
        public int Light { get { return (int)Math.Round(exHsl[2] * 100); } }
        public int SatHSB { get { return (int)Math.Round(exHsb[1] * 100); } }
        public int SatHSL { get { return (int)Math.Round(exHsl[1] * 100); } }
        public int Cyan { get { return (int)Math.Round(exCMYK[0] * 100); } }
        public int Magenta { get { return (int)Math.Round(exCMYK[1] * 100); } }
        public int Yellow { get { return (int)Math.Round(exCMYK[2] * 100); } }
        public int Key { get { return (int)Math.Round(exCMYK[3] * 100); } }
        public Color WpfColor { get { return color; } }
        public SolidColorBrush Brush { get { return new SolidColorBrush(color); } }   

        public event PropertyChangedEventHandler PropertyChanged;

        public NotifyColor(Color color)
        {
            SetColor(color);
        }

        public NotifyColor(byte r, byte g, byte b)
        {
            SetFromRGB(r, g, b);
        }

        private void setNewColor()
        {
            Color newCol = Color.FromRgb((byte)Math.Round(exRGB[0] * 255),
                                         (byte)Math.Round(exRGB[1] * 255), 
                                         (byte)Math.Round(exRGB[2] * 255));
            if (newCol != color)
            {
                color = newCol;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public void SetColor(Color color)
        {
            SetFromRGB(color.R, color.G, color.B);
        }

        public void SetFromHex(string hexstring)
        {
            try
            {
                Color newcol = (Color)ColorConverter.ConvertFromString(hexstring);
                SetFromRGB(newcol.R, newcol.G, newcol.B);
            } catch { }
        }

        public void SetFromRGB(byte r, byte g, byte b)
        {
            SetFromRGB(r / 255.0, g / 255.0, b / 255.0);
        }

        public void SetFromRGB(double r, double g, double b)
        {
            exRGB = new double[] { r, g, b };
            exHsb = ColorConversion.RGBToHSB(exRGB[0], exRGB[1], exRGB[2]);
            exHsl = ColorConversion.RGBToHSL(exRGB[0], exRGB[1], exRGB[2]);
            exCMYK = ColorConversion.RGBToCMYK(exRGB[0], exRGB[1], exRGB[2]);
            setNewColor();
        }

        public void SetFromHSL(int h, double s, double l)
        {
            if (s > 1 || l > 1 || h > 360 || h < 0) throw new ArgumentException();
            exHsl = new double[] { h, s, l };
            exRGB = ColorConversion.HSLToRGB(h, s, l);
            exHsb = ColorConversion.RGBToHSB(exRGB[0], exRGB[1], exRGB[2]);
            exCMYK = ColorConversion.RGBToCMYK(exRGB[0], exRGB[1], exRGB[2]);
            setNewColor();
        }

        public void SetFromHSB(int h, double s, double b)
        {
            if (s > 1 || b > 1 || h > 360 || h < 0) throw new ArgumentException();
            exHsb = new double[] { h, s, b };
            exRGB = ColorConversion.HSBToRGB(h, s, b);
            exHsl = ColorConversion.RGBToHSL(exRGB[0], exRGB[1], exRGB[2]);
            exCMYK = ColorConversion.RGBToCMYK(exRGB[0], exRGB[1], exRGB[2]);
            setNewColor();
        }

        public void SetFromCMYK(double c, double m, double y, double k)
        {
            if (c > 1 || m > 1 || y > 1 || k > 1) throw new ArgumentException();
            exCMYK = new double[] { c, m, y, k };
            exRGB = ColorConversion.CMYKToRGB(c, m, y, k);
            exHsb = ColorConversion.RGBToHSB(exRGB[0], exRGB[1], exRGB[2]);
            exHsl = ColorConversion.RGBToHSL(exRGB[0], exRGB[1], exRGB[2]);
            setNewColor();
        }

        public override string ToString()
        {
            return color.ToString();
        }
    }
}
