using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Colora
{
    public class NotifyColor : INotifyPropertyChanged
    {
        private Color color;
        private double[] exHsb, exHsl, exRGB;

        // get properties:
        public byte Red { get { return color.R; } }
        public byte Green { get { return color.G; } }
        public byte Blue { get { return color.B; } }
        public byte Alpha { get { return color.A; } }
        public string HexString { get { return String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B); } }
        public int Hue { get { return (int)Math.Round(exHsb[0]); } }
        public int Bright { get { return (int)Math.Round(exHsb[2] * 100); } }
        public int Light { get { return (int)Math.Round(exHsl[2] * 100); } }
        public int SatHSB { get { return (int)Math.Round(exHsb[1] * 100); } }
        public int SatHSL { get { return (int)Math.Round(exHsl[1] * 100); } }
        public double Cyan { get { return ColorConversion.RGBToCMYK(exRGB[0], exRGB[1], exRGB[2])[0]; } }
        public double Magenta { get { return ColorConversion.RGBToCMYK(exRGB[0], exRGB[1], exRGB[2])[1]; } }
        public double Yellow { get { return ColorConversion.RGBToCMYK(exRGB[0], exRGB[1], exRGB[2])[2]; } }
        public double Key { get { return ColorConversion.RGBToCMYK(exRGB[0], exRGB[1], exRGB[2])[3]; } }
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

        private void setNewColor(Color col)
        {
            if (col != color)
            {
                color = col;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(null));
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
            exRGB = new double[] { r / 255.0, g / 255.0, b / 255.0 };
            exHsb = ColorConversion.RGBToHSB(exRGB[0], exRGB[1], exRGB[2]);
            exHsl = ColorConversion.RGBToHSL(exRGB[0], exRGB[1], exRGB[2]);
            setNewColor(Color.FromRgb(r, g, b));
        }

        public void SetFromHSL(int h, double s, double l)
        {
            if (s > 1 || l > 1 || h > 360 || h < 0) throw new ArgumentException();
            exHsl = new double[] { h, s, l };
            exRGB = ColorConversion.HSLToRGB(h, s, l);
            exHsb = ColorConversion.RGBToHSB(exRGB[0], exRGB[1], exRGB[2]);
            setNewColor(Color.FromRgb((byte)Math.Round(exRGB[0] * 255), (byte)Math.Round(exRGB[1] * 255), (byte)Math.Round(exRGB[2] * 255)));
        }

        public void SetFromHSB(int h, double s, double b)
        {
            if (s > 1 || b > 1 || h > 360 || h < 0) throw new ArgumentException();
            exHsb = new double[] { h, s, b };
            exRGB = ColorConversion.HSBToRGB(h, s, b);
            exHsl = ColorConversion.RGBToHSL(exRGB[0], exRGB[1], exRGB[2]);
            setNewColor(Color.FromRgb((byte)Math.Round(exRGB[0] * 255), (byte)Math.Round(exRGB[1] * 255), (byte)Math.Round(exRGB[2] * 255)));
        }

        public override string ToString()
        {
            return color.ToString();
        }
    }
}
