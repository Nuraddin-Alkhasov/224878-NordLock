using HMI.UserControls;
using System;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using RadioButton = VisiWin.Controls.RadioButton;

namespace HMI.Views.MainRegion.Recipe
{
    /// <summary>
    /// Interaction logic for BSStepEdit.xaml
    /// </summary>
    [ExportView("Recipe_Coating_PR")]
    public partial class Recipe_Coating_PR : VisiWin.Controls.View
    {
        Point _lastTapLocation_;
        public bool SaveRecipe = true;
        IRegionService iRS = ApplicationService.GetService<IRegionService>();
        public Recipe_Coating_PR()
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

        private void List_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            _lastTapLocation_ = e.GetTouchPoint(this).Position;
        }

        private void List_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            if (IsOverSelectedStep(DragItem.PointToScreen(new Point(0d, 0d))))
            {
                switch (((RadioButton)sender).Tag.ToString()) 
                {
                    case "D": ItemDrop((CoatingStepRecipe)dgv_d.SelectedItem); break;
                    case "S": ItemDrop((CoatingStepRecipe)dgv_s.SelectedItem); break;
                    case "T": ItemDrop((CoatingStepRecipe)dgv_t.SelectedItem); break;
                    default:break;
                }
            }
            DragItem.Visibility = Visibility.Hidden;
        }

        private void List_PreviewTouchMove(object sender, TouchEventArgs e)
        {
                switch (((RadioButton)sender).Tag.ToString()) 
                {
                    case "D": RecalculatePosition(e.GetTouchPoint(this).Position, (object)dgv_d.SelectedItem); break;
                    case "S": RecalculatePosition(e.GetTouchPoint(this).Position, (object)dgv_s.SelectedItem); break;
                    case "T": RecalculatePosition(e.GetTouchPoint(this).Position, (object)dgv_t.SelectedItem); break;
                default: break;
            }
        }

        private void List_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (((RadioButton)sender).Tag.ToString())
            {
                case "D": ItemDrop((CoatingStepRecipe)dgv_d.SelectedItem); break;
                case "S": ItemDrop((CoatingStepRecipe)dgv_s.SelectedItem); break;
                case "T": ItemDrop((CoatingStepRecipe)dgv_t.SelectedItem); break;
                default: break;
            }
        }


        #endregion

        #region - - - Methods - - -
        private bool IsOverSelectedStep(Point a)
        {
            Recipe_Template SelectedItem = (Recipe_Template)SV.SelectedItem;
            if (SelectedItem != null && SelectedItem.A.IsChecked == true)
            {
                Point SelectedItemCoordinates = SelectedItem.PointToScreen(new Point(0d, 0d));
                if (a.X >= SelectedItemCoordinates.X && a.X <= SelectedItemCoordinates.X + SelectedItem.ActualWidth && a.Y >= SelectedItemCoordinates.Y && a.Y <= SelectedItemCoordinates.Y + SelectedItem.ActualHeight)
                {
                    return true;
                }
            }
            return false;
        }

        private void ItemDrop(CoatingStepRecipe SelectedRecipe)
        {
            Recipe_Template SelectedStep = (Recipe_Template)SV.SelectedItem;
            if (SelectedStep != null && SelectedRecipe!=null)
            {
                SelectedStep.RTD = new RecipeTemplateData(SelectedStep.RTD.Id, SelectedRecipe.Name, SelectedRecipe.VWR.Description, SelectedRecipe, SelectedRecipe.Symbol);
            }
        }

        private void RecalculatePosition(Point a, object _SelectedItem)
        {
            DragItem.Margin = new Thickness(a.X - 30, a.Y - 70, 0, 0);

            if (Math.Abs(a.X - _lastTapLocation_.X) > 25 && DragItem.Visibility == Visibility.Hidden)
            {
                if (_SelectedItem != null)
                {
                    DragItem_Name.Text = ((CoatingStepRecipe)_SelectedItem).Name;
                    DragItem_Pic.SymbolResourceKey = ((CoatingStepRecipe)_SelectedItem).Symbol;
                    DragItem.Width = ((CoatingStepRecipe)_SelectedItem).Name.Length * 8.4 + 65;
                    DragItem.Visibility = Visibility.Visible;
                }
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

        private void _MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Recipe_Coating_PN R_CPN = (Recipe_Coating_PN)iRS.GetView("Recipe_Coating_PN");
            R_CPN.pn_coating.ScrollNext();
        }

        private void dgv_d_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            if (dgv_d.SelectedItem != null)
                dgv_d.ScrollIntoView(dgv_d.SelectedItem);
        }

        private void dgv_s_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            if (dgv_s.SelectedItem != null)
                dgv_s.ScrollIntoView(dgv_s.SelectedItem);
        }

        private void dgv_t_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            if (dgv_t.SelectedItem != null)
                dgv_t.ScrollIntoView(dgv_t.SelectedItem);
        }

        private void cb1_Loaded(object sender, RoutedEventArgs e)
        {
            ((RecipeAdapter_Coating)((Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR")).DataContext).UpdatePaintTypes();
        }
    } 
}