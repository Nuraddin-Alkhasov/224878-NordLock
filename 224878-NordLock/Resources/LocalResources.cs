using System;
using System.Windows;

namespace HMI.Resources
{
    public class LocalResources
    {
        public LocalResources() 
        {
            Paths = new ProjectResourcePaths() 
            {
                Project = new ActualPath(),
                Backup = new BackupPath()
            };

            Paths.LoadingGif = Paths.Project.Path + "\\Resources\\Images\\Loading.gif";
            Paths.localDB = Paths.Project.Path + "\\DB\\localDB.db";
            Paths.WorldMap = Paths.Project.Path + "\\Resources\\Maps\\World.xml"; 
        }
        public ProjectResourcePaths Paths;

        public class ProjectResourcePaths
        {
            public string LoadingGif { get; set; }
            public string localDB { get; set; }
            public string WorldMap { get; set; }
            public BackupPath Backup { get; set; }
            public ActualPath Project { get; set; }


        }
        public class BackupPath
        {
            public BackupPath() 
            {
                Path = "D:\\FP-HMI-Backup\\";
                string FolderName = DateTime.Now.Year.ToString() + "-" +
                   (DateTime.Now.Month.ToString().Length == 2 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString()) + "-" +
                   (DateTime.Now.Day.ToString().Length == 2 ? DateTime.Now.Day.ToString() : "0" + DateTime.Now.Day.ToString());
                ToDayPath = "D:\\FP-HMI-Backup\\" + FolderName;
                Alarms = ToDayPath + "\\Alarms";
                Archive = ToDayPath + "\\Archive";
                Logging = ToDayPath + "\\Logging";
                Recipes = ToDayPath + "\\Recipes";
                DB = ToDayPath + "\\DB";
                QData = ToDayPath + "\\QData";
            }
            public string Path { get; }
            public string ToDayPath { get; }
            public string Alarms { get; }
            public string Archive { get; }
            public string Logging { get; }
            public string Recipes { get; }
            public string DB { get; }
            public string QData { get; }
        }

        public class ActualPath
        {
            public ActualPath()
            {
                Path = System.IO.Directory.GetCurrentDirectory().ToUpper() == @"C:\WINDOWS\SYSTEM32" ? "D:\\224878-NordLock" : System.IO.Directory.GetCurrentDirectory();
                Alarms = Path + "\\Alarms";
                Archive = Path + "\\Archive";
                Logging = Path + "\\Logging";
                Recipes = Path + "\\Recipes";
                DB = Path + "\\DB";
                ExportPath = "D:\\";
                QData = "D:\\QData";
            }
            public string Path { get; }
            public string Alarms { get; }
            public string Archive { get; }
            public string Logging { get; }
            public string Recipes { get; }
            public string DB { get; }
            public string ExportPath { get; }
            public string QData { get; }
        }
    }
}
