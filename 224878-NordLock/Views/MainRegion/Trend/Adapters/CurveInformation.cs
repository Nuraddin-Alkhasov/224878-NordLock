using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Language;
using VisiWin.Trend;

namespace HMI
{
    public interface ICurveInformation
    {
        object Curve { get; set; }
    }

    public abstract class CurveInformation<T> : ICurveInformation, INotifyPropertyChanged where T : ICurve
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public T Curve { get; set; }

        object ICurveInformation.Curve
        {
            get { return this.Curve; }
            set { this.Curve = (T)value; }
        }

        private string markedXValues;
        public string MarkedXValues
        {
            get { return this.markedXValues; }
            set
            {
                if (this.markedXValues != value)
                {
                    this.markedXValues = value;
                    OnPropertyChanged();
                }
            }
        }

        private string markedYValues;
        public string MarkedYValues
        {
            get { return this.markedYValues; }
            set
            {
                if (this.markedYValues != value)
                {
                    this.markedYValues = value;
                    OnPropertyChanged();
                }
            }
        }

        private Brush lineBrush;
        public Brush LineBrush
        {
            get { return this.lineBrush; }
            set
            {
                if (this.lineBrush != value)
                {
                    this.lineBrush = value;
                    OnPropertyChanged();
                }
            }
        }

    }

    public class TrendCurveInformation : CurveInformation<TrendCurve2>
    {
        private static readonly ILanguageService languageService = ApplicationService.GetService<ILanguageService>();
        private static readonly ITrendService trendService = ApplicationService.GetService<ITrendService>();


        public TrendCurveInformation()
        {
            languageService.LanguageChanged += this.LanguageServiceOnLanguageChanged;
        }


        private string archiveName;
        public string ArchiveName
        {
            get { return this.archiveName; }
            set
            {
                if (this.archiveName != value)
                {
                    this.archiveName = value;
                    SetLocalizedArchiveName();
                    OnPropertyChanged();
                }
            }
        }

        private string localizedArchiveName;
        public string LocalizedArchiveName
        {
            get { return this.localizedArchiveName; }
            set
            {
                if (this.localizedArchiveName != value)
                {
                    this.localizedArchiveName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string trendName;
        public string TrendName
        {
            get { return this.trendName; }
            set
            {
                if (this.trendName != value)
                {
                    this.trendName = value;
                    SetLocalizedTrendName();
                    OnPropertyChanged();
                }
            }
        }

        private string localizedTrendName;
        public string LocalizedTrendName
        {
            get { return this.localizedTrendName; }
            set
            {
                if (this.localizedTrendName != value)
                {
                    this.localizedTrendName = value;
                    OnPropertyChanged();
                }
            }
        }


        private void LanguageServiceOnLanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            this.SetLocalizedArchiveName();
            this.SetLocalizedTrendName();
        }

        private void SetLocalizedArchiveName()
        {
            string name = trendService.GetArchive(this.ArchiveName)?.Text ?? string.Empty;
            this.LocalizedArchiveName = string.IsNullOrEmpty(name) ? this.ArchiveName : name;
        }

        private void SetLocalizedTrendName()
        {
            string name = trendService.GetTrend(this.ArchiveName, this.TrendName)?.Text ?? string.Empty;
            this.LocalizedTrendName = string.IsNullOrEmpty(name) ? this.TrendName : name;
        }

    }

    public class XYCurveInformation : CurveInformation<IXYCurve>
    {
    }

    public abstract class CurveInformationCollection<T> : ObservableCollection<T> where T : ICurveInformation
    {
        public T this[ICurve key]
        {
            get { return this.Items.FirstOrDefault(item => item.Curve.Equals(key)); }
        }
    }

    public sealed class TrendCurveInformationCollection : CurveInformationCollection<TrendCurveInformation>
    {
        public void Add(TrendCurve2 curve)
        {
            var info = new TrendCurveInformation { Curve = curve, ArchiveName = curve.ArchiveName, TrendName = curve.TrendName };
            this.Add(info);
        }
    }

    public sealed class XYCurveInformationCollection : CurveInformationCollection<XYCurveInformation>
    {
        public void Add(IXYCurve curve)
        {
            var info = new XYCurveInformation { Curve = curve };
            this.Add(info);
        }
    }
}
