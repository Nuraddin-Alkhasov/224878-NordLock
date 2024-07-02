namespace HMI.Views.MainRegion.Recipe.Custom_Objects
{
    class Barcode
    {
        public Barcode()
        {
            Id = -1;
            BC = "";
            MR_Name = "";
            MR_Id = -1;
        }
        public long Id { set; get; }
        public string BC { set; get; }
        public string MR_Name { set; get; }
        public long MR_Id { set; get; }

    }
}
