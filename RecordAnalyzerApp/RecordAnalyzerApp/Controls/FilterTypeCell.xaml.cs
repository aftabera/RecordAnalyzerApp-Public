using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterTypeCell : ViewCell
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

        public FilterTypeCell()
        {
            InitializeComponent();
            BindingContext = this;

            myPicker.ItemsSource = Enum.GetValues(typeof(FilterOptions));
            myPicker.SelectedIndex = 0;
        }
    }

    public enum FilterOptions
    {
        EqualTo,
        NotEqualTo,
        LessThan,
        LessThanAndEqualTo,
        GreatorThan,
        GreatorThanAndEqualTo,
        Contains,
        NotContains,
        BeginsWith,
        EndsWith
    }
}