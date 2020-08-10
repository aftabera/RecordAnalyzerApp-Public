using RecordAnalyzerApp.Reporting;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordStoreReports : ContentPage
    {
        public RecordStoreReports()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            sortByColumn.SetItemsSource(await SimpleReportCriteria.MyRecordStore.GetAllFields());
            filterByColumn1.SetItemsSource(await SimpleReportCriteria.MyRecordStore.GetAllFields());
            filterByColumn2.SetItemsSource(await SimpleReportCriteria.MyRecordStore.GetAllFields());

            myTitle.Text = SimpleReportCriteria.Title;
            fromDate.Date = SimpleReportCriteria.DatePairHandler.FromNow;
            toDate.Date = SimpleReportCriteria.DatePairHandler.ToThen;
            sortByColumn.SelectedColumnId = SimpleReportCriteria.SortByColumnId;
            sortByDescending.On = SimpleReportCriteria.Descending;
            showCriteria.On = SimpleReportCriteria.ShowCriteria;
            showRecordDate.On = SimpleReportCriteria.ShowRecordDate;
            showRecordTime.On = SimpleReportCriteria.ShowRecordTime;
            showData.On = SimpleReportCriteria.ShowData;
            showSummary.On = SimpleReportCriteria.ShowSummary;
            showCurrentDate.On = SimpleReportCriteria.ShowCurrentDate;
            showCurrentTime.On = SimpleReportCriteria.ShowCurrentTime;

            //1st Filter options
            filterByColumn1.SelectedColumnId = SimpleReportCriteria.FilterByColumnId1;
            filterComparison1.SelectedIndex = SimpleReportCriteria.FilterComparisonId1;
            filterText1.Text = SimpleReportCriteria.FilterByText1;

            //2nd Filter options
            filterByColumn2.SelectedColumnId = SimpleReportCriteria.FilterByColumnId2;
            filterComparison2.SelectedIndex = SimpleReportCriteria.FilterComparisonId2;
            filterText2.Text = SimpleReportCriteria.FilterByText2;

            base.OnAppearing();
        }

        private void btnOk_Clicked(object sender, System.EventArgs e)
        {
            ReadUserInput();
            Navigation.PushAsync(new RecordStoreReportsViewer());
        }

        private void ReadUserInput()
        {
            SimpleReportCriteria.Title = myTitle.Text;
            SimpleReportCriteria.DatePairHandler.FromNow = fromDate.Date;
            SimpleReportCriteria.DatePairHandler.ToThen = toDate.Date;
            SimpleReportCriteria.SortByColumnId = sortByColumn.SelectedColumnId;
            SimpleReportCriteria.Descending = sortByDescending.On;
            SimpleReportCriteria.ShowCriteria = showCriteria.On;
            SimpleReportCriteria.ShowRecordDate = showRecordDate.On;
            SimpleReportCriteria.ShowRecordTime = showRecordTime.On;
            SimpleReportCriteria.ShowData = showData.On;
            SimpleReportCriteria.ShowSummary = showSummary.On;
            SimpleReportCriteria.ShowCurrentDate = showCurrentDate.On;
            SimpleReportCriteria.ShowCurrentTime = showCurrentTime.On;

            //1st Filter options
            SimpleReportCriteria.FilterByColumnId1 = filterByColumn1.SelectedColumnId;
            SimpleReportCriteria.FilterComparisonId1 = filterComparison1.SelectedIndex;
            SimpleReportCriteria.FilterByText1 = filterText1.Text;

            //2nd Filter options
            SimpleReportCriteria.FilterByColumnId2 = filterByColumn2.SelectedColumnId;
            SimpleReportCriteria.FilterComparisonId2 = filterComparison2.SelectedIndex;
            SimpleReportCriteria.FilterByText2 = filterText2.Text;
        }
    }
}