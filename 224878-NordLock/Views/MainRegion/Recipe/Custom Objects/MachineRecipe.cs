
using System;
using System.Collections.ObjectModel;

namespace HMI.Views.MainRegion.Recipe
{
    public class MachineRecipe
    {
        public MachineRecipe()
        {
            Id = -1;
            Name = "";
            Description = "";
            Article = new ArticleRecipe();
            CoatingLayers = new ObservableCollection<CoatingRecipe>();
            CoatingLayers.Add(new CoatingRecipe());
            Version = "1.0";
            LastChanged = DateTime.Now;
            User = "";
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ArticleRecipe Article { get; set; }

        public ObservableCollection<CoatingRecipe> CoatingLayers;
       
        public string Version { get; set; }
        public DateTime LastChanged { get; set; }
        public string User { get; set; }

        public readonly string Symbol = "Machine_Recipe";

        public override string ToString() { return Id + "-" + Name; }
    }
}
