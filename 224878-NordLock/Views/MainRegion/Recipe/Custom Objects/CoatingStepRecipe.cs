using System.ComponentModel;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
    public class CoatingStepRecipe 
    {
        public CoatingStepRecipe()
        {
            Id = -1;
            Name = "";
            Class_Id = 0;
            Class = ApplicationService.GetService<IRecipeService>().GetRecipeClass(ApplicationService.GetService<IRecipeService>().RecipeClassNames[(int)Class_Id]);
            Type_Id = -1;
            Type = "";
            Data = "";           
        }

        public IRecipeClass Class { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public long Class_Id { get; set; }
        long _Type_Id = 0;
        public long Type_Id
        {
            get { return _Type_Id; }
            set
            {
                _Type_Id = value;
                Type = GetType();
            }
        }

        string _Type = "";
        public string Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                Symbol = GetSymbol();
            }
        }

        public string Symbol { get; set; }
        string _Data="";
        public string Data 
        {
            get { return _Data; }
            set 
            {
                _Data = value;
                VWR = new VWRecipe(Class.Name, value);
            } 
        }

        public VWRecipe VWR { get; set; }
       
        string GetSymbol() 
        {
            switch (Type_Id) 
            {
                case 1: return "R_Dip";
                case 2: return "R_Spin";
                case 3: return "R_Tilt";
                default: return "R_No";
            }
        }

        string GetType()
        {
            switch (Type_Id)
            {
                case 1: return "Dip";
                case 2: return "Spin";
                case 3: return "Tilt";
                default: return "-";
            }
        }
        public override string ToString() { return Id+"-"+Name; }
    }
}
