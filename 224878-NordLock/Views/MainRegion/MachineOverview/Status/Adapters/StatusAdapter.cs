using HMI.Module;
using HMI.Views.MainRegion.Recipe;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.DataAccess;

namespace HMI.Views.MainRegion.MachineOverview
{
   
    [ExportAdapter("StatusAdapter")]
    public class StatusAdapter : AdapterBase
    {
        readonly IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable VW_OrderId;
        public StatusAdapter()
        {
            this.ShowMRData = new ActionCommand(this.ShowMRDataCommandExecuted);
            this.Close = new ActionCommand(this.CloseCommandExecuted);
            VW_OrderId = VS.GetVariable("NL.PLC.Blocks.50 HMI.01 PC.DB PC.Status.Header.Order Id");
            VW_OrderId.Change += VW_OrderId_ValueChanged;
        }

       
        #region - - - Properties - - -

        private Order currentOrder = new Order();
        public Order CurrentOrder
        {
            get { return currentOrder; }
            set
            {
                if (value != null)
                {
                    this.currentOrder = value;
                    this.OnPropertyChanged("CurrentOrder");
                }
            }
        }


        #endregion

        #region - - - Commands - - -
        public ICommand ShowMRData { get; set; }
        void ShowMRDataCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MessageBoxRegion", "MO_MRData", CurrentOrder.MR.Id);
        }

        public ICommand Close { get; set; }
        void CloseCommandExecuted(object parameter)
        {
            Task obTask = Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    ApplicationService.SetView("DialogRegion", "EmptyView");
                });
            });
           
        }

        #endregion

        #region - - - Methods - - -

        private void VW_OrderId_ValueChanged(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                Task obTask = Task.Run(() =>
                {

                    uint O_Id = (uint)e.Value;


                    DataTable DT = (new LocalDBAdapter("SELECT Orders.Id, Orders.Data_1, Orders.Data_2, Orders.Data_3, Orders.MR_Id, Recipes_MR.Name,  Orders.User " +
                                                       "FROM Orders " +
                                                       "INNER JOIN Recipes_MR ON Orders.MR_Id = Recipes_MR.Id " +
                                                       "WHERE Orders.Id = " + O_Id + "; ")).DB_Output();

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow r in DT.Rows)
                        {
                            CurrentOrder = new Order()
                            {
                                Id = O_Id,
                                Data_1 = (string)r["Data_1"],
                                Data_2 = (string)r["Data_2"],
                                Data_3 = (string)r["Data_3"],
                                MR = new MachineRecipe() { Id = (long)r["MR_Id"], Name = (string)r["Name"] },
                                User = (string)r["User"]
                            };
                        }
                    }
                    else { CurrentOrder = new Order(); }
                });
            }
        }
        #endregion
    }


}