namespace HMI.Views.MainRegion.Recipe
{
    public class VWVariable
    {
        public VWVariable(object _s, object _t, object _v)
        {
            Item = _s;
            Type = _t;
            Value = _v;
        }
        public object Item { get; set; }
        public object Type { get; set; }
        public object Value { get; set; }
        public override string ToString() { return Item.ToString(); }
    }
}
