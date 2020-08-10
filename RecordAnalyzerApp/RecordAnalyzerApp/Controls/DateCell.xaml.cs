using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DateCell : ViewCell
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(DateCell));

        public static readonly BindableProperty DateProperty =
            BindableProperty.Create("Date", typeof(DateTime), typeof(DateCell));

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public DateTime Date
        {
            get
            {
                return (DateTime)GetValue(DateProperty);
            }
            set
            {
                SetValue(DateProperty, value);
            }
        }

        public DateCell()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}