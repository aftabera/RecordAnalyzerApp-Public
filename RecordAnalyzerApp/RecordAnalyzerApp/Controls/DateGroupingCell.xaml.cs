using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DateGroupingCell : ViewCell
    {
        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create("SelectedIndex", typeof(int), typeof(DatePairCell));

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

        public DateGroupingCell()
        {
            InitializeComponent();
            BindingContext = this;

            myPicker.ItemsSource = Enum.GetValues(typeof(DateGroupingOption));
            myPicker.SelectedIndex = 0;
        }
    }

    public enum DateGroupingOption
    {
        None,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}