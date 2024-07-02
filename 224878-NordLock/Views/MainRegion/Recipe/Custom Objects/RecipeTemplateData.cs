using System.ComponentModel;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
    public class RecipeTemplateData
    {
        public RecipeTemplateData()
        {
            Id = 0;
            Name = "";
            Description ="";
            Recipe = null;
            Symbol = "R_No";
        }
        public RecipeTemplateData(int _id)
        {
            Id = _id;
            Name = "";
            Description = "";
            Recipe = null;
            Symbol = "R_No";
        }
        public RecipeTemplateData(int _id, string _name, string _descr, object _recipe, string _symbol)
        {
            Id = _id;
            Name = _name;
            Description = _descr;
            Recipe = _recipe;
            Symbol = _symbol;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Symbol { get; set; }

        public object Recipe { set; get; }
    }
}
