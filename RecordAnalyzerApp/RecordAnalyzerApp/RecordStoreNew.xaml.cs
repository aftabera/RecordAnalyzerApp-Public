using RecordAnalyzerApp.Model;
using RecordAnalyzerApp.Persistance;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordStoreNew : ContentPage
    {
        RecordStore _store = null;

        public RecordStoreNew()
        {
            InitializeComponent();

            MyAdView.IsVisible = !Common.Values.PRO_VERSION;

#if DEBUG
            //viewDataBG.Source = ResourcesProvider.ImageResourceHelper.GetFileImageSource(1000);
            //viewDataBG.Source = "FirstButton.png";
#endif


            _store = new RecordStore();
            BindingContext = _store;

            ShowHideButtons(false);
        }

        //private void InitializeFonts()
        //{
        //    double fontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label));
        //    txtStoreName.FontSize = fontSize;
        //    //btnViewData.FontSize = fontSize;
        //    //btnEditTypes.FontSize = fontSize;
        //    //btnViewReports.FontSize = fontSize;
        //    //btnViewGraphs.FontSize = fontSize;
        //}

        public RecordStoreNew(RecordStore store)
            :this()
        { 
            _store = store;
            BindingContext = _store;
            btnSaveStore.Text = "Update";
            Title = _store.StoreName;

            ShowHideButtons(true);
        }

        void ShowHideButtons(bool show)
        {
            foreach (var item in myLayout.Children)
            {
                if (item is Entry || item is Controls.AdControlView) { }
                else
                {
                    item.IsVisible = show;
                }
            }
        }

        private async void btnSaveStore_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStoreName.Text))
            {
                await DisplayAlert("Error", "You must enter record store name!", "OK");
                return;
            }
           
            if (_store.Id == 0)
            {
                await MyDB.C.InsertAsync(_store);
                //await Navigation.PushAsync(new RecordStoreEditor(_store), true);
                //Navigation.RemovePage(this);
            }
            else
            {
                _store.StoreName = txtStoreName.Text;
                await MyDB.C.UpdateAsync(_store);                
            }

            await Navigation.PopAsync();
        }

        private async void btnViewData_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecordStoreData(_store), true);
        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
            if (_store.Id > 0)
            {
                bool result = await DisplayAlert("Confirmation", "Are you sure you want to delete. This cannot be restored?", "Yes", "No");
                if (result)
                {
                    int count = await RecordStore.Delete(_store);
                    if (count > 0)
                    {
                        await Navigation.PopAsync();
                    }
                }
            }
        }

        private async void btnEditTypes_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecordStoreEditor(_store), true);
        }

        private async void btnViewReports_Clicked(object sender, EventArgs e)
        {
            if (await _store.HasData())
            {
                Reporting.SimpleReportCriteria.MyRecordStore = _store;
                await Navigation.PushAsync(new RecordStoreReports());
            }
            else
            {
                await DisplayAlert(string.Empty, "No data found!", "OK");
            }
        }

        private async void btnViewGraphs_Clicked(object sender, EventArgs e)
        {
            if (await _store.HasData())
            {
                Graph.SimpleGraphCriteria.MyRecordStore = _store;
                await Navigation.PushAsync(new RecordStoreGraph());
            }
            else
            {
                await DisplayAlert(string.Empty, "No data found!", "OK");
            }
        }
    }
}