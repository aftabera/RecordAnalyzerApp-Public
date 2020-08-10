using RecordAnalyzerApp.Model;
using RecordAnalyzerApp.Persistance;
using System;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordStoreEditor : ContentPage
    {
        RecordStore _store;
        ObservableCollection<RecordStoreDataType> _collection;

        public RecordStoreEditor()
        {
            InitializeComponent();
        }

        public RecordStoreEditor(RecordStore store)
            : this()
        {
            _store = store;
            Title = store.StoreName;
        }

        protected async override void OnAppearing()
        {
            var c = await MyDB.C.Table<RecordStoreDataType>().Where(m=> m.RecordStoreId == _store.Id).ToListAsync();

            _collection = new ObservableCollection<RecordStoreDataType>(c);
            myListView.ItemsSource = _collection;

            base.OnAppearing();
        }

        private async void btnNew_Clicked(object sender, EventArgs e)
        {
            var type = new RecordStoreDataType();
            type.RecordStoreId = _store.Id;

            var page = new RecordStoreColumn(type);

            page.ColumnNameExists += Page_ColumnNameExists; 

            page.DataTypeCreated += (RecordStoreDataType value) => _collection.Add(value);

            await Navigation.PushModalAsync(page);
        }

        private bool Page_ColumnNameExists(int id, string arg)
        {
            foreach (var item in _collection)
            {
                if (item.Id != id && string.Compare(item.Title, arg, true) == 0)
                    return true;
            }
            return false;
        }

        private async void myListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var type = e.Item as RecordStoreDataType;

            var page = new RecordStoreColumn(type);

            page.ColumnNameExists += Page_ColumnNameExists;

            await Navigation.PushModalAsync(page);
        }
    }
}