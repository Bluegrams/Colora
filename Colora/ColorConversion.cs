using System;

namespace Colora
{
    /// <summary>
    /// Methods for converting color values between RGB, HSL, HSB and CMYK.
    /// (Note that RGB values must be given as numbers between 0 and 1.)
    /// </summary>
    static class ColorConversion
    {
        /// <summary>
        /// Convert RGB to HSB.
        /// </summary>
        /// <param name="R">Red value between 0 and 1.</param>
        /// <param name="G">Green value between 0 and 1.</param>
        /// <param name="B">Blue value between 0 and 1.</param>
        /// <returns>Array with three elements containing H value between 0 and 360 and S and B value between 0 and 1.</returns>
        public static double[] RGBToHSB(double R, double G, double B)
        {
            double max = (double)Math.Max(Math.Max(R, G), B);
            double min = (double)Math.Min(Math.Min(R, G), B);
            double diff = max - min;
            double h = calcH(R, G, B, diff);
            double s = (max == 0) ? 0 : diff / max;
            return new double[] { h, s, max };
        }

        /// <summary>
        /// Convert RGB to HSL.
        /// </summary>
        /// <param name="R">Red value between 0 and 1.</param>
        /// <param name="G">Green value between 0 and 1.</param>
        /// <param name="B">Blue value between 0 and 1.</param>
        /// <returns>Array with three elements containing H value between 0 and 360 and S and L value between 0 and 1.</returns>
        public static double[] RGBToHSL(double R, double G, double B)
        {
            double max = (double)Math.Max(Math.Max(R, G), B);
            double min = (double)Math.Min(Math.Min(R, G), B);
            double diff = max - min;
            double h = calcH(R, G, B, diff);
            double s = (max == 0 || min == 1) ? 0 : diff / (1 - Math.Abs(max + min - 1));
            double l = (max + min) / 2;
            return new double[] { h, s, l };
        }

        private static double calcH(double r, double g, double b, double diff)
        {
            if (r == g && g == b)
                return 0;
            else if (r >= g && r >= b)
                return hueTerm(0, g, b, diff);
            else if (g >= r && g >= b)
                return hueTerm(2, b, r, diff);
            else
                return hueTerm(4, r, g, diff);
        }

        private static double hueTerm(int s, double n1, double n2, double diff)
        {
            double res = 60 * (s + (n1 - n2) / diff);
            return res > 0 ? res : res + 360;
        }

        /// <summary>
        /// Convert RGB to CMYK.
        /// </summary>
        /// <param name="R">Red value between 0 and 1.</param>
        /// <param name="G">Green value between 0 and 1.</param>
        /// <param name="B">Blue value between 0 and 1.</param>
        /// <returns>Array with four elements containing C, M, Y and K value between 0 and 1.</returns>
        public static double[] RGBToCMYK(double R, double G, double B)
        {
            double k = 1 - Math.Max(Math.Max(R, G), B);
            double c = k == 1 ? 0 : (1 - R - k) / (1 - k);
            double m = k == 1 ? 0 : (1 - G - k) / (1 - k);
            double y = k == 1 ? 0 : (1 - B - k) / (1 - k);
            return new double[] { c, m, y, k };
        }

        /// <summary>
        /// Convert CMYK to RGB.
        /// </summary>
        /// <param name="c">Cyan value between 0 and 1.</param>
        /// <param name="m">Magenta value between 0 and 1.</param>
        /// <param name="y">Yellow value between 0 and 1.</param>
        /// <param name="k">Key value between 0 and 1.</param>
        /// <returns>Array with three elements conatining R, G and B value.</returns>
        public static double[] CMYKToRGB(double c, double m, double y, double k)
        {
            return new double[] { (1 - c) * (1 - k), (1 - m) * (1 - k), (1 - y) * (1 - k) };
        }

        /// <summary>
        /// Convert HSB to RGB.
        /// </summary>
        /// <param name="H">Hue value between 0 and 360.</param>
        /// <param name="S">Saturation value between 0 and 1.</param>
        /// <param name="B">Brightness value between 0 and 1.</param>
        /// <returns>Array with three elements containing R, G and B value.</returns>
        public static double[] HSBToRGB(int H, double S, double B)
        {
            int h = H / 60;
            double f = H / (double)60 - h;
            double p = B * (1 - S);
            double q = B * (1 - S * f);
            double t = B * (1 - S * (1 - f));
            switch (h)
            {
                case 0:
                    return new double[] { B, t, p };
                case 1:
                    return new double[] { q, B, p };
                case 2:
                    return new double[] { p, B, t };
                case 3:
                    return new double[] { p, q, B };
                case 4:
                    return new double[] { t, p, B };
                case 5:
                    return new double[] { B, p, q };
                default:
                    return new double[] { B, t, p };
            }
        }

        /// <summary>
        /// Convert HSL to RGB.
        /// </summary>
        /// <param name="H">Hue value between 0 and 360.</param>
        /// <param name="S">Saturation value between 0 and 1.</param>
        /// <param name="L">Lightness value between 0 and 1.</param>
        /// <returns>Array with three elements containing R, G and B value.</returns>
        public static double[] HSLToRGB(int H, double S, double L)
        {
            double C = (1 - Math.Abs(2 * L - 1)) * S;
            double hh = H / 60.0;
            double X = C * (1 - Math.Abs(hh % 2.0 - 1));
            double[] rgb = new double[] { 0, 0, 0 };
            if (hh <= 1)
                rgb = new double[] { C, X, 0 };
            else if (hh <= 2)
                rgb = new double[] { X, C, 0 };
            else if (hh <= 3)
                rgb = new double[] { 0, C, X };
            else if (hh <= 4)
                rgb = new double[] { 0, X, C };
            else if (hh <= 5)
                rgb = new double[] { X, 0, C };
            else if (hh <= 6)
                rgb = new double[] { C, 0, X };
            double m = L - 0.5 * C;
            rgb[0] += m; rgb[1] += m; rgb[2] += m;
            return rgb;
        }
    }
}