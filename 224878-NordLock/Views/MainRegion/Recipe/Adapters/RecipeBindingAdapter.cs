
using HMI.Module;
using HMI.Views.MainRegion.Recipe.Custom_Objects;
using HMI.Views.MessageBoxRegion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
    class RecipeBindingAdapter : AdapterBase
    {
        Visibility _DialogVisible;
        public RecipeBindingAdapter()
        {
            this.NewCommand = new ActionCommand(NewCommandExecuted);
            this.EditCommand = new ActionCommand(EditCommandExecuted);
            this.CloseDialogCommand = new ActionCommand(CloseDialogCommandExecuted);
            this.CloseDialogViewCommand = new ActionCommand(CloseDialogViewCommandExecuted);

            this.SelectMachineRecipeCommand = new ActionCommand(SelectMachineRecipeCommandExecuted);
            this.CloseRecipeSelectViewCommand = new ActionCommand(CloseRecipeSelectViewCommandExecuted);

            this.SaveCommand = new ActionCommand(SaveCommandExecuted);
            this.DeleteCommand = new ActionCommand(DeleteCommandExecuted);
            _DialogVisible = Visibility.Hidden;
        }

        #region - - - Properties - - -

        ObservableCollection<Barcode> barcodeList = new ObservableCollection<Barcode>();
        public ObservableCollection<Barcode> BarcodeList
        {
            get { return this.barcodeList; }
            set
            {
                this.barcodeList = value;
                this.OnPropertyChanged("BarcodeList");
            }
        }

        ObservableCollection<MachineRecipe> machineRecipes = new ObservableCollection<MachineRecipe>();
        public ObservableCollection<MachineRecipe> MachineRecipes
        {
            get { return this.machineRecipes; }
            set
            {
                this.machineRecipes = value;
                this.OnPropertyChanged("MachineRecipes");
            }
        }

        MachineRecipe selectedMachineRecipe;
        public MachineRecipe SelectedMachineRecipe
        {
            get { return this.selectedMachineRecipe; }
            set
            {
                this.selectedMachineRecipe = value;
                this.OnPropertyChanged("SelectedMachineRecipe");
            }
        }

        Barcode selectedBarcode = new Barcode();
        public Barcode SelectedBarcode
        {
            get { return this.selectedBarcode; }
            set
            {
                if (value != null) { IsBCToMRSelected = true; }
                else { IsBCToMRSelected = false; }
                this.selectedBarcode = value;
                this.OnPropertyChanged("SelectedBarcode");
            }
        }
        Barcode selectedBarcodeBuffer;
        public Barcode SelectedBarcodeBuffer
        {
            get { return this.selectedBarcodeBuffer; }
            set
            {
                this.selectedBarcodeBuffer = value;
                this.OnPropertyChanged("SelectedBarcodeBuffer");
            }
        }

        bool isBCToMRSelected;
        public bool IsBCToMRSelected
        {
            get { return this.isBCToMRSelected; }
            set
            {
                this.isBCToMRSelected = value;
                this.OnPropertyChanged("IsBCToMRSelected");
            }
        }
        public Visibility DialogVisible
        {
            get { return this._DialogVisible; }
            set
            {
                if (!Equals(value, this._DialogVisible))
                {
                    this._DialogVisible = value;
                    this.OnPropertyChanged("DialogVisible");
                }
            }
        }


        #endregion

        #region - - - Commands - - -

        public ICommand NewCommand { get; set; }
        private void NewCommandExecuted(object parameter)
        {
            Task T = Task.Run(() =>
            {
                DataTable DT = (new LocalDBAdapter("SELECT MIN(Id) " +
                                                  "FROM Recipes_MR")).DB_Output();
                bool result = (new LocalDBAdapter("INSERT INTO Barcodes (Barcode, MR_Id) VALUES ('NO-Barcode', "+ DT.Rows[0][0].ToString() + ")")).DB_Input();
                UpdateBarcodeList();
            });
        }

        public ICommand EditCommand { get; set; }

        private void EditCommandExecuted(object parameter)
        {
            DialogVisible = Visibility.Visible;
            SelectedBarcodeBuffer = new Barcode()
            {
                Id = SelectedBarcode.Id,
                MR_Name = SelectedBarcode.MR_Name,
                MR_Id = SelectedBarcode.MR_Id,
                BC = SelectedBarcode.BC
            };
        }

        public ICommand SaveCommand { get; set; }

        private void SaveCommandExecuted(object parameter)
        {
            if (this.SelectedBarcode != null)
            {
                Task T = Task.Run(() =>
                {
                    if (SelectedBarcode != null)
                    {
                        DataTable DT = (new LocalDBAdapter("Select * " +
                                                           "FROM Barcodes " +
                                                           "WHERE Barcode = '" + SelectedBarcodeBuffer.BC + "';")).DB_Output();

                        if (DT.Rows.Count > 0)
                        {
                            new MessageBoxTask("@Datapicker.Text10", "@Datapicker.Text7", MessageBoxIcon.Exclamation);
                            return;
                        }
                        else 
                        {
                            bool result = (new LocalDBAdapter("UPDATE Barcodes " +
                                                             "SET Barcode ='" + SelectedBarcodeBuffer.BC + "', MR_Id = " + SelectedBarcodeBuffer.MR_Id + " " +
                                                             "WHERE Id = " + SelectedBarcodeBuffer.Id + ";")).DB_Input();
                        }
                    }
                    DialogVisible = Visibility.Hidden;
                    UpdateBarcodeList();
                });
            }
        }

        public ICommand DeleteCommand { get; set; }

        private void DeleteCommandExecuted(object parameter)
        {
            if (this.SelectedBarcode != null)
            {
                Task T = Task.Run(() =>
                {
                    bool result = (new LocalDBAdapter("DELETE FROM Barcodes WHERE Id = " + SelectedBarcode.Id + ";")).DB_Input();
                    UpdateBarcodeList();
                });
            }
        }

        public ICommand SelectMachineRecipeCommand { get; set; }

        private void SelectMachineRecipeCommandExecuted(object parameter)
        {
            SelectedBarcodeBuffer.MR_Id = SelectedMachineRecipe.Id;
            SelectedBarcodeBuffer.MR_Name = SelectedMachineRecipe.Name;
            this.OnPropertyChanged("SelectedBarcodeBuffer");
            CloseRecipeSelectViewCommandExecuted(null);
        }

        public ICommand CloseDialogCommand { get; set; }

        private void CloseDialogCommandExecuted(object parameter)
        {
            DialogVisible = Visibility.Hidden;
        }

        public ICommand CloseDialogViewCommand { get; set; }

        private void CloseDialogViewCommandExecuted(object parameter)
        {
            DialogVisible = Visibility.Hidden;
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }

        public ICommand CloseRecipeSelectViewCommand { get; set; }

        private void CloseRecipeSelectViewCommandExecuted(object parameter)
        {
            ApplicationService.SetView("MessageBoxRegion", "EmptyView");
        }

        #endregion

        #region - - - Methods - - -
        public void UpdateBarcodeList()
        {
            Task T = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    DataTable DT = (new LocalDBAdapter("SELECT Barcodes.Id, Barcodes.Barcode,  Recipes_MR.Name, Barcodes.MR_Id " +
                                                       "FROM Barcodes "+
                                                       "INNER JOIN Recipes_MR ON Barcodes.MR_Id = Recipes_MR.Id;")).DB_Output();
                    BarcodeList.Clear();
                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow r in DT.Rows)
                        {
                            BarcodeList.Add(new Barcode()
                            {
                                Id = (long)r["Id"],
                                BC = (string)r["Barcode"],
                                MR_Name = (string)r["Name"],
                                MR_Id = (long)r["MR_Id"],
                            });
                        }
                    }

                    DT = (new LocalDBAdapter("SELECT Id, Name, Description, LastChanged, User " +
                                             "FROM Recipes_MR;")).DB_Output();
                    MachineRecipes.Clear();
                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow r in DT.Rows)
                        {
                            MachineRecipes.Add(new MachineRecipe()
                            {
                                Id = (long)r["Id"],
                                Name = (string)r["Name"],
                                Description = (string)r["Description"],
                                LastChanged = (DateTime)r["LastChanged"],
                                User = (string)r["User"]
                            });
                        }
                    }
                });
            });
        }

        #endregion

    }
}
