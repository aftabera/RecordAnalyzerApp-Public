using RecordAnalyzerApp.Common;
using RecordAnalyzerApp.Model;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColumnListCell : ViewCell
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(ColumnListCell));

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

        List<PickerListItem> _list = new List<PickerListItem>();

        public ColumnListCell()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public void SetItemsSource(List<RecordStoreDataType> list)
        {
            foreach (var item in list)
            {
                _list.Add(new PickerListItem
                {
                    Id = item.Id,
                    Title = item.Title
                });
            }

            if (_list.Count == 0)
            {
                _list.Add(new PickerListItem
                {
                    Id = 0,
                    Title = "None"
                });
            }

            myPicker.ItemsSource = _list;
            myPicker.SelectedIndex = 0;
        }

        public int SelectedColumnId 
        { 
            get
            {
                return (myPicker.SelectedItem as PickerListItem).Id;
            }
            set
            {
                try
                {
                    var index = _list.FindIndex(p => p.Id == value);
                    myPicker.SelectedIndex = index >= 0 ? index : 0;
                }
                catch 
                {
                    myPicker.SelectedIndex = 0;
                }
            }
        }

        internal bool HasItems()
        {
            return myPicker.Items.Count > 0;
        }
    }
}