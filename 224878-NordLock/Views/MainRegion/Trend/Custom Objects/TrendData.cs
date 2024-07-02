
namespace HMI.Views.MainRegion
{
    public class TrendData
    {

        public TrendData()
        {
            ArchiveName = "";
            TrendName_1 = "";
            CurveTag_1 = "";
            TrendName_2 = "";
            CurveTag_2 = "";
            Header = "";
            Min = 0;
            Max = 100;
        }

        public string ArchiveName { set; get; }
        public string TrendName_1 { set; get; }
        public string CurveTag_1 { set; get; }
        public string TrendName_2 { set; get; }
        public string CurveTag_2 { set; get; }
        public string Header { set; get; }
        public int Min { set; get; }
        public int Max { set; get; }

        public string BackViewName { set; get; }
    }
}
