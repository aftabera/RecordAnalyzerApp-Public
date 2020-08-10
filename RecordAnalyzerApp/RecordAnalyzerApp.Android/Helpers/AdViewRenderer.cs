using Android.Gms.Ads;
using Android.Widget;
using RecordAnalyzerApp.Controls;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RecordAnalyzerApp.Controls.AdControlView), typeof(RecordAnalyzerApp.Droid.Helpers.AdViewRenderer))]
namespace RecordAnalyzerApp.Droid.Helpers
{
    public class AdViewRenderer: ViewRenderer<Controls.AdControlView, AdView>
    {
#if DEBUG
        string adUnitId = "ca-app-pub-3940256099942544/6300978111"; //google's dedicated test ad unit ID for Android banners
#else
        string adUnitId = "ca-app-pub-5469819848076810/8984664651";
#endif   
        AdSize adSize = AdSize.SmartBanner;
        AdView adView;

        public AdViewRenderer() : base(Android.App.Application.Context) { }

        AdView CreateAdView()
        {
            if (adView != null)
                return adView;

            adView = new AdView(Android.App.Application.Context);
            adView.AdSize = adSize;
            adView.AdUnitId = adUnitId;

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            adView.LayoutParameters = adParams;
            adView.LoadAd(new AdRequest.Builder().Build());

            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdControlView> e)
        {
            base.OnElementChanged(e);

            if (Common.Values.PRO_VERSION) return;

            if (Control == null)
            {
                CreateAdView();
                SetNativeControl(adView);
            }
        }
    }
}