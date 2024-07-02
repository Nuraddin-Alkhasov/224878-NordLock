using HMI.Module;
using HMI.Views.MessageBoxRegion;
using HMI.Views.TouchpadRegion;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using static HMI.Resources.LocalResources;

namespace HMI.Services
{
    class DoBackup
    {
      
        string[] activeDays;

        public DoBackup()
        {
            Application.Current.Dispatcher.InvokeAsync((Action)delegate
            {
                ApplicationService.SetView("TouchpadRegion", "WaitScreen", new WaitData(0, "@WaitScreen.Text1"));
            });
            BackUP();
        }
        ProjectResourcePaths Paths;
        private void BackUP()
        {
            Task doTask = Task.Run(() => {
                try
                {
                    //DB Clear 1 year  DELETE FROM Orders 

                    var b = (new LocalDBAdapter("DELETE FROM SetValues " +
                                              "WHERE [Id] In (SELECT SetValues_Id FROM Runs WHERE [Start] < '" + (DateTime.Now.AddYears(-1)).ToString("yyyy-MM-dd HH:mm:ss") + "');")).DB_Input();

                    b = (new LocalDBAdapter("DELETE FROM ActualValues " +
                                              "WHERE [Id] In (SELECT ActualValues_Id FROM Runs WHERE [Start] < '" + (DateTime.Now.AddYears(-1)).ToString("yyyy-MM-dd HH:mm:ss") + "');")).DB_Input();

                    b = (new LocalDBAdapter("DELETE FROM Errors " +
                                                "WHERE [TimeStamp] < '" + (DateTime.Now.AddYears(-1)).ToString("yyyy-MM-dd HH:mm:ss") + "';")).DB_Input();

                    b = (new LocalDBAdapter("DELETE FROM Runs " +
                                             "WHERE [Start] < '" + (DateTime.Now.AddYears(-1)).ToString("yyyy-MM-dd HH:mm:ss") + "';")).DB_Input();

                    b = (new LocalDBAdapter("DELETE FROM Charges " +
                                               "WHERE [Start] < '" + (DateTime.Now.AddYears(-1)).ToString("yyyy-MM-dd HH:mm:ss") + "';")).DB_Input();

                    b = (new LocalDBAdapter("DELETE FROM Orders " +
                                                "WHERE [TimeStamp] < '" + (DateTime.Now.AddYears(-1)).ToString("yyyy-MM-dd HH:mm:ss") + "';")).DB_Input();

                    b = (new LocalDBAdapter("vacuum;")).DB_Input();

                    Paths = (new Resources.LocalResources()).Paths;
                    //prepare data
                    
                    activeDays = new string[30];
                    for (int i = 0; i < 30; i++)
                    {
                        activeDays[i] = (DateTime.Now.AddDays(i * -1)).ToString("yyyy-MM-dd");
                    }

                    ClearData();

                    //create root folder
                    if (!System.IO.Directory.Exists(Paths.Backup.ToDayPath))
                    {
                        System.IO.Directory.CreateDirectory(Paths.Backup.ToDayPath);
                    }

                    if (!Directory.Exists(Paths.Backup.Alarms))
                    {
                        System.IO.Directory.CreateDirectory(Paths.Backup.Alarms);
                    }
                    if (!Directory.Exists(Paths.Backup.Archive + @"\CoolingZone"))
                    {
                        System.IO.Directory.CreateDirectory(Paths.Backup.Archive + @"\CoolingZone");
                    }
                    if (!Directory.Exists(Paths.Backup.Archive + @"\Dryer"))
                    {
                        System.IO.Directory.CreateDirectory(Paths.Backup.Archive + @"\Dryer");
                    }
                    if (!Directory.Exists(Paths.Backup.Archive + @"\Paint"))
                    {
                        System.IO.Directory.CreateDirectory(Paths.Backup.Archive + @"\Paint");
                    }
                    if (!Directory.Exists(Paths.Backup.Archive + @"\PreehatingZone"))
                    {
                        System.IO.Directory.CreateDirectory(Paths.Backup.Archive + @"\PreehatingZone");
                    }
                    if (!Directory.Exists(Paths.Backup.Archive))
                    {
                        System.IO.Directory.CreateDirectory(Paths.Backup.Archive);
                    }
                    if (!Directory.Exists(Paths.Backup.DB))
                    {
                        System.IO.Directory.CreateDirectory(Paths.Backup.DB);
                    }
                    if (!Directory.Exists(Paths.Backup.Logging))
                    {
                        System.IO.Directory.CreateDirectory(Paths.Backup.Logging);
                    }
                    if (!Directory.Exists(Paths.Backup.Recipes))
                    {
                        System.IO.Directory.CreateDirectory(Paths.Backup.Recipes);
                    }

                    //Alarms
                    string[] filePaths = Directory.GetFiles(Paths.Project.Alarms);
                    foreach (var filename in filePaths)
                    {
                        if (!IsFileLocked(filename))
                        {
                            string str = filename.Replace(Paths.Project.Alarms, Paths.Backup.Alarms);
                            File.Copy(filename, str, true);
                        }
                    }
                    //Archive
                    filePaths = Directory.GetFiles(Paths.Project.Archive);
                    foreach (var filename in filePaths)
                    {
                        if (!IsFileLocked(filename))
                        {
                            string str = filename.Replace(Paths.Project.Archive, Paths.Backup.Archive);
                            File.Copy(filename.ToString(), str, true);
                        }
                    }
                    //DB
                    filePaths = Directory.GetFiles(Paths.Project.DB);
                    foreach (var filename in filePaths)
                    {
                        if (!IsFileLocked(filename))
                        {
                            string str = filename.Replace(Paths.Project.DB, Paths.Backup.DB);
                            File.Copy(filename.ToString(), str, true);
                        }
                    }
                    //Logging
                    filePaths = Directory.GetFiles(Paths.Project.Logging);
                    foreach (var filename in filePaths)
                    {
                        if (!IsFileLocked(filename))
                        {
                            string str = filename.Replace(Paths.Project.Logging, Paths.Backup.Logging);
                            File.Copy(filename.ToString(), str, true);
                        }
                    }
                    //Recipes
                    string[] fileDirectories = Directory.GetDirectories(Paths.Project.Recipes);
                    foreach (var fileDirectory in fileDirectories)
                    {
                        if (!Directory.Exists(Paths.Backup.Recipes + fileDirectory.Replace(Path.GetDirectoryName(fileDirectory), "")))
                        {
                            System.IO.Directory.CreateDirectory(Paths.Backup.Recipes + fileDirectory.Replace(Path.GetDirectoryName(fileDirectory), ""));
                        }

                        filePaths = Directory.GetFiles(fileDirectory);
                        foreach (var filename in filePaths)
                        {
                            if (!IsFileLocked(filename))
                            {
                                string str = filename.Replace(fileDirectory, Paths.Backup.Recipes + fileDirectory.Replace(Path.GetDirectoryName(fileDirectory), ""));
                                File.Copy(filename.ToString(), str, true);
                            }
                        }
                    }

                    if (File.Exists(Paths.Backup.ToDayPath + ".zip"))
                    {
                        if (!IsFileLocked((new Resources.LocalResources()).Paths.Backup.ToDayPath + ".zip"))
                        {
                            File.Delete((new Resources.LocalResources()).Paths.Backup.ToDayPath + ".zip");
                        }
                    }

                    ZipFile.CreateFromDirectory(Paths.Backup.ToDayPath, Paths.Backup.ToDayPath + ".zip", CompressionLevel.Fastest, false);
                    System.IO.Directory.Delete(Paths.Backup.ToDayPath, true);
                }
                catch (Exception ex)
                {
                    new MessageBoxTask(ex, "* * * *", MessageBoxIcon.Exclamation);
                }

                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    ApplicationService.SetView("TouchpadRegion", "EmptyView");
                });
            });
        }

        private void ClearData()
        {
            string[] filePaths;

            //Arhive CoolingZone
           if (System.IO.Directory.Exists(Paths.Project.Archive))
            {
                filePaths = Directory.GetFiles(Paths.Project.Archive + @"\CoolingZone");
                foreach (var filename in filePaths)
                {
                    bool isInRange = false;
                    foreach (var day in activeDays)
                    {
                        if (filename.Contains(day))
                        {
                            isInRange = true;
                            break;
                        }
                    }
                    if (!isInRange)
                        File.Delete(filename);
                }
            }
            //Arhive Dryer
            if (System.IO.Directory.Exists(Paths.Project.Archive))
            {
                filePaths = Directory.GetFiles(Paths.Project.Archive + @"\Dryer");
                foreach (var filename in filePaths)
                {
                    bool isInRange = false;
                    foreach (var day in activeDays)
                    {
                        if (filename.Contains(day))
                        {
                            isInRange = true;
                            break;
                        }
                    }
                    if (!isInRange)
                        File.Delete(filename);
                }
            }
            //Arhive Paint
            if (System.IO.Directory.Exists(Paths.Project.Archive))
            {
                filePaths = Directory.GetFiles(Paths.Project.Archive + @"\Paint");
                foreach (var filename in filePaths)
                {
                    bool isInRange = false;
                    foreach (var day in activeDays)
                    {
                        if (filename.Contains(day))
                        {
                            isInRange = true;
                            break;
                        }
                    }
                    if (!isInRange)
                        File.Delete(filename);
                }
            }
            //Arhive PreehatingZone
            if (System.IO.Directory.Exists(Paths.Project.Archive))
            {
                filePaths = Directory.GetFiles(Paths.Project.Archive + @"\PreehatingZone");
                foreach (var filename in filePaths)
                {
                    bool isInRange = false;
                    foreach (var day in activeDays)
                    {
                        if (filename.Contains(day))
                        {
                            isInRange = true;
                            break;
                        }
                    }
                    if (!isInRange)
                        File.Delete(filename);
                }
            }



            // Alarms
            if (System.IO.Directory.Exists(Paths.Project.Alarms))
            {
                filePaths = Directory.GetFiles(Paths.Project.Alarms);
                foreach (var filename in filePaths)
                {
                    bool isInRange = false;
                    foreach (var day in activeDays)
                    {
                        if (filename.Contains(day))
                        {
                            isInRange = true;
                            break;
                        }
                    }
                    if (!isInRange)
                        if (!IsFileLocked(filename))
                            File.Delete(filename);
                }
            }


            //Logging
            if (System.IO.Directory.Exists(Paths.Project.Logging))
            {
                filePaths = Directory.GetFiles(Paths.Project.Logging);
                foreach (var filename in filePaths)
                {
                    bool isInRange = false;
                    foreach (var day in activeDays)
                    {
                        if (filename.Contains(day))
                        {
                            isInRange = true;
                            break;
                        }
                    }
                    if (!isInRange)
                        if (!IsFileLocked(filename))
                            File.Delete(filename);
                }
            }
               
            //Backups

            if (Directory.Exists(Paths.Backup.Path))
            {
                filePaths = Directory.GetFiles(Paths.Backup.Path);
                foreach (var filename in filePaths)
                {
                    bool isInRange = false;
                    foreach (var day in activeDays)
                    {
                        if (filename.Contains(day))
                        {
                            isInRange = true;
                            break;
                        }
                    }
                    if (!isInRange)
                        if (!IsFileLocked(filename))
                            File.Delete(filename);
                }
            }
            

            
        }

        public bool IsFileLocked(string filename)
        {
            bool Locked = false;
            try
            {
                FileStream fs =  File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch 
            {
                Locked = true;
            }
            return Locked;
        }
    }
}
