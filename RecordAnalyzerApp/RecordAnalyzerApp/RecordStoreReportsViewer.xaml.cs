using RecordAnalyzerApp.Reporting;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordStoreReportsViewer : ContentPage
    {
        public RecordStoreReportsViewer()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            myActivityIndicator.IsVisible = myActivityIndicator.IsRunning = true;

            await ShowReport();

            myActivityIndicator.IsVisible = myActivityIndicator.IsRunning = false;
        }

        private async Task ShowReport()
        {
            var htmlSource = new HtmlWebViewSource();

            var data = new SimpleReportDataBuilder();
            await data.BuildReportData();

            var report = new SimpleReportBuilder(data);
            await report.BuildReport();

            htmlSource.Html = report.ToString();

            myWebView.Source = htmlSource;
        }

        private async void shareButton_Clicked(object sender, EventArgs e)
        {
            await SaveAndShareAsFile();
            /*
            string shareAsText = "Share as Text";
            string shareAsFile = "Share as File";
            
            string result = await DisplayActionSheet("Share Options", "Cancel", string.Empty, shareAsText, shareAsFile);

            if (result == shareAsText)
            {
                await SaveAndShareAsText();
            }
            else if (result == shareAsFile)
            {
                await SaveAndShareAsFile();
            }
            //*/
        }

        //private async Task SaveAndShareAsText()
        //{
        //    await Share.RequestAsync(new ShareTextRequest
        //    {
        //        Title = "Share Web Link",
        //        Text = (myWebView.Source as HtmlWebViewSource).Html,     
        //    });
        //}

        private async Task SaveAndShareAsFile()
        {
            //var fn = string.Format("{0}.htm", DateTime.Now.Ticks);
            var fn = "Report.htm";
            var file = Path.Combine(FileSystem.CacheDirectory, fn);
            File.WriteAllText(file, (myWebView.Source as HtmlWebViewSource).Html);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = string.IsNullOrWhiteSpace(SimpleReportCriteria.Title) ? "Untitled" : SimpleReportCriteria.Title,
                File = new ShareFile(file)
            });
        }
    }
}