using RecordAnalyzerApp.Graph;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartTypeCell : ViewCell
    {
        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create("SelectedIndex", typeof(int), typeof(ChartTypeCell));

        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        public ChartTypeCell()
        {
            InitializeComponent();
            BindingContext = this;

            myPicker.ItemsSource = Enum.GetValues(typeof(ChartType));
            myPicker.SelectedIndex = 0;
        }
    }
}