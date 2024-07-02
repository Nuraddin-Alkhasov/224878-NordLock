using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Reporting.WinForms;
using VisiWin.ApplicationFramework;
using Point = System.Windows.Point;
using Size = System.Drawing.Size;

namespace HMI.Reporting
{
    /// <summary>
    /// Interaktionslogik für VisiWinReportViewer.xaml
    /// </summary>
    public partial class VisiWinReportViewer : UserControl
    {
        private static readonly int MIN_PAGE = 1;

        public static readonly DependencyProperty ContentUnderlayNotiferProperty = DependencyProperty.Register("ContentUnderlayNotifer", typeof(ContentUnderlayNotifer),
            typeof(VisiWinReportViewer), new FrameworkPropertyMetadata(null, ContentUnderlayNotiferChanged));

        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(int), typeof(VisiWinReportViewer),
            new PropertyMetadata(MIN_PAGE, CurrentPageChanged));

        public static readonly DependencyProperty ExportOnlyToDefaultPathProperty = DependencyProperty.Register("ExportOnlyToDefaultPath", typeof(bool), typeof(VisiWinReportViewer),
            new PropertyMetadata(true));

        private static readonly DependencyPropertyKey PageCountModePropertyKey = DependencyProperty.RegisterReadOnly("PageCountMode", typeof(PageCountMode), typeof(VisiWinReportViewer),
            new PropertyMetadata(PageCountMode.Estimate, PageCountModeChanged));

        public static readonly DependencyProperty PageCountModeProperty = PageCountModePropertyKey.DependencyProperty;

        public static readonly DependencyProperty PageSettingsProperty = DependencyProperty.Register("PageSettings", typeof(PageSettings), typeof(VisiWinReportViewer),
            new PropertyMetadata(PageSettingsChanged));

        public static readonly DependencyProperty PrinterSettingsProperty = DependencyProperty.Register("PrinterSettings", typeof(PrinterSettings), typeof(VisiWinReportViewer),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PrintReportWithDialogProperty = DependencyProperty.Register("PrintReportWithDialog", typeof(bool), typeof(VisiWinReportViewer),
            new PropertyMetadata(true));

        private static readonly DependencyPropertyKey RenderingExtentionsPropertyKey = DependencyProperty.RegisterReadOnly("RenderingExtentions",
            typeof(IReadOnlyCollection<RenderingExtension>), typeof(VisiWinReportViewer), new PropertyMetadata(null));

        public static readonly DependencyProperty RenderingExtentionsProperty = RenderingExtentionsPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ReportConfigurationProperty = DependencyProperty.Register("ReportConfiguration", typeof(ReportConfiguration), typeof(VisiWinReportViewer),
            new PropertyMetadata(null, ReportConfigurationChangedAsync));

        private static readonly DependencyPropertyKey TotalPageNumberPropertyKey = DependencyProperty.RegisterReadOnly("TotalPageNumber", typeof(int), typeof(VisiWinReportViewer),
            new PropertyMetadata(0));

        public static readonly DependencyProperty TotalPageNumberProperty = TotalPageNumberPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey ZoomStagesPropertyKey = DependencyProperty.RegisterReadOnly("ZoomStages", typeof(ReadOnlyCollection<ZoomStage>),
            typeof(VisiWinReportViewer), new PropertyMetadata(null));

        public static readonly DependencyProperty ZoomStagesProperty = ZoomStagesPropertyKey.DependencyProperty;

        public static readonly DependencyProperty SelectedZoomStageProperty = DependencyProperty.Register("SelectedZoomStage", typeof(ZoomStage), typeof(VisiWinReportViewer),
            new PropertyMetadata(ZoomStage.PageWidth, SelectedZoomStageChanged));

        public static readonly DependencyProperty ExportInProgressProperty = DependencyProperty.Register("ExportInProgress", typeof(bool), typeof(VisiWinReportViewer),
            new PropertyMetadata(false));

        private readonly IReportingService reportingService;

        private CancellationTokenSource cancellationTokenSource;

        public VisiWinReportViewer()
        {
            this.InitializeComponent();
            if (ApplicationService.IsInDesignMode)
            {
                return;
            }

            this.reportingService = ApplicationService.GetService<IReportingService>();
            this.cancellationTokenSource = null;

            //DependencyProperties initialisieren
            this.PrinterSettings = this.ReportViewer.PrinterSettings;
            this.RenderingExtentions = new ReadOnlyCollection<RenderingExtension>(this.ReportViewer.LocalReport.ListRenderingExtensions());
            this.ZoomStages =
                new ReadOnlyCollection<ZoomStage>(new[]
                                                      {
                                                          ZoomStage.FullPage, ZoomStage.PageWidth, ZoomStage.p500, ZoomStage.p200, ZoomStage.p150, ZoomStage.p100, ZoomStage.p75, ZoomStage.p50,
                                                          ZoomStage.p25
                                                      });

            //EventHandler registrieren
            this.ReportViewer.RenderingComplete += (s, e) => this.UpdateTotalPage();
            this.ReportViewer.PageSettingsChanged += this.reportViewer_PageSettingsChanged;
            this.Loaded += this.VisiWinReportViewer_LoadedAsync;
            this.ReportViewer.PageNavigation += this.reportViewer_PageNavigation;

            this.ReportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            this.UpdateTotalPage();
        }

        /// <summary>
        /// Dieses Objekt signalisiert dem ReportViewer wann er sich verstecken muss und ein Dummy anzeigen muss.
        /// </summary>
        public ContentUnderlayNotifer ContentUnderlayNotifer
        {
            get { return (ContentUnderlayNotifer)this.GetValue(ContentUnderlayNotiferProperty); }
            set { this.SetValue(ContentUnderlayNotiferProperty, value); }
        }

        /// <summary>
        /// Aktuelle Seitenzahl der dargestellten Reportseite.
        /// </summary>
        public int CurrentPage
        {
            get { return (int)this.GetValue(CurrentPageProperty); }
            set { this.SetValue(CurrentPageProperty, value); }
        }

        public bool ExportInProgress
        {
            get { return (bool)this.GetValue(ExportInProgressProperty); }
            set { this.SetValue(ExportInProgressProperty, value); }
        }

        public bool ExportOnlyToDefaultPath
        {
            get { return (bool)this.GetValue(ExportOnlyToDefaultPathProperty); }
            set { this.SetValue(ExportOnlyToDefaultPathProperty, value); }
        }

        /// <summary>
        /// Gibt an, ob die Gesamtseitenzahl als geschätzter oder tatsächlicher Wert interpretiert werden soll.
        /// Wenn der PageCountMode PageCountMode.Estimate ist, ist ein Fragezeichen in der Toolbar neben der Gesamtseitenzahl
        /// sichtbar.
        /// </summary>
        public PageCountMode PageCountMode
        {
            get { return (PageCountMode)this.GetValue(PageCountModeProperty); }
            private set { this.SetValue(PageCountModePropertyKey, value); }
        }

        public PageSettings PageSettings
        {
            get { return (PageSettings)this.GetValue(PageSettingsProperty); }
            set { this.SetValue(PageSettingsProperty, value); }
        }

        /// <summary>
        /// Bestimmt, mit welchen Einstellungen der Report gedruckt wird, wenn kein Dialog geöffnet wird.
        /// </summary>
        public PrinterSettings PrinterSettings
        {
            get { return (PrinterSettings)this.GetValue(PrinterSettingsProperty); }
            set { this.SetValue(PrinterSettingsProperty, value); }
        }

        /// <summary>
        /// Bestimmt, ob beim Druck aus den Print-Button ein Dialog geöffnet werden soll.
        /// Standardwert ist true.
        /// </summary>
        public bool PrintReportWithDialog
        {
            get { return (bool)this.GetValue(PrintReportWithDialogProperty); }
            set { this.SetValue(PrintReportWithDialogProperty, value); }
        }

        /// <summary>
        /// Liste aller unterstützen RenderingExtensions.
        /// </summary>
        public IReadOnlyCollection<RenderingExtension> RenderingExtentions
        {
            get { return (IReadOnlyCollection<RenderingExtension>)this.GetValue(RenderingExtentionsProperty); }
            private set { this.SetValue(RenderingExtentionsPropertyKey, value); }
        }

        /// <summary>
        /// Bestimmt, welcher Report angezeigt werden soll.
        /// </summary>
        public ReportConfiguration ReportConfiguration
        {
            get { return (ReportConfiguration)this.GetValue(ReportConfigurationProperty); }
            set { this.SetValue(ReportConfigurationProperty, value); }
        }

        public ZoomStage SelectedZoomStage
        {
            get { return (ZoomStage)this.GetValue(SelectedZoomStageProperty); }
            set { this.SetValue(SelectedZoomStageProperty, value); }
        }

        /// <summary>
        /// Die Gesamtseitenzahl des derzeit angezeigten Berichts.
        /// Diese Eigenschaft ist readonly.
        /// </summary>
        public int TotalPageNumber
        {
            get { return (int)this.GetValue(TotalPageNumberProperty); }
            private set { this.SetValue(TotalPageNumberPropertyKey, value); }
        }

        /// <summary>
        /// Liste aller verfügbaren Zoomstufen.
        /// </summary>
        public ReadOnlyCollection<ZoomStage> ZoomStages
        {
            get { return (ReadOnlyCollection<ZoomStage>)this.GetValue(ZoomStagesProperty); }
            private set { this.SetValue(ZoomStagesPropertyKey, value); }
        }

        private static void ContentUnderlayNotiferChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as VisiWinReportViewer;
            var old = e.OldValue as ContentUnderlayNotifer;
            var @new = e.NewValue as ContentUnderlayNotifer;

            if (view == null)
            {
                return;
            }

            if (old != null)
            {
                old.ContentIsCovered -= view.ContentIsCovered;
            }

            if (@new != null)
            {
                @new.ContentIsCovered += view.ContentIsCovered;
            }
        }

        private static void CurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as VisiWinReportViewer;
            var @new = (int)e.NewValue;

            if (view == null)
            {
                return;
            }

            view.UpdateTotalPage();

            if (@new < 1)
            {
                view.CurrentPage = 1;
            }
            else
            {
                if (view.PageCountMode == PageCountMode.Actual)
                {
                    if (0 < view.TotalPageNumber)
                    {
                        view.ReportViewer.CurrentPage = Math.Max(Math.Min(view.TotalPageNumber, @new), 1);
                    }
                }
                else
                {
                    view.ReportViewer.CurrentPage = @new;
                }
            }

            if (view.ReportViewer.CurrentPage != @new)
            {
                view.CurrentPage = view.ReportViewer.CurrentPage;
            }

            view.UpdateTotalPage();
        }

        private static void PageCountModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as VisiWinReportViewer;
            var newMode = (PageCountMode)e.NewValue;

            if (view == null)
            {
                return;
            }

            //switch (newMode)
            //{
            //    case PageCountMode.Estimate:
            //        view.EstimatedPageNumber.Visibility = Visibility.Visible;
            //        break;
            //    case PageCountMode.Actual:
            //        view.EstimatedPageNumber.Visibility = Visibility.Collapsed;
            //        break;
            //}
        }

        private static void PageSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as VisiWinReportViewer;
            var newSettings = e.NewValue as PageSettings;

            if (view == null || newSettings == null)
            {
                return;
            }

            view.ReportViewer.SetPageSettings(newSettings);
        }

        private static async void ReportConfigurationChangedAsync(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as VisiWinReportViewer;
            var config = e.NewValue as ReportConfiguration;

            if (view == null || config == null)
            {
                if (config == null && view != null)
                {
                    view.ReportViewContainer.Visibility = Visibility.Hidden;
                    view.LoadingMessageContainer.Visibility = Visibility.Visible;
                }
                return;
            }

            await view.reportingService.ConfigureReportAsync(view.ReportViewer.LocalReport, config);

            view.ReportViewer.RefreshReport();

            view.ReportViewContainer.Visibility = Visibility.Visible;
            view.LoadingMessageContainer.Visibility = Visibility.Collapsed;
        }

        private static void SelectedZoomStageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as VisiWinReportViewer;
            var oldZoom = (ZoomStage)e.OldValue;
            var newZoom = (ZoomStage)e.NewValue;

            if (oldZoom != newZoom && view != null)
            {
                if (newZoom == ZoomStage.FullPage)
                {
                    view.ReportViewer.ZoomMode = ZoomMode.FullPage;
                }
                else if (newZoom == ZoomStage.PageWidth)
                {
                    view.ReportViewer.ZoomMode = ZoomMode.PageWidth;
                }
                else
                {
                    view.ReportViewer.ZoomMode = ZoomMode.Percent;
                    view.ReportViewer.ZoomPercent = (int)newZoom;
                }
            }
        }

        private void ButtonGoPageBack_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentPage--;
        }

        private void ButtonGoPageForward_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentPage++;
        }

        private void ButtonGoToEnd_Click(object sender, RoutedEventArgs e)
        {
            //Versuchen, die CurrentPage auf die letzte Seite zu setzen
            this.ReportViewer.CurrentPage = this.UpdateTotalPage();
            //Rückschreiben auf welcher Seite der ReportViewer nun ist.
            this.CurrentPage = this.ReportViewer.CurrentPage;
            this.UpdateTotalPage();
        }

        private void ButtonGoToStart_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentPage = MIN_PAGE;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Versteckt den Dummy oder den ReportViewer.
        /// Dieses Methode wird aufgerufen wenn der ContentUnderlayNotifer das Event ContentIsCovered auslöst.
        /// </summary>
        /// <param name="showDummy">Soll der Dummy angezeigt werden?</param>
        private void ContentIsCovered(bool showDummy)
        {
            if (showDummy)
            {
                this.ShowDummy();
            }
            else
            {
                this.HideDummy();
            }
        }

      

        /// <summary>
        /// Versteckt den Dummy und zeigt wieder den ReportViewer an.
        /// </summary>
        private void HideDummy()
        {
            if (!this.IsVisible)
            {
                return;
            }
            this.Hoster.Visibility = Visibility.Visible;
            this.Dummy.Visibility = Visibility.Hidden;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var searchState = this.ReportViewer.SearchState;
            if (searchState != null)
            {
                this.ReportViewer.FindNext();
            }
        }

        private void PageSettings_Click(object sender, RoutedEventArgs e)
        {
            this.ReportViewer.PageSetupDialog();
            this.UpdateTotalPage();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.PrintReportWithDialog)
            {
                this.ReportViewer.PrintDialog(this.PrinterSettings);
            }
            else
            {
                var ps = this.PageSettings;
                if (ps == null)
                {
                    ps = new PageSettings();
                    var s = this.ReportViewer.LocalReport.GetDefaultPageSettings();
                    ps.PaperSize = s.PaperSize;
                    ps.Margins = s.Margins;
                    ps.Landscape = s.IsLandscape;
                }
                ps.PrinterSettings = this.ReportViewer.PrinterSettings;
                var print = new ReportPrintDocument(this.ReportViewer.LocalReport, ps);
                print.Print();
            }
        }

        private void PrintLayout_Click(object sender, RoutedEventArgs e)
        {
            this.ReportViewer.SetDisplayMode(this.ReportViewer.DisplayMode == DisplayMode.Normal ? DisplayMode.PrintLayout : DisplayMode.Normal);
            this.UpdateTotalPage();
        }

        /// <summary>
        /// Tritt ein, wenn der User durch Scrollen die Seite wechselt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportViewer_PageNavigation(object sender, PageNavigationEventArgs e)
        {
            this.CurrentPage = e.NewPage;
            this.UpdateTotalPage();
        }

        private void reportViewer_PageSettingsChanged(object sender, EventArgs e)
        {
            this.ReportViewer.PageSettingsChanged -= this.reportViewer_PageSettingsChanged;
            this.PageSettings = this.ReportViewer.GetPageSettings();
            this.ReportViewer.PageSettingsChanged += this.reportViewer_PageSettingsChanged;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExportOnlyToDefaultPath)
            {
                if (this.ReportViewer.LocalReport.IsReadyForRendering)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        try
                        {
                            this.cancellationTokenSource = cts;
                            this.ExportInProgress = true;
                            await this.reportingService.ExportReportAsync(this.ReportConfiguration, RenderFormat.PDF, "ReportViewer", cts.Token);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            this.ExportInProgress = false;
                            this.cancellationTokenSource = null;
                        }
                    }
                }
            }
            else
            {
                var re = this.SaveContextComboBox.SelectedItem as RenderingExtension;
                if (re == null)
                {
                    return;
                }
                this.ReportViewer.ExportDialog(re);
            }
        }

        /// <summary>
        /// Versteckt den ReportViewer und zeigt einen Dummy mit einem Screenshot vom ReportViewer an.
        /// </summary>
        private void ShowDummy()
        {
            if (!this.IsVisible)
            {
                return;
            }
            this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    var source = this.TakeScreenshot();

                    this.Dummy.Source = source;

                    this.Dummy.Visibility = Visibility.Visible;
                    this.Hoster.Visibility = Visibility.Hidden;
                }));
        }

        /// <summary>
        /// Erzeugt einen Screenshot vom ReportViewer.
        /// </summary>
        /// <returns>Der Screenshot</returns>
        private ImageSource TakeScreenshot()
        {
            var topLeftCorner = this.Hoster.PointToScreen(new Point(0, 0));
            var topLeftGdiPoint = new System.Drawing.Point((int)topLeftCorner.X, (int)topLeftCorner.Y);
            var size = new Size((int)this.Hoster.ActualWidth, (int)this.Hoster.ActualHeight);

            var screenShot = new Bitmap((int)this.Hoster.ActualWidth, (int)this.Hoster.ActualHeight);

            using (var graphics = Graphics.FromImage(screenShot))
            {
                graphics.CopyFromScreen(topLeftGdiPoint, new System.Drawing.Point(), size, CopyPixelOperation.SourceCopy);
            }

            ImageSource img = Imaging.CreateBitmapSourceFromHBitmap(screenShot.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return img;
        }

        /// <summary>
        /// Aktualisiert die Gesamtseitenanzahl sowie den PageCountMode.
        /// </summary>
        /// <returns>
        /// Falls der PageCountMode Actual ist, wird die Gesamtseitenzahl zurückgegeben, andernfalls wird int.MaxValue
        /// zurückgegeben.
        /// </returns>
        private int UpdateTotalPage()
        {
            PageCountMode mode;
            var tmp = this.ReportViewer.GetTotalPages(out mode);
            this.PageCountMode = mode;
            this.TotalPageNumber = tmp;
            return mode == PageCountMode.Actual ? tmp : int.MaxValue;
        }

      

        /// <summary>
        /// Zurücksetzen und Aktualisieren der View sowie des ReportViewers und seines LocalReports wenn die View geladen wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void VisiWinReportViewer_LoadedAsync(object sender, RoutedEventArgs e)
        {
            this.ReportViewer.Reset();
            await this.reportingService.ConfigureReportAsync(this.ReportViewer.LocalReport, this.ReportConfiguration);
            this.ReportViewer.RefreshReport();
            this.ReportViewer.Refresh();
        }
    }

    #region Helper Klassen

    [ValueConversion(typeof(bool), typeof(bool))]
    internal class BooleanNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (bool)value;
            return !v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValueConverterGroup : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ChainConversion(0, 1, value, targetType, parameter, culture,
                (converter, _value, _targetType, _parameter, _culture) => converter.Convert(_value, _targetType, _parameter, _culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ChainConversion(this.Count - 1, -1, value, targetType, parameter, culture,
                (converter, _value, _targetType, _parameter, _culture) => converter.Convert(_value, _targetType, _parameter, _culture));
        }

        private object ChainConversion(int seed, int offSet, object value, Type targetType, object parameter, CultureInfo culture,
                                       Func<IValueConverter, object, Type, object, CultureInfo, object> func)
        {
            while (0 <= seed && seed < this.Count)
            {
                value = func(this[seed], value, targetType, parameter, culture);
                seed += offSet;
            }
            return value;
        }
    }

    public enum ZoomStage
    {
        FullPage,
        PageWidth,
        p500 = 500,
        p200 = 200,
        p150 = 150,
        p100 = 100,
        p75 = 75,
        p50 = 50,
        p25 = 25
    }

    internal class ZoomStageToLocalizableStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var z = (ZoomStage)value;
            switch (z)
            {
                case ZoomStage.FullPage:
                    return "@Reporting.ReportViewer.FullPage";
                case ZoomStage.PageWidth:
                    return "@Reporting.ReportViewer.PageWidth";
                case ZoomStage.p500:
                    return "@Reporting.ReportViewer.p500";
                case ZoomStage.p200:
                    return "@Reporting.ReportViewer.p200";
                case ZoomStage.p150:
                    return "@Reporting.ReportViewer.p150";
                case ZoomStage.p100:
                    return "@Reporting.ReportViewer.p100";
                case ZoomStage.p75:
                    return "@Reporting.ReportViewer.p75";
                case ZoomStage.p50:
                    return "@Reporting.ReportViewer.p50";
                case ZoomStage.p25:
                    return "@Reporting.ReportViewer.p25";
                default:
                    return "Invalid Value";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion Helper Klassen
}