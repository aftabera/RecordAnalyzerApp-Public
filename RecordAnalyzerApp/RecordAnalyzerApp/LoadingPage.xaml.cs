using RecordAnalyzerApp.Persistance;
using System.ComponentModel;
using Xamarin.Forms;

namespace RecordAnalyzerApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await MyDB.InitializeDatabase();
            await Navigation.PushAsync(new RecordStoreList());

            Navigation.RemovePage(this);
        }
    }
}
