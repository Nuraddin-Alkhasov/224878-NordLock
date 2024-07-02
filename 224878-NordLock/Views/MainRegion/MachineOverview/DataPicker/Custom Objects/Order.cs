using HMI.Views.MainRegion.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{
    public class Order
    {
        public Order()
        {
            Id = -1;
            Data_1 = "";
            Data_2 = "";
            Data_3 = "";
            MR = new MachineRecipe();
            User = "";       
        }

        public Order(Order T)
        {
            Id = T.Id;
            Data_1 = T.Data_1;
            Data_2 = T.Data_2;
            Data_3 = T.Data_3;
            MR = T.MR;
            User = T.User;
        }

        public long Id { get; set; }
        public string Data_1 { get; set; }
        public string Data_2 { get; set; }
        public string Data_3 { get; set; }
        public MachineRecipe MR { get; set; }
        public string User { get; set; }
        public override string ToString() { return MR.Name; }
    }
}
