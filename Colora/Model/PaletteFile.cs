using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Media;
using System.Xml;

namespace Colora.Model
{
    static class PaletteFile
    {

        internal static bool SavePalette(Palette palette)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.DefaultExt = ".col";
            sfd.Filter = "Colora Palette |*.col|Gimp Palette |*.gpl";
            sfd.FileName = palette.Name;
            sfd.Title = "Save Color Palette";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    switch (Path.GetExtension(sfd.FileName))
                    {
                        case ".gpl":
                            saveGimpPalette(sfd.FileName, palette);
                            break;
                        default:
                            saveColoraPalette(sfd.FileName, palette);
                            break;
                    }
                    sfd.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0} {1}", Properties.Resources.PaletteWindow_strErrorSave, ex.Message), "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            sfd.Dispose();
            return false;
        }

        private static void saveColoraPalette(string fileName, Palette palette)
        {
            var Ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Palette));
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            {
                xmlSerializer.Serialize(fs, palette, Ns);
            }
        }

        private static void saveGimpPalette(string fileName, Palette palette)
        {
            using (StreamWriter sw = new StreamWriter(fileName)) {
                sw.WriteLine("GIMP Palette");
                sw.WriteLine("Name: " + palette.Name);
                sw.WriteLine("#");
                foreach(PColor palcol in palette.Colors)
                {
                    Color col = (Color)ColorConverter.ConvertFromString(palcol.Hex);
                    sw.WriteLine(String.Format("{0,3} {1,3} {2,3} {3,3}", col.R, col.G, col.B, palcol.Name));
                }
            }
        }

        internal static bool OpenPalette(out Palette palette)
        {
            palette = new Palette();
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.DefaultExt = ".col";
            ofd.Filter = "All Formats |*.col;*.gpl|Colora Palette |*.col|GIMP Palette |*.gpl";
            ofd.Title = "Open Color Palette";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    switch (Path.GetExtension(ofd.FileName))
                    {
                        case ".gpl":
                            palette = openGimpPalette(ofd.FileName);
                            break;
                        default:
                            palette = openColoraPalette(ofd.FileName);
                            break;
                    }
                    ofd.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0} {1}", Properties.Resources.PaletteWindow_strErrorOpen, ex.Message), "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            ofd.Dispose();
            return false;
        }

        private static Palette openColoraPalette(string fileName)
        {
            Palette palette = new Palette();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Palette));
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                try
                {
                    palette = (Palette)xmlSerializer.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0} {1}", Properties.Resources.PaletteWindow_strInvalidFormat, ex.Message), "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return palette;
        }

        private static Palette openGimpPalette(string fileName)
        {
            Palette palette = new Palette();
            using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    StreamReader sr = new StreamReader(fs);
                    if (sr.ReadLine() != "GIMP Palette")
                    {
                        throw new Exception("Invalid GIMP palette file format.");
                    }
                    Match match = Regex.Match(sr.ReadLine(), "^Name: (.*)");
                    if (!match.Success)
                        throw new Exception("Invalid GIMP palette file format.");
                    palette.Name = match.Groups[1].Value;
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        string[] line = sr.ReadLine().Split(' ', '\t').Where((s) => !String.IsNullOrWhiteSpace(s)).ToArray();
                        byte r = byte.Parse(line[0]);
                        byte g = byte.Parse(line[1]);
                        byte b = byte.Parse(line[2]);
                        string name = "";
                        if (line.Length > 3)
                            name = line[3];
                        palette.Colors.Add(new PColor(r, g, b, name));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0} {1}", Properties.Resources.PaletteWindow_strInvalidFormat, ex.Message), "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return palette;
        }
    }
}
