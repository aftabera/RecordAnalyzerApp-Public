using RecordAnalyzerApp.Model;
using RecordAnalyzerApp.Persistance;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordStoreColumn : ContentPage
    {
        const int _START_INDEX = 2;

        public event Action<RecordStoreDataType> DataTypeCreated = null;
        public event Func<int, string, bool> ColumnNameExists = null;

        RecordStoreDataType _model;        
        int _index = _START_INDEX;
        bool _flag = false;

        public RecordStoreColumn()
        {
            InitializeComponent();
            dataTypePicker.ItemsSource = Enum.GetValues(typeof(DataTypes));
        }

        public RecordStoreColumn(RecordStoreDataType model)
            : this()
        {
            _model = model;
            _flag = (_model.DataTypeId == (int)DataTypes.List);
            BindingContext = _model;
        }

        protected async override void OnAppearing()
        {
            await ShowListDataIfApplicable();

            base.OnAppearing();
        }

        private async Task ShowListDataIfApplicable()
        {
            if (_model.Id > 0 && IsListDataTypeSelected())
            {
                var listData = await RecordStoreDataTypeList.GetByRecordStoreDataTypeId(_model.Id);
                int count = 0;
                foreach (var item in listData)
                {
                    count++;
                    AddRowForListItem(item.ListElementTitle, count < listData.Count);
                }
            }
        }

        private async void okButton_Clicked(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var listItems = GetListItemsList();

            int count;

            if (_model.Id == 0)
            {
                count = await MyDB.C.InsertAsync(_model);
            }
            else
            { 
                count = await MyDB.C.UpdateAsync(_model);

                // delete existing list data if applicable
                if (listItems.Count > 0)
                {
                    await MyDB.C.Table<RecordStoreDataTypeList>().DeleteAsync(m => m.RecordStoreDataTypeId == _model.Id);
                }                    
            }

            if (count > 0)
            {
                // if list is created save list data
                if (listItems.Count > 0)
                {
                    var list = new List<RecordStoreDataTypeList>();
                    for (int i = 0; i < listItems.Count; i++)
                    {
                        list.Add(new RecordStoreDataTypeList
                        {
                            Index = i,
                            ListElementTitle = listItems[i],
                            RecordStoreDataTypeId = _model.Id,
                        });
                    }
                    await MyDB.C.InsertAllAsync(list);
                }

                DataTypeCreated?.Invoke(_model);
                await Navigation.PopModalAsync();
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(columnNameEntry.Text))
            {
                DisplayAlert("Error", "You must enter column name!", "OK");
                return false;
            }

            var result = true;
            if (ColumnNameExists != null)
                result = ColumnNameExists(_model.Id, columnNameEntry.Text);

            // if entry is new verify if name entered already exists or not
            if (result)
            {
                DisplayAlert("Error", "You must enter a unique column name!", "OK");
                return false;
            }

            if (dataTypePicker.SelectedIndex == -1)
            {
                DisplayAlert("Error", "You must select data type!", "OK");
                return false;
            }

            // verify list items added if list selected
            if (IsListDataTypeSelected() && !IsListItemAdded())
            {
                DisplayAlert("Error", "You must select an element for list data type!", "OK");
                return false;
            }

            return true;
        }

        private bool IsListItemAdded()
        {
            for (int i = _START_INDEX; i < myLayout.Children.Count - _START_INDEX; i++)
            {
                if (myLayout.Children[i] is StackLayout layout)
                {
                    if (layout.Children[0] is Entry ntry)
                    {
                        if (!string.IsNullOrWhiteSpace(ntry.Text))
                            return true;
                    }
                }
            }
            return false;
        }

        private List<string> GetListItemsList()
        {
            var list = new List<string>();
            for (int i = _START_INDEX; i < myLayout.Children.Count - _START_INDEX; i++)
            {
                if (myLayout.Children[i] is StackLayout layout)
                {
                    if (layout.Children[0] is Entry ntry)
                    {
                        if (!string.IsNullOrWhiteSpace(ntry.Text))
                            list.Add(ntry.Text);
                    }
                }
            }
            list.TrimExcess();
            return list;
        }

        private bool IsListDataTypeSelected()
        {
            return dataTypePicker.SelectedIndex == (int)DataTypes.List;
        }

        private void cancelButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
                
        private void dataTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataTypePicker.SelectedIndex == (int)DataTypes.List && !_flag)
            {
                //dataTypePicker.IsEnabled = false;
                AddRowForListItem();
            }
            else if (_index > _START_INDEX)
            {
                // remove rows if list was selected before and then changed back
                var layoutList = new List<Layout>();
                for (int i = _START_INDEX; i < myLayout.Children.Count; i++)
                {
                    if (myLayout.Children[i] is Layout layout)
                        layoutList.Add(layout);
                }

                foreach (var item in layoutList)
                {
                    myLayout.Children.Remove(item);
                }

                _index = _START_INDEX;
            }
        }
        
        private void AddRowForListItem(string listElement = "", bool hasNextElement = false)
        {
            var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0, 5, 0) };
            
            var ent = new Entry
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Keyboard = Keyboard.Text,
                Text = listElement,
            };

            layout.Children.Add(ent);

            var btn = new Button
            {
                Text = hasNextElement ? "Remove" : "Add",
                HorizontalOptions = LayoutOptions.End,
            };

            btn.Clicked += (object sender, EventArgs e) => 
            {
                var b = (sender as Button);
                if (b.Text == "Add")
                {
                    AddRowForListItem();
                    b.Text = "Remove";
                }
                else
                {
                    myLayout.Children.Remove(layout);
                    _index--;
                }
            };

            layout.Children.Add(btn);

            myLayout.Children.Insert(_index, layout);
            _index++;
        }
    }

    public enum DataTypes
    {
        Text = 0,        
        Numeric = 1,
        Date = 2,
        Time = 3,
        YesNo = 4,
        Email = 5,
        Telephone = 6,
        Url = 7,
        MultilineText = 8,
        List = 9,
        Password = 10, 
    }
}