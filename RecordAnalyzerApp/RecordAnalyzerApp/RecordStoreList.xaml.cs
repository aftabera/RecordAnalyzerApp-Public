using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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
    public partial class RecordStoreList : ContentPage
    {
        public RecordStoreList()
        {
            InitializeComponent();

            MyAdView.IsVisible = !Common.Values.PRO_VERSION;
        }

        protected async override void OnAppearing()
        {
            await LoadRecordStores();

            base.OnAppearing();
        }

        private async Task LoadRecordStores()
        {
            List<RecordStore> stores = null;
            try
            {
                stores = await MyDB.C.Table<RecordStore>().ToListAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK"); 
            }
            storesListView.ItemsSource = stores;
        }

        private async void btnNew_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new RecordStoreNew(), true);
        }

        private async void storesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var store = e.Item as RecordStore;
            await Navigation.PushAsync(new RecordStoreNew(store), true);
        }

        private async void btnDBSize_Clicked(object sender, System.EventArgs e)
        {
            long size = MyDB.GetDBFileSize();
            int sizeInMB = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(size / 1000000)));

            string msg;
            if (sizeInMB < 1)
                msg = "DB File Size is less than 1 MB";
            else
                msg = string.Format("DB File Size is: {0} MB", sizeInMB);

            await DisplayAlert("Storage Info", msg, "OK");
        }

        private async void btnDBBackup_Clicked(object sender, EventArgs e)
        {
            await CreateBackup();
        }

        private async Task CreateBackup()
        {
            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (status == PermissionStatus.Granted)
            {
                if (await MyDB.CreateBackup())
                {
                    await DisplayAlert("Created", MyDB.GetBackupFilePath(), "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Backup Failed!", "OK");
                }
            }
            else
            {
                if (Common.Values.PERMISSION_TRIED)
                {
                    await DisplayAlert("Error", "For backup please allow application to access storage!", "OK");
                }
                else
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    Common.Values.PERMISSION_TRIED = true;
                    await CreateBackup();
                }
            }
        }

        private async void btnDBRestore_Clicked(object sender, EventArgs e)
        {
            await RestoreBackup();
        }

        private async Task RestoreBackup()
        {
            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (status == PermissionStatus.Granted)
            {
                bool result = await DisplayAlert("Confirmation", "Are you sure you want to restore last backup. This cannot be undone?", "Yes", "No");
                if (result)
                {
                    if (await MyDB.RestoreBackup())
                    {
                        await LoadRecordStores();
                        await DisplayAlert("Success", "Backup Restored!", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Restore Failed!", "OK");
                    }
                }
            }
            else
            {
                if (Common.Values.PERMISSION_TRIED)
                {
                    await DisplayAlert("Error", "For backup restore please allow application to access storage!", "OK");
                }
                else
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    Common.Values.PERMISSION_TRIED = true;
                    await RestoreBackup();
                }
            }
        }

        private async void btnAbout_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new About.AboutPage());
        }
    }
}