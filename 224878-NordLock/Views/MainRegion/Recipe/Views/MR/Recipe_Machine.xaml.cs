using HMI.UserControls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using RadioButton = VisiWin.Controls.RadioButton;

namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("Recipe_Machine")]
    public partial class Recipe_Machine : VisiWin.Controls.View
    {

        public bool SaveRecipe = true;
        Point _A_lastTapLocation_;
        Point _C_lastTapLocation_;
        readonly IRegionService iRS = ApplicationService.GetService<IRegionService>();
        public Recipe_Machine()
        {
            this.InitializeComponent();
        }

        #region - - - Event Handlers - - -

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var adapter = ApplicationService.GetAdapter(nameof(ReportViewAdapter)) as ReportViewAdapter;
            //IRecipeClass MR = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MR");
            //string MRName = ApplicationService.GetVariableValue("Recipe.MR.Name").ToString();
            //if (MRName != "")
            //{
            //    if (MR.IsExistingRecipeFile(MRName))
            //    {
            //        Dictionary<string, object> r_val = MR.GetRecipeFile(MRName).GetValues();
            //        r_val.Add("Recipe.MR.Name", MR.GetRecipeFile(MRName).FileName);
            //        Func<CancellationToken, Task<ReportConfiguration>> config;
            //        config = (t) => MRReport.GetReportConfiguration(r_val);
            //        adapter?.OpenView(config);

            //    }
            //}
        }


        private void AList_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            _A_lastTapLocation_ = e.GetTouchPoint(this).Position;
        }
        private void AList_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (dgv_a.SelectedIndex!=-1)
            { 
                RecalculatePosition(_A_lastTapLocation_, e.GetTouchPoint(this).Position, "A");
            }
        }

        private void AList_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            if (ArticleList.IsChecked == true && A.IsChecked == true)
            {
                if ((IsOverSelectedStep(DragItem.PointToScreen(new Point(0d, 0d)), "A")))
                    ItemDrop((ArticleRecipe)dgv_a.SelectedItem, "A");
            }
            DragItem.Visibility = Visibility.Hidden;
        }

        private void AList_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ArticleList.IsChecked == true && A.IsChecked == true)
            {
                ItemDrop((ArticleRecipe)dgv_a.SelectedItem, "A");
            }
        }

        private void CList_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            _C_lastTapLocation_ = e.GetTouchPoint(this).Position;
        }
        private void CList_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (dgv_c.SelectedIndex != -1)
            {
                RecalculatePosition(_C_lastTapLocation_, e.GetTouchPoint(this).Position, "C");
            }
        }

        private void CList_PreviewTouchUp(object sender, TouchEventArgs e)
        {
         
            if ((IsOverSelectedStep(DragItem.PointToScreen(new Point(0d, 0d)), "C")))
                ItemDrop((CoatingRecipe)dgv_c.SelectedItem, "C");

            DragItem.Visibility = Visibility.Hidden;
        }

        private void CList_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CoatingList.IsChecked == true)
            {
                ItemDrop((CoatingRecipe)dgv_c.SelectedItem, "C");
            }
        }

        #endregion

        #region - - - Methods - - -

        private void RecalculatePosition(Point _lastTapLocation_, Point a, string _selectedRecipeType)
        {
            DragItem.Margin = new Thickness(a.X - 30, a.Y - 70, 0, 0);

            if (Math.Abs(a.X - _lastTapLocation_.X) > 25 && DragItem.Visibility == Visibility.Hidden)
            {
                switch (_selectedRecipeType)
                {
                    case "A":
                        DragItem_Name.Text = ((ArticleRecipe)dgv_a.SelectedItem).Name;
                        DragItem_Pic.SymbolResourceKey = ((ArticleRecipe)dgv_a.SelectedItem).Symbol;
                        DragItem.Width = ((ArticleRecipe)dgv_a.SelectedItem).Name.Length * 8.4 + 65;
                        break;
                    case "C":
                        DragItem_Name.Text = ((CoatingRecipe)dgv_c.SelectedItem).Name;
                        DragItem_Pic.SymbolResourceKey = ((CoatingRecipe)dgv_c.SelectedItem).Symbol;
                        DragItem.Width = ((CoatingRecipe)dgv_c.SelectedItem).Name.Length * 8.4 + 65;
                        break;
                    default: break;
                }
                DragItem.Visibility = Visibility.Visible;    
            }
        }

        private bool IsOverSelectedStep(Point a, string _selectedRecipeType)
        {
            
            Point SelectedItemCoordinates;
            switch (_selectedRecipeType) 
            {
                case "A":
                    SelectedItemCoordinates = A.PointToScreen(new Point(0d, 0d));
                    if (a.X >= SelectedItemCoordinates.X && a.X <= SelectedItemCoordinates.X + A.ActualWidth && a.Y >= SelectedItemCoordinates.Y && a.Y <= SelectedItemCoordinates.Y + A.ActualHeight)
                    {
                        return true;
                    }
                    break;
                case "C":
                    Recipe_Template SelectedItem = (Recipe_Template)SV.SelectedItem;
                    if (SelectedItem != null && SelectedItem.A.IsChecked == true) 
                    {
                        SelectedItemCoordinates = SelectedItem.PointToScreen(new Point(0d, 0d));
                        if (a.X >= SelectedItemCoordinates.X && a.X <= SelectedItemCoordinates.X + SelectedItem.ActualWidth && a.Y >= SelectedItemCoordinates.Y && a.Y <= SelectedItemCoordinates.Y + SelectedItem.ActualHeight)
                        {
                            return true;
                        }
                    }
                    
                    break;
            }
            return false;
        }

        private void ItemDrop(object SelectedRecipe, string _selectedRecipeType)
        {
            switch (_selectedRecipeType)
            {
                case "A":
                    ((RecipeAdapter_MR)this.DataContext).ArticleBuffer = (ArticleRecipe)SelectedRecipe;
                    break;
                case "C":
                    Recipe_Template SelectedStep = (Recipe_Template)SV.SelectedItem;
                    if (SelectedStep != null && SelectedRecipe != null) 
                    {
                        CoatingRecipe T = (CoatingRecipe)SelectedRecipe;
                        SelectedStep.RTD = new RecipeTemplateData(SelectedStep.RTD.Id, T.Name, T.Description, SelectedRecipe, T.Symbol);

                    }
                    break;
                default: break;
            }  
        }

      

        public void BlinkONOFF()
        {
            //Task obTask = Task.Run(() =>
            //{
            //    Application.Current.Dispatcher.InvokeAsync((Action)delegate
            //    {
            //        List<Steps> steps = new List<Steps>();

            //        //steps.Add(new Steps(RC_S_14.S1_name.Value, RC_S_14.S1));
            //        //steps.Add(new Steps(RC_S_14.S2_name.Value, RC_S_14.S2));
            //        //steps.Add(new Steps(RC_S_14.S3_name.Value, RC_S_14.S3));
            //        //steps.Add(new Steps(RC_S_14.S4_name.Value, RC_S_14.S4));
            //        //steps.Add(new Steps(RC_S_58.S5_name.Value, RC_S_58.S5));
            //        //steps.Add(new Steps(RC_S_58.S6_name.Value, RC_S_58.S6));
            //        //steps.Add(new Steps(RC_S_58.S7_name.Value, RC_S_58.S7));
            //        //steps.Add(new Steps(RC_S_58.S8_name.Value, RC_S_58.S8));

            //        for (int i = 0; i<= 7; i++)
            //        {
            //            steps[i]._RB.IsBlinkEnabled = false;
            //        }

            //        SaveRecipe = true;

            //        for (int i = 7; i >= 0; i--)
            //        {
            //            if (steps[i]._Name != "-")
            //            {
            //                for (int x = 0; x <= i; x++)
            //                {
            //                    if (steps[x]._Name != "-")
            //                    {
            //                        steps[x]._RB.IsBlinkEnabled = false;
            //                    }
            //                    else
            //                    {
            //                        SaveRecipe = false;

            //                        steps[x]._RB.IsBlinkEnabled = true;
            //                    }
            //                }
            //                break;
            //            }
            //        }

            //        if (steps[0]._Name == "-")
            //        {
            //            steps[0]._RB.IsBlinkEnabled = true;
            //            SaveRecipe = false;
            //        }

            //    });
            //});
        }


        #endregion

        private void dgv_c_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dgv_c.SelectedItem != null)
                dgv_c.ScrollIntoView(dgv_c.SelectedItem);
        }

        private void dgv_a_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if(dgv_a.SelectedItem!=null)
                dgv_a.ScrollIntoView(dgv_a.SelectedItem);
        }
    }


}
