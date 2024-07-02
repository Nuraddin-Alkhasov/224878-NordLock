
namespace HMI.Views.TouchpadRegion
{
    public class WaitData 
    {
        public string LocalizableText;
        public int Type;

        public WaitData(int _Type, string _LocalizableText)
        {
            Type = _Type;
            LocalizableText = _LocalizableText;
        }
    }
}
