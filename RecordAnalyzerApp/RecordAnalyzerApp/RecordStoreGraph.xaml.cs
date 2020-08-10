using RecordAnalyzerApp.Controls;
using RecordAnalyzerApp.Graph;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordStoreGraph : ContentPage
    {
        public RecordStoreGraph()
        {
            InitializeComponent();
            InitializeData();
        }

        private async void InitializeData()
        {
            var fields = await SimpleGraphCriteria.MyRecordStore.GetAllFields();
            groupByColumn.SetItemsSource(fields);
            graphValueColumn.SetItemsSource(fields);

            fromDate.Date = SimpleGraphCriteria.DatePairHandler.FromNow;
            toDate.Date = SimpleGraphCriteria.DatePairHandler.ToThen;
            myDateGrouping.SelectedIndex = (int)SimpleGraphCriteria.DateGroupingCriteria;
            groupByColumn.SelectedColumnId = SimpleGraphCriteria.GroupByColumnId;
            graphValueColumn.SelectedColumnId = SimpleGraphCriteria.GraphValueFromColumnId;
            myChartType.SelectedIndex = (int)SimpleGraphCriteria.ChartType;
        }

        private void btnOk_Clicked(object sender, System.EventArgs e)
        {
            SimpleGraphCriteria.DatePairHandler.FromNow = fromDate.Date;
            SimpleGraphCriteria.DatePairHandler.ToThen = toDate.Date;
            SimpleGraphCriteria.DateGroupingCriteria = (DateGroupingOption)myDateGrouping.SelectedIndex;
            SimpleGraphCriteria.GroupByColumnId = groupByColumn.SelectedColumnId;
            SimpleGraphCriteria.GraphValueFromColumnId = graphValueColumn.SelectedColumnId;
            SimpleGraphCriteria.ChartType = (ChartType)myChartType.SelectedIndex;

            if (!IsGraphValueColumnSelected())
            {
                DisplayAlert("Error", "You must select value from column!", "OK");
                return;
            }

            Navigation.PushAsync(new RecordStoreGraphViewer());
        }

        private bool IsGraphValueColumnSelected()
        {
            return graphValueColumn.SelectedColumnId > 0;
        }
    }
}