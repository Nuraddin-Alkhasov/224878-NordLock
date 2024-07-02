using HMI.Module;
using HMI.Views.MessageBoxRegion;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;


namespace HMI.Services
{
    class CheckJumo
    {

        public CheckJumo()
        {
            Check();
        }

        private void Check()
        {
            DataTable temp = (new LocalDBAdapter("SELECT Value FROM Config WHERE Variable = 'JumoPath';")).DB_Output();
            string path = temp.Rows[0][0].ToString();
            Task doTask = Task.Run(() => {
                string[] filePaths = Directory.GetFiles(path);
                foreach (var filename in filePaths)
                {
                    if (filename.Contains("_2019_4_25"))
                        return;
                }
                new MessageBoxTask("@Backup.Text8", "@Backup.Text9", MessageBoxIcon.Exclamation);
            });
        }

       
    }
}
