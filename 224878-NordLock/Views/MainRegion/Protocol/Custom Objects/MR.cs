using HMI.Views.MainRegion.Recipe;
using System;
using System.Collections.ObjectModel;

namespace HMI.Views.MainRegion.Protocol
{
    public class MR
    {

        public MR()
        {
            Id = -1;

            Name = " - - - ";
            Description = " - - - ";
            Article_Id = -1;
            Article_Name = " - - - ";
            C1_Id = -1;
            C1_Name = " - - - ";
            C2_Id = -1;
            C2_Name = " - - - ";
            C3_Id = -1;
            C3_Name = " - - - ";
            C4_Id = -1;
            C4_Name = " - - - ";
        }

        public long Id { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public long Article_Id { set; get; }
        public string Article_Name { set; get; }
        public long C1_Id { set; get; }
        public string C1_Name { set; get; }
        public long C2_Id { set; get; }
        public string C2_Name { set; get; }
        public long C3_Id { set; get; }
        public string C3_Name { set; get; }
        public long C4_Id { set; get; }
        public string C4_Name { set; get; }

        public string[] Coatings { set; get; }

    }
}
