using HMI.Interfaces;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using VisiWin.ApplicationFramework;


namespace HMI.Services
{
    [ExportService(typeof(IEKS))]
    [Export(typeof(IEKS))]
    public class Service_EKS : ServiceBase, IEKS
    {

        HMI.Services.Custom_Objects.ElectronicKeySystem EKS;

        public Service_EKS()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }



        #region OnProject

        // Hier stehen noch keine VisiWin Funktionen zur Verfügung
        protected override void OnLoadProjectStarted()
        {
            base.OnLoadProjectStarted();
        }

        // Hier kann auf die VisiWin Funktionen zugegriffen werden
        protected override void OnLoadProjectCompleted()
        {
            EKS = new Custom_Objects.ElectronicKeySystem(); ;
            EKS.OpenConnection();
               
            base.OnLoadProjectCompleted();
        }



        // Hier stehen noch die VisiWin Funktionen zur Verfügung
        protected override void OnUnloadProjectStarted()
        {
            if (EKS != null)
                EKS.CloseConnection();
            base.OnUnloadProjectStarted();
        }

        // Hier sind keine VisiWin Funktionen mehr verfügbar. Bei C/S ist die Verbindung zum Server schon getrennt.
        protected override void OnUnloadProjectCompleted()
        {
            base.OnUnloadProjectCompleted();
        }

        public void OpenConnection()
        {
            if (EKS != null)
                EKS.OpenConnection();
        }

        public void CloseConnection()
        {
            if (EKS != null)
                EKS.CloseConnection();
        }

        public string Read()
        {
            if (EKS != null)
                return EKS.Read();
            else
                return "";
        }

        public void Write(string data)
        {
            if (EKS != null)
                EKS.Write(data);
        }

        public string GetStatus()
        {
            if (EKS != null)
                return EKS.GetStatus();
            else
                return "";
        }





        #endregion

    }
}
