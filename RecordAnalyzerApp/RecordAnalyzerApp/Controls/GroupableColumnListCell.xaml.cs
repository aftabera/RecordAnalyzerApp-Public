using RecordAnalyzerApp.Common;
using RecordAnalyzerApp.Model;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroupableColumnListCell : ViewCell
    {
        List<PickerListItem> _list = new List<PickerListItem>();

        public GroupableColumnListCell()
        {
            InitializeComponent();
        }

        public void SetItemsSource(List<RecordStoreDataType> list)
        {
            _list.Add(new PickerListItem
            {
                Id = 0,
                Title = "None"
            });

            foreach (var item in list)
            {
                if (item.IsGroupableDataType())
                {
                    _list.Add(new PickerListItem
                    {
                        Id = item.Id,
                        Title = item.Title
                    });
                }
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