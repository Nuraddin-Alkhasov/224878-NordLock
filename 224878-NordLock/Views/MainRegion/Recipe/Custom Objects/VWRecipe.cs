using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace HMI.Views.MainRegion.Recipe
{
    public class VWRecipe
    {
        public VWRecipe(string _Class, string _Data) 
        {
            Class = _Class;
            Data = _Data;
            if (_Data != "")
            {
                Recipe = XElement.Parse(Data);

                User = GetRecipeAttribute("User");
                Version = GetRecipeAttribute("Version");
                Description = GetRecipeAttribute("Description");
                LastChanged = DateTime.FromOADate(float.Parse(GetRecipeAttribute("LastChange"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
               
                foreach (XElement x in Recipe.Descendants("V"))
                {
                    VWVariables.Add(new VWVariable(x.Attribute("Item").Value, x.Attribute("Type").Value, x.Attribute("Value").Value));
                }
            }
            else 
            {
                Recipe = null;

                User = "";
                Version = "";
                Description = "";
                LastChanged = DateTime.Now;
            }
        }
        public VWRecipe(string _Data)
        {
            Class = "";
            Data = _Data;
            if (_Data != "")
            {
                Recipe = XElement.Parse(Data);

                foreach (XElement x in Recipe.Descendants("V"))
                {
                    VWVariables.Add(new VWVariable(x.Attribute("Item").Value, x.Attribute("Type").Value, x.Attribute("Value").Value));
                }
            }
            else
            {
                Recipe = null;

                User = "";
                Version = "";
                Description = "";
                LastChanged = DateTime.Now;
            }
        }

        string Data;
        readonly string Class;
        readonly XElement Recipe;

        public ObservableCollection<VWVariable> VWVariables = new ObservableCollection<VWVariable>();


        public string User { get; set; }
        public string Version { get; set; }
       
        public string Description { get; set; }

        public DateTime LastChanged { get; set; }

        string GetRecipeAttribute(string _Attribute) 
        {
            return Recipe.Descendants(Class).Select(x => (string)x.Attribute(_Attribute).Value).First();
        }

        public string GetXML()
        {
            Recipe.Descendants("V").Remove();
            foreach (VWVariable x in VWVariables)
            {
                Recipe.Element("TBStatus").Element("Values").Add(new XElement("V", new XAttribute("Item", x.Item), new XAttribute("Type", x.Type), new XAttribute("Value", x.Value)));
            }
            Data = Recipe.ToString();
            return Data;
        }

        public override string ToString() { return Class; }
    }
}
