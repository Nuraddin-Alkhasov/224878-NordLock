using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisiWin.ApplicationFramework;

namespace HMI.Reporting
{
    /// <summary>
    /// Erweiterungen für die Klasse String
    /// </summary>
    internal static class StringExtension
    {
        /// <summary>
        /// Versucht den LocalizableText mithilfe des VisiWin.ApplicationService aufzulösen.
        /// Gelingt dies, so wird der lokalisierte String zurückgegeben, andernfalls wird der Placeholder zurückgegeben.
        /// </summary>
        /// <param name="localizableText">Textbezeichner</param>
        /// <param name="placeholder">Platzhalter falls der Textbezeichner nicht aufgelöst werden kann.</param>
        /// <returns></returns>
        public static string TryToLocalize(this string localizableText, string placeholder)
        {
            var x = ApplicationService.GetText(localizableText);
            if (!Regex.IsMatch(x, @"^\@(.{1,}\.)?(LocString)$".Replace("LocString", Regex.Escape(localizableText.Replace("@", "")))))
            {
                return x;
            }
            return placeholder;
        }
    }

    /// <summary>
    /// Erweiterungsklasse für die Klasse Imaging.BitmapImage
    /// </summary>
    internal static class BitmapImageExtension
    {
        /// <summary>
        /// Erzeugt ein byte[] mit den Informationen des BitmapImage, kodiert als PNG
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] BitmapToByteArray(this BitmapImage img)
        {
            byte[] data;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img));
            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
    }

    /// <summary>
    /// Erweiterungsklasse für die Klasse Media.Color
    /// </summary>
    internal static class ColorExtension
    {
        /// <summary>
        /// Erzeugt einen Hex-Code mit R,G und B Kanal.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToRgbHexCode(this Color color)
        {
            return string.Format("#{0}{1}{2}", color.R.ToString("X2"), color.G.ToString("X2"), color.B.ToString("X2"));
        }
    }
}