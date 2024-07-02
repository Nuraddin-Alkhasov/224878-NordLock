
using HMI.Module;
using System;
using System.Data;

namespace HMI.Views.MainRegion.Recipe
{
    public class RecipeManager 
    {

        public RecipeManager(string _name)
        {
            Name = _name;
            Class = 0;
        }
        public RecipeManager(string _name, long _class)
        {
            Name = _name;
            Class = _class;
        }

        public string Name { get; set; }
        public long Class { get; set; }
        public long IsMRRecipeExisting()
        {
            DataTable DT = (new LocalDBAdapter("SELECT Id " +
                                                  "FROM Recipes_MR " +
                                                  "WHERE Name='" + Name + "'; ")
                          ).DB_Output();
            if (DT.Rows.Count > 0)
            {
                return Convert.ToInt64(DT.Rows[0]["Id"]);
            }
            else
            {
                return -1;
            }
        }
        public long IsArticleRecipeExisting()
        {
            DataTable DT = (new LocalDBAdapter("SELECT Id " +
                                                   "FROM Recipes_Article_VW " +
                                                   "WHERE Class_Id = " + Class + " AND Name='" + Name + "'; ")
                           ).DB_Output();
            if (DT.Rows.Count > 0)
            {
                return Convert.ToInt64(DT.Rows[0]["Id"]);
            }
            else
            {
                return -1;
            }
        }

        public long IsCoatingRecipeExisting()
        {
            DataTable DT = (new LocalDBAdapter("SELECT Id " +
                                                   "FROM Recipes_Coating " +
                                                   "WHERE Name='" + Name + "'; ")
                           ).DB_Output();
            if (DT.Rows.Count > 0)
            {
                return Convert.ToInt64(DT.Rows[0]["Id"]);
            }
            else
            {
                return -1;
            }

        }

        public long IsCoatingStepRecipeExisting()
        {
            DataTable DT = (new LocalDBAdapter("SELECT Id " +
                                                   "FROM Recipes_CoatingStep_VW " +
                                                   "WHERE Class_Id = " + Class + " AND Name='" + Name + "'; ")
                           ).DB_Output();
            if (DT.Rows.Count > 0)
            {
                return Convert.ToInt64(DT.Rows[0]["Id"]);
            }
            else
            {
                return -1;
            }
        }
    }
}
