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
    public partial class RecordStoreDataForm : ContentPage
    {
        RecordStoreDataMaster _data;
        List<RecordStoreDataType> _fields;
        Dictionary<int, Cell> _myDictionary;
        int _firstColumnId = 0;

        public RecordStoreDataForm()
        {
            InitializeComponent();
            _myDictionary = new Dictionary<int, Cell>();
        }

        public RecordStoreDataForm(RecordStoreDataMaster data)
            : this()
        {
            _data = data;
            MasterDate.Date = data.MasterDateTime.Date;
            MasterTime.Time = data.MasterDateTime.TimeOfDay;
        }

        protected async override void OnAppearing()
        {
            await PopulateFields();
            base.OnAppearing();
        }

        private async Task PopulateFields()
        {
            _fields = await MyDB.C.Table<RecordStoreDataType>()
                            .Where(m => m.RecordStoreId == _data.RecordStoreId)
                            .OrderBy(m => m.Id)
                            .ToListAsync();

            //code to show existing record here
            var details = await MyDB.C.Table<RecordStoreDataDetail>()
                .Where(m => m.RecordStoreDataMasterId == _data.Id)
                .ToListAsync();

            int i = 0;
            foreach (var item in _fields)
            {
                string value = string.Empty;

                if (details.Count > 0)
                {
                    var detail = details.Find(m => m.RecordStoreDataTypeId == item.Id);
                    if (detail != null) value = detail.Value;
                }

                Cell myCell = null;

                var dataType = (DataTypes)item.DataTypeId;
                switch (dataType)
                {
                    case DataTypes.Numeric:
                        myCell = CellGenerator.GetCellForTextDataType(item, value, Keyboard.Numeric);
                        break;
                    case DataTypes.Email:
                        myCell = CellGenerator.GetCellForTextDataType(item, value, Keyboard.Email);
                        break;
                    case DataTypes.Telephone:
                        myCell = CellGenerator.GetCellForTextDataType(item, value, Keyboard.Telephone);
                        break;
                    case DataTypes.Url:
                        myCell = CellGenerator.GetCellForTextDataType(item, value, Keyboard.Url);
                        break;
                    case DataTypes.Date:
                        myCell = CellGenerator.GetCellForDateDataType(item, value);
                        break;
                    case DataTypes.Time:
                        myCell = CellGenerator.GetCellForTimeDataType(item, value);
                        break;
                    case DataTypes.MultilineText:
                        myCell = CellGenerator.GetCellForMultilineText(item, value);
                        break;
                    case DataTypes.YesNo:
                        myCell = CellGenerator.GetCellForYesNoDataType(item, value);
                        break;
                    case DataTypes.List:
                        myCell = await CellGenerator.GetCellForListDataType(item, value, myCell);
                        break;
                    case DataTypes.Password:
                        myCell = CellGenerator.GetCellForPasswordDataType(item, value);
                        break;
                    case DataTypes.Text:
                    default:
                        myCell = CellGenerator.GetCellForTextDataType(item, value, Keyboard.Text);
                        break;
                }

                if (myCell != null)
                {
                    myTableSection.Insert(i++, myCell);
                    _myDictionary.Add(item.Id, myCell);
                    if (_firstColumnId == 0) _firstColumnId = item.Id;
                }
            }
        }
                                 
        private async void btnOk_Clicked(object sender, EventArgs e)
        {
            if (_firstColumnId == 0)
            {
                await DisplayAlert("Error", "You must add a data type first!", "OK");
                return;
            }

            int count;
            _data.MasterDateTime = new DateTime(
                    MasterDate.Date.Year,
                    MasterDate.Date.Month,
                    MasterDate.Date.Day,
                    MasterTime.Time.Hours,
                    MasterTime.Time.Minutes,
                    MasterTime.Time.Seconds
                    );
            _data.PrimaryValue = ToString(_myDictionary[_firstColumnId], true);

            if (_data.Id == 0)
            {
                count = await MyDB.C.InsertAsync(_data);
            }
            else
            {
                count = await MyDB.C.UpdateAsync(_data);

                if (count > 0)
                {
                    count = await MyDB.C.Table<RecordStoreDataDetail>()
                        .Where(m => m.RecordStoreDataMasterId == _data.Id)
                        .DeleteAsync();
                }
            }

            if (count == 0) return;

            if (count > 0)
            {
                var details = new List<RecordStoreDataDetail>(); 
                foreach (var item in _myDictionary)
                {
                    var row = new RecordStoreDataDetail();

                    row.Value = ToString(item.Value);
                    row.RecordStoreDataTypeId = item.Key;
                    row.RecordStoreDataMasterId = _data.Id;
                    row.RecordStoreId = _data.RecordStoreId;

                    details.Add(row);
                }

                count = await MyDB.C.InsertAllAsync(details);
                if (count > 0)
                {
                    await Navigation.PopModalAsync();
                }
            }
        }

        private string ToString(Cell c, bool requiredForPrimary = false)
        {
            if (c is EntryCell ntryCell)
            {
                return ntryCell.Text;
            }
            else if (c is ViewCell vc)
            {
                View myView = (vc.View as StackLayout).Children[1];
                if (myView is DatePicker dp)
                {
                    return dp.Date.ToString();
                }
                else if (myView is TimePicker tp)
                {
                    return tp.Time.ToString();
                }
                else if (myView is Editor ditor)
                {
                    return ditor.Text;
                }
                else if (myView is Picker cker)
                {
                    if (requiredForPrimary)
                    {
                        return cker.SelectedItem.ToString();
                    }
                    return cker.SelectedIndex.ToString();
                }
                else if (myView is Entry ntry)
                {
                    return ntry.Text;
                }
            }
            else if (c is SwitchCell sc)
            {
                return sc.On ? RecordStoreDataType.YES : RecordStoreDataType.NO;
            }
            return string.Empty;
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();            
        }
    }

    static class CellGenerator
    {
        public static Cell GetCellForTextDataType(RecordStoreDataType item, string value, Keyboard keyboard)
        {
            return new EntryCell
            {
                Label = item.Title,
                Keyboard = keyboard,
                Text = value,
            };
        }

        public static Cell GetCellForDateDataType(RecordStoreDataType item, string value)
        {
            Cell myCell;
            var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0, 5, 0) };
            layout.Children.Add(
                new Label { Text = item.Title, VerticalOptions = LayoutOptions.Center }
            );
            layout.Children.Add(
                new DatePicker
                {
                    Date = string.IsNullOrWhiteSpace(value) ? DateTime.Now.Date : Convert.ToDateTime(value).Date,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                }
            );
            myCell = new ViewCell { View = layout };
            return myCell;
        }

        public static Cell GetCellForTimeDataType(RecordStoreDataType item, string value)
        {
            Cell myCell;
            var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0, 5, 0) };
            layout.Children.Add(
                new Label { Text = item.Title, VerticalOptions = LayoutOptions.Center }
            );
            layout.Children.Add(
                new TimePicker
                {
                    Time = string.IsNullOrWhiteSpace(value) ? DateTime.Now.TimeOfDay : Convert.ToDateTime(value).TimeOfDay,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                }
            );
            myCell = new ViewCell { View = layout };
            return myCell;
        }

        public static Cell GetCellForMultilineText(RecordStoreDataType item, string value)
        {
            Cell myCell;
            var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0, 5, 0) };
            layout.Children.Add(
                new Label { Text = item.Title, VerticalOptions = LayoutOptions.Center }
            );
            layout.Children.Add(
                new Editor
                {
                    Text = value,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = 150,                    
                }
            );
            myCell = new ViewCell { View = layout };
            return myCell;
        }

        public static Cell GetCellForYesNoDataType(RecordStoreDataType item, string value)
        {
            return new SwitchCell
            {
                Text = item.Title,
                On = string.IsNullOrWhiteSpace(value) ? false : (value == "Yes" ? true : false),
            };
        }

        public static async Task<Cell> GetCellForListDataType(RecordStoreDataType item, string value, Cell myCell)
        {
            var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0, 5, 0) };
            layout.Children.Add(
                new Label { Text = item.Title, VerticalOptions = LayoutOptions.Center }
            );

            var myPicker = new Picker { HorizontalOptions = LayoutOptions.FillAndExpand };

            // add code fill picker here
            var elementList = await RecordStoreDataTypeList.GetByRecordStoreDataTypeId(item.Id);

            foreach (var element in elementList)
            {
                myPicker.Items.Add(element.ListElementTitle);
            }

            int.TryParse(value, out int temp);
            myPicker.SelectedIndex = temp;

            layout.Children.Add(myPicker);
            myCell = new ViewCell { View = layout };
            return myCell;
        }

        internal static Cell GetCellForPasswordDataType(RecordStoreDataType item, string value)
        {
            Cell myCell;
            var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0, 5, 0) };

            //*
            layout.Children.Add(
                new Label { Text = item.Title, VerticalOptions = LayoutOptions.Center, IsVisible = false }
            );
            //*/

            var ntry = new Entry
            {
                Text = value,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsPassword = true,
            };

            layout.Children.Add(ntry);

            var btn = new Button 
            {
                Text = "View",
                HorizontalOptions = LayoutOptions.End,
            };

            btn.Clicked += (object sender, EventArgs e) => 
            {
                ntry.IsPassword = !ntry.IsPassword;
                btn.Text = ntry.IsPassword ? "View" : "Hide";
            };

            layout.Children.Add(btn);

            myCell = new ViewCell { View = layout };
            return myCell;
        }
    }
}