
using System;
using System.Collections.ObjectModel;

namespace HMI.Views.MainRegion.Recipe
{
    public class CoatingRecipe
    {
        public CoatingRecipe()
        {

            Id = -1;
            Name = "";
            Description = "";
            Paint_Id = -1;
            CoatingSteps = new ObservableCollection<CoatingStepRecipe>();
            Version = "1.0";
            LastChanged = DateTime.Now;
            User = "";
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Paint_Id { get; set; }

        public ObservableCollection<CoatingStepRecipe> CoatingSteps;
       
        public string Version { get; set; }
        public DateTime LastChanged { get; set; }
        public string User { get; set; }

        public readonly string Symbol = "R_Coat";

        public override string ToString() { return Id + "-" + Name; }

    }
}
