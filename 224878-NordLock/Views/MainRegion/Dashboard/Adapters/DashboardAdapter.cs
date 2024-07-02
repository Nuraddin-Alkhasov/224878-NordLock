//*****************************************************************************
//	notwendige Dateien: 
//	-----------------------------------------------------------------------------
//	DashboardAdapter.cs							--> dieser Adapter
//	WidgetSelectionView.xaml(.cs)				-->	Dialog zur Widget-Auswahl
//	WidgetEmptyView.xaml(.cs)					-->	Platzhalter für nicht vorhandene Widgets
//  DialogView.xaml(.cs)                        --> Zur Dialoganzeige
//
//*****************************************************************************

using HMI.Views.DialogRegion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.Controls;

namespace HMI.Dashboard
{
    /// <summary>
    /// Der DashboardAdapter stellt die im Projekt vorhandenen DashboardWidgets zur Verfügng.
    /// Der AddDashboardWidgetCommand des Dashboard-Controls ist an den gleichnamigen Command dieses
    /// Adapters gebunden und zeigt die View zur Widget-Auswahl (WidgetSelectionView) als Dialog an.
    /// </summary>
    [ExportAdapter("DashboardAdapter")]
    public class DashboardAdapter : AdapterBase
    {
        /// <summary>
        /// Der Name der WidgetSelectionView
        /// </summary>
        private const string WidgetSelectionViewName = "widgetSelectionView";

        /// <summary>
        /// DependencyProperty für die <see cref="AddDashboardWidgetCommand" />-Eigenschaft
        /// </summary>
        public static readonly DependencyProperty AddDashboardWidgetCommandProperty = DependencyProperty.Register("AddDashboardWidgetCommand", typeof(ActionCommand), typeof(DashboardAdapter), new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty für die <see cref="AvailableDashboardWidgets" />-Eigenschaft
        /// </summary>
        public static readonly DependencyProperty AvailableDashboardWidgetsProperty = DependencyProperty.Register("AvailableDashboardWidgets", typeof(ICollectionView), typeof(DashboardAdapter), new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty für die <see cref="SelectedDashboardWidget" />-Eigenschaft
        /// </summary>
        public static readonly DependencyProperty SelectedDashboardWidgetProperty = DependencyProperty.Register("SelectedDashboardWidget", typeof(IDashboardWidgetMetadata), typeof(DashboardAdapter), new PropertyMetadata(null, OnSelectedDashboardWidgetPropertyChanged));

        /// <summary>
        /// DependencyProperty für die <see cref="IsSizeIncorrect" />-Eigenschaft
        /// </summary>
        public static readonly DependencyProperty IsSizeIncorrectProperty = DependencyProperty.Register("IsSizeIncorrect", typeof(bool), typeof(DashboardAdapter), new PropertyMetadata(false));

        /// <summary>
        /// Verweis auf den DashboardWidgetCommandParameter
		/// Dieser wird für das AddDashboardWidget-Kommando vom Dashboard-Control benötigt
        /// </summary>
        private DashboardWidgetCommandParameter widgetCommandParameter;

        /// <summary>
        /// Gib an, ob die Widget-Liste bereits befüllt wurde
        /// </summary>
        private bool widgetListCreated;

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="DashboardAdapter" /> Klasse
        /// </summary>
        public DashboardAdapter()
        {
            this.AddDashboardWidgetCommand = new ActionCommand(this.OnAddDashboardWidgetCommand);
        }

        /// <summary>
        /// Holt oder setzt das AddDashboardWidget-Kommando
        /// </summary>
        public ActionCommand AddDashboardWidgetCommand
        {
            get { return (ActionCommand)this.GetValue(AddDashboardWidgetCommandProperty); }
            set { this.SetValue(AddDashboardWidgetCommandProperty, value); }
        }

        /// <summary>
        /// Holt oder setzt die gruppierte Liste der verfügbaren Widgets
        /// </summary>
        public ICollectionView AvailableDashboardWidgets
        {
            get { return (ICollectionView)this.GetValue(AvailableDashboardWidgetsProperty); }
            set { this.SetValue(AvailableDashboardWidgetsProperty, value); }
        }

        /// <summary>
        /// Bereitstellung aller im Projekt vorhandenen DashboardWidgets
        /// </summary>
        [ImportMany(typeof(IView))]
        public IEnumerable<Lazy<IView, IDashboardWidgetMetadata>> ImportedDashboardWidgetsLazy { get; set; }

        /// <summary>
        /// Gibt an, ob das SelectedDashboardWidget zu groß für die aktuelle Position ist
        /// </summary>
        public bool IsSizeIncorrect
        {
            get { return (bool)this.GetValue(IsSizeIncorrectProperty); }
            set { this.SetValue(IsSizeIncorrectProperty, value); }
        }

        /// <summary>
        /// Holt oder setzt das selektierte DashboardWidget
        /// </summary>
        public IDashboardWidgetMetadata SelectedDashboardWidget
        {
            get { return (IDashboardWidgetMetadata)this.GetValue(SelectedDashboardWidgetProperty); }
            set { this.SetValue(SelectedDashboardWidgetProperty, value); }
        }

        /// <summary>
        /// Wird ausgeführt, wenn die View zur Kommunikation angemeldet wird
        /// </summary>
        /// <param name="view">Die angemeldete View</param>
        public override void OnViewAttached(IView view)
        {
            base.OnViewAttached(view);

            if (view.Name == WidgetSelectionViewName)
            {
                if (!this.widgetListCreated)
                {
                    this.FillWidgetList();
                }

                if (this.SelectedDashboardWidget != null)
                {
                    ApplicationService.SetView("DashboardPreviewRegion", this.SelectedDashboardWidget.ViewName);
                }
            }
        }

        /// <summary>
        /// Wird ausgeführt, wenn sich der Wert der <see cref="SelectedDashboardWidget" /> Eigenschaft ändert
        /// </summary>
        /// <param name="d">DependencyObject, welches das Ereignis betrifft</param>
        /// <param name="e">Details zur Änderung</param>
        private static void OnSelectedDashboardWidgetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (DashboardAdapter)d;
            instance.OnSelectedDashboardWidgetChanged();
        }

        /// <summary>
        /// Auf korrekte Auswahl eines Widgets prüfen
        /// </summary>
        /// <param name="sender">Objekt, welches das Ereignis betrifft</param>
        /// <param name="e">Details zur Änderung</param>
        private void CheckDashboardWidgetSize(object sender, DialogResultEventArgs e)
        {
            if (e.Result == DialogResult.OK)
            {
                if (this.IsSizeIncorrect)
                {
                    e.CancelDialogClosing = true;
                }
            }
        }

        /// <summary>
        /// Befüllt die Widget-Liste und gruppiert diese nach der angegebenen Kategorie
        /// </summary>
        private void FillWidgetList()
        {
            var collectionView = new ListCollectionView(this.ImportedDashboardWidgetsLazy.Select(import => import.Metadata).ToList());
            if (collectionView.GroupDescriptions != null)
            {
                collectionView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            }

            this.AvailableDashboardWidgets = collectionView;
            this.widgetListCreated = true;
        }

        /// <summary>
        /// Wird ausgeführt, wenn das AddDashboardWidget-Kommando ausgelöst wurde
        /// </summary>
        private void OnAddDashboardWidgetCommand(object obj)
        {
            this.widgetCommandParameter = obj as DashboardWidgetCommandParameter;

            this.SetIsSizeIncorrect();

            if (this.widgetCommandParameter != null &&
                DialogView.Show("DB_WidgetSelector", "@Dashboard.AddWidget", VerifyDialogResultFunction: this.CheckDashboardWidgetSize) == DialogResult.OK)
            {
                this.widgetCommandParameter.ViewName = this.SelectedDashboardWidget.ViewName;
            }
        }

        /// <summary>
        /// Wird ausgeführt, wenn sich das SelectedDashboardWidget geändert hat
        /// </summary>
        private void OnSelectedDashboardWidgetChanged()
        {
            if (this.SelectedDashboardWidget != null && this.widgetListCreated)
            {
                ApplicationService.SetView("DashboardPreviewRegion", this.SelectedDashboardWidget.ViewName);
                this.SetIsSizeIncorrect();
            }
        }

        /// <summary>
        /// Ermittelt, ob das ausgewählte Widget zu groß für die aktuelle Position ist
        /// </summary>
        private void SetIsSizeIncorrect()
        {
            if (this.SelectedDashboardWidget != null && this.widgetCommandParameter != null)
            {
                this.IsSizeIncorrect = !VisiWin.Controls.Dashboard.CheckIsSizeSuitable(this.SelectedDashboardWidget, this.widgetCommandParameter);
            }
        }
    }
}