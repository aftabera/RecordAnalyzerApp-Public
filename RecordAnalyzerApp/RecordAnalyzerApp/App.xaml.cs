using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp
{
    public partial class App : Application
    {
        public App(string directory)
        {
            Persistance.MyDB.BACKUP_DIRECTORY = directory;

            InitializeComponent();

            if (ShowIntro)
            {
                MainPage = new NavigationPage(new Intro.IntroPage());
            }
            else
            {
                MainPage = new NavigationPage(new LoadingPage());
            }
        }

        public bool ShowIntro 
        { 
            get
            {
                if (Properties.ContainsKey(nameof(ShowIntro)))
                    return (bool)Properties[nameof(ShowIntro)];

                return true;
            }
            set
            {
                Properties[nameof(ShowIntro)] = value;
                SavePropertiesAsync();
            }
        }

        
    }
}
