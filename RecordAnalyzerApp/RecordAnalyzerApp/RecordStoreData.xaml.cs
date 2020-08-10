using RecordAnalyzerApp.Common;
using RecordAnalyzerApp.Model;
using RecordAnalyzerApp.Persistance;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordStoreData : ContentPage
    {
        RecordStore _store;
        DatePairHandler _handler;

        public RecordStoreData()
        {
            InitializeComponent();

            MyAdView.IsVisible = !Values.PRO_VERSION;
        }

        public RecordStoreData(RecordStore store)
            : this()
        {
            _store = store;
            Title = _store.StoreName;

            _handler = new DatePairHandler();
            _handler.DatePeriodChanged += _handler_DatePeriodChanged;       
            _handler.CreateAndHandleClickEvent(btnToday);
            _handler.CreateAndHandleClickEvent(btnYesterday);
            _handler.CreateAndHandleClickEvent(btnThisWeek);
            _handler.CreateAndHandleClickEvent(btnLastWeek);
            _handler.CreateAndHandleClickEvent(btnThisMonth);
            _handler.CreateAndHandleClickEvent(btnLastMonth);
            _handler.CreateAndHandleClickEvent(btnThisYear);
            _handler.CreateAndHandleClickEvent(btnLastYear);
            _handler.CreateAndHandleClickEvent(btnAll);
        }

        private async void _handler_DatePeriodChanged(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var masterData = await MyDB.C.Table<RecordStoreDataMaster>()
                    .Where(m => (m.RecordStoreId == _store.Id && m.MasterDateTime >= fromDate && m.MasterDateTime <= toDate))
                    .OrderByDescending(m => m.MasterDateTime)
                    .ToListAsync();

                myListView.ItemsSource = masterData;
            }
            catch { }
        }

        protected async override void OnAppearing()
        {
            if (string.IsNullOrWhiteSpace(searchText.Text))
            {
                await FindAndShowData();
            }
            else
            {
                await FilterDataBySearchText();
            }
            base.OnAppearing();
        }

        private async Task FindAndShowData()
        {
            try
            {
                var masterData = await MyDB.C.Table<RecordStoreDataMaster>()
                .Where(m => m.RecordStoreId == _store.Id)
                .OrderByDescending(m => m.Id)
                .Take(99)
                .ToListAsync();

                myListView.ItemsSource = masterData;
            }
            catch { }
        }

        private async void btnNew_Clicked(object sender, EventArgs e)
        {
            var form = new RecordStoreDataForm(new RecordStoreDataMaster() { RecordStoreId = _store.Id }) { Title = _store.StoreName };
            await Navigation.PushModalAsync(form, true);
        }

        private async void myListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var masterData = e.Item as RecordStoreDataMaster;
            var form = new RecordStoreDataForm(masterData) { Title = _store.StoreName };
            await Navigation.PushModalAsync(form, true);
        }

        private async void searchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchText.Text))
            {
                await FindAndShowData();
            }
        }

        private async void searchText_SearchButtonPressed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchText.Text))
            {
                return;
            }
            await FilterDataBySearchText();
        }

        private async Task FilterDataBySearchText()
        {
            try
            {
                var masterData = await MyDB.C.Table<RecordStoreDataMaster>()
                .Where(m => m.RecordStoreId == _store.Id && m.PrimaryValue.ToLower().Contains(searchText.Text.ToLower()))
                .OrderByDescending(m => m.Id)
                .ToListAsync();

                myListView.ItemsSource = masterData;
            }
            catch { }
        }
    }
}