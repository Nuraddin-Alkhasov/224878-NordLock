using System;
using System.Globalization;
using System.Windows.Data;

namespace HMI.Reporting
{
    /// <summary>
    /// Enum für die möglichen Exporttypen des IReportingServices
    /// </summary>
    public enum RenderFormat
    {
        EXCEL, //Excel 2003     | *.xls
        EXCELOPENXML, //Excel   | *.xlsx
        IMAGE, //Tiff-Datei     | *.TIF
        PDF, //                 | *.pdf
        WORD, //Word 2003       | *.doc
        WORDOPENXML //Word      | *.docx
    }

    /// <summary>
    /// Konverter um ein RenderFormat-Wert in einen Dateikürzel-String
    /// oder in eine RenderFormat-String umzuwandeln
    /// </summary>
    [ValueConversion(typeof(RenderFormat), typeof(string))]
    public class RenderModeConverter : IValueConverter
    {
        /// <summary>
        /// Gibt einen FileExtension- oder RenderExtensionFormatstring zurück.
        /// Wenn beim Parameter "parameter" true eingegeben wird, wird ein FileExtension-String erzeugt,
        /// bei false ein RenderExtension-Formatstring.
        /// </summary>
        /// <param name="value">RenderFormat-Objekt</param>
        /// <param name="targetType">wird ignoriert (kann also null sein)</param>
        /// <param name="parameter">Soll ein FileExtension-String erzeugt werden?</param>
        /// <param name="culture">wird ignoriert (kann also null sein)</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileExtension = parameter is bool ? (bool)parameter : true;
            var renderMode = value is RenderFormat ? (RenderFormat)value : RenderFormat.PDF;

            if (fileExtension)
            {
                switch (renderMode)
                {
                    case RenderFormat.EXCEL:
                        return ".xls";
                    case RenderFormat.EXCELOPENXML:
                        return ".xlsx";
                    case RenderFormat.IMAGE:
                        return ".TIF";
                    case RenderFormat.PDF:
                        return ".pdf";
                    case RenderFormat.WORD:
                        return ".doc";
                    case RenderFormat.WORDOPENXML:
                        return ".docx";
                    default:
                        return ".pdf";
                }
            }
            switch (renderMode)
            {
                case RenderFormat.EXCEL:
                    return "EXCEL";
                case RenderFormat.EXCELOPENXML:
                    return "EXCELOPENXML";
                case RenderFormat.IMAGE:
                    return "IMAGE";
                case RenderFormat.PDF:
                    return "PDF";
                case RenderFormat.WORD:
                    return "WORD";
                case RenderFormat.WORDOPENXML:
                    return "WORDOPENXML";
                default:
                    return "PDF";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileExtension = parameter is bool ? (bool)parameter : true;
            var str = value as string;

            if (fileExtension)
            {
                switch (str)
                {
                    case ".xls":
                        return RenderFormat.EXCEL;
                    case ".xlsx":
                        return RenderFormat.EXCELOPENXML;
                    case ".TIF":
                        return RenderFormat.IMAGE;
                    case ".pdf":
                        return RenderFormat.PDF;
                    case ".doc":
                        return RenderFormat.WORD;
                    case ".docx":
                        return RenderFormat.WORDOPENXML;
                    default:
                        return RenderFormat.PDF;
                }
            }
            switch (str)
            {
                case "EXCEL":
                    return RenderFormat.EXCEL;
                case "EXCELOPENXML":
                    return RenderFormat.EXCELOPENXML;
                case "IMAGE":
                    return RenderFormat.IMAGE;
                case "PDF":
                    return RenderFormat.PDF;
                case "WORD":
                    return RenderFormat.WORD;
                case "WORDOPENXML":
                    return RenderFormat.WORDOPENXML;
                default:
                    return RenderFormat.PDF;
            }
        }
    }
}