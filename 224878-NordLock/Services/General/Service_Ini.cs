using HMI.Interfaces;
using HMI.Services.Custom_Objects;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;

namespace HMI.Services
{
    [ExportService(typeof(IIni))] 
    [Export(typeof(IIni))]
    public class Service_Ini : ServiceBase, IIni
    {

        IVariableService VS;
        IVariable Service;
        public Service_Ini()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }
     

        #region OnProject

      
        // Hier stehen noch keine VisiWin Funktionen zur Verfügung
        protected override void OnLoadProjectStarted()
        {
            (new VWSafeStart()).DoWork();
            base.OnLoadProjectStarted();
        }

        // Hier kann auf die VisiWin Funktionen zugegriffen werden
        protected override void OnLoadProjectCompleted()
        {
            
            InitializeRecipe();
            VS = ApplicationService.GetService<IVariableService>();
            Service = VS.GetVariable("NL.PLC.Blocks.50 HMI.02 MOP.DB MOP Kommunikation.Kommunikation.Gebrückter Modus durch Remote.Aktiv");
            Service.Change += Service_Change;
            base.OnLoadProjectCompleted();
        }

        // Hier stehen noch die VisiWin Funktionen zur Verfügung
        protected override void OnUnloadProjectStarted()
        {
            base.OnUnloadProjectStarted();
        }

        // Hier sind keine VisiWin Funktionen mehr verfügbar. Bei C/S ist die Verbindung zum Server schon getrennt.
        protected override void OnUnloadProjectCompleted()
        {
            base.OnUnloadProjectCompleted();
        }

        private void InitializeRecipe()
        {
            IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("Article");
            T.StartEdit();
            T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("CoatingStep");
            T.StartEdit();
            T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("Coating");
            T.StartEdit();
            T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("TBStatus");
            T.StartEdit();
        }


        void Service_Change(object sender, VariableEventArgs e)
        {
            if(e.Quality.Data == DataQuality.Good)
            {
                if ((bool)e.Value)
                {
                    ApplicationService.SetView("DialogRegion", "ServiceScreen");
                }
                else
                {
                    ApplicationService.SetView("DialogRegion", "EmptyView");
                }
            }
        }

        #endregion

    }
}
