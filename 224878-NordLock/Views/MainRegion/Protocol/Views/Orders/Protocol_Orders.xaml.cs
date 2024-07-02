using HMI.Views.DialogRegion;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
	/// <summary>
	/// Interaction logic for Protocol.xaml
	/// </summary>
	[ExportView("Protocol_Orders")]
	public partial class Protocol_Orders : VisiWin.Controls.View
	{
        int oldIndex = -1;
		public Protocol_Orders()
		{
			this.InitializeComponent();
		}

		private void LayoutRoot_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
            if (this.IsVisible)
            {
                if (dgv_orders.Items.Count >= 1)
                    dgv_orders.SelectedIndex = oldIndex;
                else
                    dgv_orders.SelectedIndex = -1;

                List<object> paramList = new List<object>();
                paramList.Add(oldIndex);
                paramList.Add(dgv_orders);
              
                ((ProtocolAdapter)this.DataContext).BW_FilterSQL(paramList);
               
            }
		}

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            if (DialogView.Show("Protocol_Filter", "@Protocol.Text5", DialogButton.OKCancel, DialogResult.Cancel) == DialogResult.OK)
            {
                ProtocolAdapter temp = (ProtocolAdapter)this.DataContext;
                temp.BW_FilterSQL(null);
            }
        }

      
        private void DataGridRow_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dgv_orders.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
            oldIndex = dgv_orders.SelectedIndex;
        }
    }
}