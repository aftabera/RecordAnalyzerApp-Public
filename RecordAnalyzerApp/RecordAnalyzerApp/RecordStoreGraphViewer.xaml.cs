using Microcharts;
using RecordAnalyzerApp.Graph;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Entry = Microcharts.Entry;

namespace RecordAnalyzerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordStoreGraphViewer : ContentPage
    {
        SKImage _img = null;

        public RecordStoreGraphViewer()
        {
            InitializeComponent();

            myChartView.PaintSurface += MyChartView_PaintSurface;
        }

        private void MyChartView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            _img = e.Surface.Snapshot();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            myActivityIndicator.IsVisible = myActivityIndicator.IsRunning = true;

            await ShowGraph();

            myActivityIndicator.IsVisible = myActivityIndicator.IsRunning = false;
        }

        private async Task ShowGraph()
        {
            var myEntries = await GetEntries();

            if (myEntries.Count == 0)
            {
                await DisplayAlert("Error", "No data found!", "OK");
                await Navigation.PopAsync();

                return;
            }

            Chart myChart = new BarChart();

            switch (SimpleGraphCriteria.ChartType)
            {
                case ChartType.PointChart:
                    myChart = new PointChart();
                    break;
                case ChartType.LineChart:
                    myChart = new LineChart();
                    break;
                case ChartType.DonutChart:
                    myChart = new DonutChart();
                    break;
                case ChartType.RadiaGaugeChart:
                    myChart = new RadialGaugeChart();
                    break;
                case ChartType.RadarChart:
                    myChart = new RadarChart();
                    break;
                case ChartType.BarChart:
                default:
                    break;
            }

            myChart.Entries = myEntries;
            myChart.LabelTextSize = 30;

            myChartView.Chart = myChart;
        }

        private async Task<List<Entry>> GetEntries()
        {
            var dataBuilder = new SimpleGraphDataBuilder();
            var graphData = await dataBuilder.BuildReportData();

            List<Entry> list = new List<Entry>();

            if (graphData != null)
            {
                foreach (var item in graphData)
                {
                    list.Add(new Entry(item.Value)
                    {
                        Label = item.Title,
                        ValueLabel = item.Value.ToString(),
                        Color = SKColor.Parse(ColorHexStringProvider.NextColorString()),
                    });
                }
            }

            list.TrimExcess();
            return list;
        }

        private async void shareButton_Clicked(object sender, EventArgs e)
        {
            await SaveAndShare();
        }

        private async Task SaveAndShare()
        {
            if (_img == null) return;

            //var fn = string.Format("{0}.png", DateTime.Now.Ticks);
            var fn = "Grpah.png";
            var file = Path.Combine(FileSystem.CacheDirectory, fn);

            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                using (_img)
                {
                    var data = _img.Encode();

                    using (var output = new BinaryWriter(File.Open(file, FileMode.Create)))
                    {
                        output.Write(data.ToArray());
                    }
                }

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = fn,
                    File = new ShareFile(file)
                });
            }
            catch { }
        }
    }
}