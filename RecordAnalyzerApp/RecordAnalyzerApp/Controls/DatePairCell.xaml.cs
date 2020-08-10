using RecordAnalyzerApp.Common;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatePairCell : ViewCell
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

        DatePairHandler _datePairHandler;

        public DatePairCell()
        {
            InitializeComponent();
            BindingContext = this;

            datePairPicker.ItemsSource = Enum.GetValues(typeof(DatePairOption));
            datePairPicker.SelectedIndex = 0;
        }

        public void SetDatePairHandler(DatePairHandler handler)
        {
            _datePairHandler = handler;
        }

        private void datePairPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (datePairPicker.SelectedItem != null)
                _datePairHandler.UpdateDatePeriod(datePairPicker.SelectedItem.ToString());            
        }
    }

    public enum DatePairOption
    {
        Today,
        Yesterday,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth,
        ThisYear,
        LastYear,
        ALL
    }
}