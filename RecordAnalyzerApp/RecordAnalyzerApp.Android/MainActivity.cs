using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Ads;

namespace RecordAnalyzerApp.Droid
{
    [Activity(Label = "RecordS Pro", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            // Required for using carousal view
            Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);


            // Initialize Ads View
            //MobileAds.Initialize(ApplicationContext, "ca-app-pub-5469819848076810~7344279920");

            LoadApplication(new App(GetBackupFolderPath()));
        }
        string GetBackupFolderPath()
        {
            //return this.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
            return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}