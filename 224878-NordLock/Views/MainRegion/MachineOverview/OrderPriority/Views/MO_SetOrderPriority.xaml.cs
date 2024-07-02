using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Language;

namespace HMI.Views.MainRegion
{
	[ExportView("MO_SetOrderPriority")]
	public partial class MO_SetOrderPriority
	{
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        ILanguageService TS = ApplicationService.GetService<ILanguageService>();
        public MO_SetOrderPriority()
		{
			this.InitializeComponent();
            H.Header = TS.GetText("@MainView.Text71");
            TS.LanguageChanged += TS_LanguageChanged;
            VW_isRelease = VS.GetVariable("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Lager Rückführen.Freigabe");
            VW_isRelease.Change += isRelease_Change;
            VW_isReleaseStop = VS.GetVariable("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Lager Rückführen.StopRelease");
            VW_isReleaseStop.Change += VW_isReleaseStop_Change;
            VW_Status = VS.GetVariable("NLM4.PLC.Blocks.50 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Produktionsmodus.Lager Rückführen.Status");
            VW_Status.Change += Status_Change;

        }

		private void _IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			if (this.IsVisible) 
			{
				IRegionService iRS = ApplicationService.GetService<IRegionService>();
				Adapter_SetOrderPriority A_SOP = (Adapter_SetOrderPriority)((MO_SetOrderPriority)iRS.GetView("MO_SetOrderPriority")).DataContext;
				this.DataContext = A_SOP;
				A_SOP.GetOrders();


            }
		}
       
		private void TS_LanguageChanged(object sender, LanguageChangedEventArgs e)
		{
			H.Header = TS.GetText("@MainView.Text71");
		}
        IVariable VW_Status;
       
        private void Status_Change(object sender, VariableEventArgs e)
        {
            switch ((short)e.Value)
            {
                case 0: btnstart.IsDefault = false; btnstart.IsBlinkEnabled = false; break;
                case 1: btnstart.IsDefault = true; btnstart.IsBlinkEnabled = false; break;
                case 2: btnstart.IsDefault = false; btnstart.IsBlinkEnabled = true; break;
            }
        }

        IVariable VW_isRelease;

        private void isRelease_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                btnstart.IsEnabled = true;
            }
            else
            {
                btnstart.IsEnabled = false;
            }
        }

        IVariable VW_isReleaseStop;

        private void VW_isReleaseStop_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                btnstop.IsEnabled = true;
            }
            else
            {
                btnstop.IsEnabled = false;
            }
        }
    }
}