using HMI.Module;
using HMI.Views.MainRegion.Recipe;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Protocol
{

	[ExportView("Protocol_SetValues")]
	public partial class Protocol_SetValues
    {
        public Protocol_SetValues()
		{
			this.InitializeComponent();
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ApplicationService.SetView("MessageBoxRegion", "EmptyView");
		}

		private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
            if (this.IsVisible)
            {
                ObservableCollection<VWVariable> VWVariables = new ObservableCollection<VWVariable>();
                VWVariables = (ObservableCollection<VWVariable>)ApplicationService.ObjectStore.GetValue("Protocol_SetValues_KEY");
                ApplicationService.ObjectStore.Remove("Protocol_SetValues_KEY");
                Task obTask = Task.Run(async () =>
                {
                    for (int i = 0; i < VWVariables.Count; i++)
                    {
                        switch (Convert.ToInt32(VWVariables[i].Value))
                        {
                            case 1:
                                await Dispatcher.InvokeAsync((Action)delegate
                                {
                                    SV.Items.Add(new Protocol_CSV_D()
                                    {
                                        ReversalTime = Convert.ToInt32(VWVariables[i + 1].Value),
                                        SpinSpeed = Convert.ToDouble(VWVariables[i + 2].Value),
                                        DipTime = Convert.ToInt32(VWVariables[i + 3].Value)
                                    });
                                    SV.ScrollIntoView(SV.Items[SV.Items.Count-1]);
                                });
                                i += 3;
                                break;
                            case 2:
                                await Dispatcher.InvokeAsync((Action)delegate
                                {
                                    SV.Items.Add(new Protocol_CSV_S()
                                    {
                                        PlanetSpeed = Convert.ToDouble(VWVariables[i + 1].Value),
                                        PlanetTime = Convert.ToInt32(VWVariables[i + 2].Value),
                                        SpinSpeed1 = Convert.ToDouble(VWVariables[i + 3].Value),
                                        SpinSpeed2 = Convert.ToDouble(VWVariables[i + 4].Value),
                                        SpinSpeed3 = Convert.ToDouble(VWVariables[i + 5].Value),
                                        SpinTime1 = Convert.ToInt32(VWVariables[i + 6].Value),
                                        SpinTime3 = Convert.ToInt32(VWVariables[i + 7].Value)
                                    });
                                    SV.ScrollIntoView(SV.Items[SV.Items.Count - 1]);
                                });
                                i += 7;
                                break;
                            case 3:
                                await Dispatcher.InvokeAsync((Action)delegate
                                {
                                    SV.Items.Add(new Protocol_CSV_T()
                                    {
                                        TiltAngle = Convert.ToDouble(VWVariables[i + 1].Value),
                                        SpinSpeed = Convert.ToDouble(VWVariables[i + 2].Value),
                                        ReversalTime = Convert.ToInt32(VWVariables[i + 3].Value),
                                        TiltTime = Convert.ToInt32(VWVariables[i + 4].Value)


                                    });
                                    SV.ScrollIntoView(SV.Items[SV.Items.Count - 1]);
                                });
                                i += 4;
                                break;
                            default: break;
                        }

                        await Task.Delay(300);
                    }
                    
                });
            }
            else 
            {
                SV.Items.Clear();
            }
        }

     
    }
}