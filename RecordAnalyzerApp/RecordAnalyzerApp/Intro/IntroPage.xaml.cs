using RecordAnalyzerApp.ResourcesProvider;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp.Intro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntroPage : ContentPage
    {
        public IntroPage()
        {
            InitializeComponent();
            IntroCarouselView.ItemsSource = ImageResourceHelper.GetIntroImagesList();
        }

        private void IntroCarouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            SetBulletImage(e.PreviousItem, 0);
            SetBulletImage(e.CurrentItem, 1);
        }

        private void SetBulletImage(object item, int sourceId)
        {
            if (item is IntroBindElement intro)
            {
                if (bulletImagesStack.Children[intro.Key] is Image bullet)
                {
                    bullet.Source = ImageResourceHelper.GetImgSource(sourceId);
                }

                skipIntro.IsVisible = intro.SkipButtonVisible();
                startApp.IsVisible = !skipIntro.IsVisible;
            }
        }

        private async void Start_Clicked(object sender, System.EventArgs e)
        {
            (Application.Current as App).ShowIntro = false;

            await Navigation.PushAsync(new LoadingPage());

            Navigation.RemovePage(this);
        }
    }
}