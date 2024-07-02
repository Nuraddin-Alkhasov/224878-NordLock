using HMI.Module;
using HMI.Views.DialogRegion;
using HMI.Views.MainRegion.Recipe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.UserManagement;

namespace HMI.Views.MainRegion.Protocol
{
    [ExportAdapter("ProtocolAdapter")]
    public class ProtocolAdapter : AdapterBase
    {
        public ProtocolAdapter()
        {
            this.ShowMRData = new ActionCommand(this.ShowMRDataCommandExecuted);
            this.ShowSetValues = new ActionCommand(this.ShowSetValuesExecuted);

            Orders = new ObservableCollection<Order>();
            TimeSpanFilterTypes = new ObservableCollection<HistoricalTimeSpanFilterItem>
            {
                new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.Today),
                new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.Yesterday),
                new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.ThisWeek),
                new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.LastWeek),
                new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.Custom)
            };
            SetTimeSpan(HistoricalTimeSpanFilterType.Today);
            this.desiredUsers = new ObservableCollection<UserFilterItem>();
            IsAllUsersSelected = true;
            RefreshUsers();
        }

        #region - - - FILTER - - -

        private ObservableCollection<UserFilterItem> desiredUsers;
        private bool isUserSelectionEnabled;
        private bool isAllUsersSelected;
        private DateTime maxTime;
        private DateTime minTime;
        private int selectedTimeSpanFilterTypeIndex;
        private bool customTimeSpanFilterEnabled;
        private string data1Filter;
        private string data2Filter;
        private string data3Filter;
        private string mrFilter;

        public string Data1Filter
        {
            get { return this.data1Filter; }
            set
            {
                if (!Equals(value, this.data1Filter))
                {
                    this.data1Filter = value;
                    this.OnPropertyChanged("Data1Filter");
                }
            }
        }

        public string Data2Filter
        {
            get { return this.data2Filter; }
            set
            {
                if (!Equals(value, this.data2Filter))
                {
                    this.data2Filter = value;
                    this.OnPropertyChanged("Data2Filter");
                }
            }
        }

        public string Data3Filter
        {
            get { return this.data3Filter; }
            set
            {
                if (!Equals(value, this.data3Filter))
                {
                    this.data3Filter = value;
                    this.OnPropertyChanged("Data3Filter");
                }
            }
        }

        public string MRFilter
        {
            get { return this.mrFilter; }
            set
            {
                if (!Equals(value, this.mrFilter))
                {
                    this.mrFilter = value;
                    this.OnPropertyChanged("MRFilter");
                }
            }
        }

        public DateTime MaxTime
        {
            get { return maxTime; }
            set
            {
                if (value != maxTime)
                {
                    maxTime = value;
                    OnPropertyChanged("MaxTime");
                }
            }
        }

        public DateTime MinTime
        {
            get { return minTime; }
            set
            {
                if (value != minTime)
                {
                    minTime = value;
                    OnPropertyChanged("MinTime");
                }
            }
        }

        public int SelectedTimeSpanFilterTypeIndex
        {
            get { return selectedTimeSpanFilterTypeIndex; }
            set
            {
                if (value != selectedTimeSpanFilterTypeIndex)
                {
                    selectedTimeSpanFilterTypeIndex = value;
                    OnPropertyChanged("SelectedTimeSpanFilterTypeIndex");
                }
            }
        }

        public ObservableCollection<HistoricalTimeSpanFilterItem> TimeSpanFilterTypes { get; }

        public bool CustomTimeSpanFilterEnabled
        {
            get { return customTimeSpanFilterEnabled; }
            set
            {
                if (value != customTimeSpanFilterEnabled)
                {
                    customTimeSpanFilterEnabled = value;
                    OnPropertyChanged("CustomTimeSpanFilterEnabled");
                }
            }
        }

        public void SetTimeSpan(HistoricalTimeSpanFilterType selectedTimeSpanFilterType)
        {
            switch (selectedTimeSpanFilterType)
            {
                case HistoricalTimeSpanFilterType.Custom:
                    break;
                case HistoricalTimeSpanFilterType.Today:
                    MinTime = DateTime.Now.Date;
                    MaxTime = MinTime.Add(TimeSpan.FromDays(1));
                    break;
                case HistoricalTimeSpanFilterType.Yesterday:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-1.0));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(1));
                    break;
                case HistoricalTimeSpanFilterType.ThisWeek:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-(double)DateTime.Now.Date.DayOfWeek));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(7));
                    break;
                case HistoricalTimeSpanFilterType.LastWeek:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-(double)DateTime.Now.Date.DayOfWeek - 7));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(7));
                    break;
                default:
                    break;
            }

        }

        public ObservableCollection<UserFilterItem> DesiredUsers
        {
            get
            {
                return this.desiredUsers;
            }
            set
            {
                if (value != this.desiredUsers)
                {
                    this.desiredUsers = value;
                    OnPropertyChanged("DesiredUsers");
                }
            }
        }

        public bool IsAllUsersSelected
        {
            get { return this.isAllUsersSelected; }
            set
            {
                if (value != this.isAllUsersSelected)
                {
                    this.isAllUsersSelected = value;
                    OnPropertyChanged("IsAllUsersSelected");

                    IsUserSelectionEnabled = !this.isAllUsersSelected;
                }
            }
        }

        public bool IsUserSelectionEnabled
        {
            get { return this.isUserSelectionEnabled; }
            set
            {
                if (value != this.isUserSelectionEnabled)
                {
                    this.isUserSelectionEnabled = value;
                    OnPropertyChanged("IsUserSelectionEnabled");
                }
            }
        }

        void RefreshUsers()
        {
            desiredUsers.Clear();
            IUserManagementService uMS = ApplicationService.GetService<IUserManagementService>();
            if (uMS != null)
            {
                foreach (string user in uMS.UserNames)
                {
                    IUserDefinition uD = uMS.GetUserDefinition(user);
                    if (uD != null)
                        desiredUsers.Add(new UserFilterItem(user, uD.FullName));
                }
            }
        }

        public void BW_FilterSQL(List<object> param)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += FilterSQL;
            bw.RunWorkerAsync(argument: param);
        }

        private void FilterSQL(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                Orders.Clear();
                DataTable DT = (new LocalDBAdapter(GenerateSQLQuery())).DB_Output();
                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {

                        Orders.Add(new Order()
                        {
                            Id = (long)r["Id"],
                            TimeStamp = ((DateTime)r["TimeStamp"]).ToString("dd.MM.yyyy HH:mm:ss"),
                            Data_1 = (string)r["Data_1"],
                            Data_2 = (string)r["Data_2"],
                            Data_3 = (string)r["Data_3"],
                            MR_Id = (long)r["MR_Id"],
                            MR = (string)r["Name"],
                            Charges = (long)r["Charges"],
                            User = (string)r["User"]
                        });
                    }
                }
                List<object> paramList = (List<object>)e.Argument;
                if (paramList != null)
                    if ((int)paramList[0] != -1)
                        ((DataGrid)paramList[1]).SelectedIndex = (int)paramList[0];
            });

        }

        public string GenerateSQLQuery()
        {
            string T = "Select Orders.Id, Orders.TimeStamp, Orders.Data_1, Orders.Data_2, Orders.Data_3, Orders.MR_Id, Recipes_MR.Name, Orders.Charges, Orders.User " +
                       "From Orders " +
                       "INNER JOIN Recipes_MR ON Orders.MR_Id = Recipes_MR.Id";
            string F = "";
            List<string> FL = new List<string>();

            if (minTime != null && maxTime != null)
            {
                string Minmonth;
                if (minTime.Month.ToString().Length == 1) { Minmonth = "0" + minTime.Month.ToString(); }
                else { Minmonth = minTime.Month.ToString(); }

                string Minday;
                if (minTime.Day.ToString().Length == 1) { Minday = "0" + minTime.Day.ToString(); }
                else { Minday = minTime.Day.ToString(); }

                string Maxmonth;
                if (maxTime.Month.ToString().Length == 1) { Maxmonth = "0" + maxTime.Month.ToString(); }
                else { Maxmonth = maxTime.Month.ToString(); }

                string Maxday;
                if (maxTime.Day.ToString().Length == 1) { Maxday = "0" + maxTime.Day.ToString(); }
                else { Maxday = maxTime.Day.ToString(); }

                FL.Add("datetime(TimeStamp) >= date('" + minTime.Year + "-" + Minmonth + "-" + Minday + "') AND date(TimeStamp) <= date('" + MaxTime.Year + "-" + Maxmonth + "-" + Maxday + "')");

            }

            if (data1Filter != "" && data1Filter != null)
            {
                FL.Add("Data_1 like '%" + data1Filter + "%'");
            }

            if (data2Filter != "" && data2Filter != null)
            {
                FL.Add("Data_2 like '%" + data2Filter + "%'");
            }

            if (data3Filter != "" && data3Filter != null)
            {
                FL.Add("Data_3 like '%" + data3Filter + "%'");
            }

            if (mrFilter != "" && mrFilter != null)
            {
                FL.Add("MachineRecipes.MR like '%" + mrFilter + "%'");
            }

            if (!this.isAllUsersSelected)
            {
                string userFilt = "(";
                foreach (UserFilterItem user in this.DesiredUsers)
                {
                    if (user.IsSelected)
                        userFilt = userFilt + "Orders.User ='" + user.FullName + "' OR ";
                }
                if (userFilt.Length > 3)
                {
                    userFilt = userFilt.Remove(userFilt.Length - 4);
                    userFilt += ")";
                    FL.Add(userFilt);
                }
            }
            //else
            //{
            //    string userFilt = "(";
            //    foreach (UserFilterItem user in this.DesiredUsers)
            //    {
            //        userFilt = userFilt + "Orders.User ='" + user.FullName + "' OR ";
            //    }
            //    userFilt = userFilt.Remove(userFilt.Length - 4);
            //    userFilt = userFilt + ")";
            //    FL.Add(userFilt);
            //}

            if (FL.Count > 0)
            {
                F += " WHERE ";
                for (int i = 0; i < FL.Count; i++)
                {
                    if (i == 0)
                    {
                        F += FL[i];
                    }
                    else
                    {
                        F = F + " AND " + FL[i];
                    }
                }
            }
            return T + F + ";";
        }

        #endregion

        #region - - - Orders - - -

        private ObservableCollection<Order> orders;
        public ObservableCollection<Order> Orders
        {
            get { return this.orders; }
            set
            {
                if (!Equals(value, this.orders))
                {
                    this.orders = value;
                    this.OnPropertyChanged("Orders");
                }
            }
        }

        private Order selectedOrder;
        public Order SelectedOrder
        {
            get { return this.selectedOrder; }
            set
            {
                if (!Equals(value, this.orders))
                {
                    this.selectedOrder = value;
                    if (selectedOrder != null) selectedOrder.SetChargesList();
                    this.OnPropertyChanged("SelectedOrder");
                }
            }
        }

        #endregion

        #region - - - MachineRecipe - - -

        private MachineRecipe selectedMR = new MachineRecipe();
        public MachineRecipe SelectedMR
        {
            get { return this.selectedMR; }
            set
            {
                this.selectedMR = value;
                this.OnPropertyChanged("SelectedMR");
            }
        }

        #endregion

        #region - - - Charges - - -

        private Charge selectedCharge;
        public Charge SelectedCharge
        {
            get { return this.selectedCharge; }
            set
            {
                if (value != this.selectedCharge)
                {
                    this.selectedCharge = value;
                    if (selectedCharge != null)
                    {
                        selectedCharge.SetRunList();
                        selectedCharge.SetErrorList();
                    }
                    this.OnPropertyChanged("SelectedCharge");
                }
            }
        }

        #endregion

        #region - - - Commands - - -
        public ICommand ShowMRData { get; set; }
        void ShowMRDataCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MessageBoxRegion", "MO_MRData", SelectedOrder.MR_Id);
        }

        #endregion

        #region - - - Methods - - -
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "SelectedTimeSpanFilterTypeIndex")
            {
                if (SelectedTimeSpanFilterTypeIndex >= 0)
                {
                    HistoricalTimeSpanFilterType selectedFilterType = TimeSpanFilterTypes[SelectedTimeSpanFilterTypeIndex].FilterType;
                    CustomTimeSpanFilterEnabled = selectedFilterType == HistoricalTimeSpanFilterType.Custom;
                    SetTimeSpan(selectedFilterType);
                }
            }
        }

        #endregion

        #region - - - SetValues - - -

        private Run selectedRun;
        public Run SelectedRun
        {
            get { return this.selectedRun; }
            set
            {
                if (value != this.selectedRun)
                {
                    this.selectedRun = value;
                    if (value != null)
                        IsShowSetValuesVisible = Visibility.Visible;
                    else
                        IsShowSetValuesVisible = Visibility.Hidden;
                    this.OnPropertyChanged("SelectedRun");
                }
            }
        }

        private Visibility isShowSetValuesVisible = Visibility.Hidden;
        public Visibility IsShowSetValuesVisible
        {
            get { return this.isShowSetValuesVisible; }
            set
            {
                if (!Equals(value, this.isShowSetValuesVisible))
                {
                    this.isShowSetValuesVisible = value;
                    this.OnPropertyChanged("IsShowSetValuesVisible");
                }
            }
        }

        public ICommand ShowSetValues { get; set; }
        void ShowSetValuesExecuted(object parameter)
        {
            DataTable SetValues = (new LocalDBAdapter("SELECT * " +
                                                 "FROM SetValues " +
                                                 "WHERE Id=" + SelectedRun.SetValues_Id)).DB_Output();
            if (SetValues.Rows.Count > 0)
            {
                XElement Values = XElement.Parse(SetValues.Rows[0]["Data"].ToString());
                ObservableCollection<VWVariable> VWVariables = new ObservableCollection<VWVariable>();
                
                foreach (XElement x in Values.Descendants("V"))
                {
                    VWVariables.Add(new VWVariable(x.Attribute("Item").Value, x.Attribute("Type").Value, x.Attribute("Value").Value));
                }
                ApplicationService.SetView("MessageBoxRegion", "Protocol_SetValues", VWVariables);
            }



           
        }


        #endregion

    }
}
