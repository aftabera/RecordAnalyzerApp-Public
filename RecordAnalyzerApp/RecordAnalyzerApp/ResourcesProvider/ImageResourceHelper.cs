using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace RecordAnalyzerApp.ResourcesProvider
{
    public static class ImageResourceHelper
    {
        public static List<IntroBindElement> GetIntroImagesList()
        {
            return new List<IntroBindElement>
            {
                new IntroBindElement(0, GetImgSource(100)),
                new IntroBindElement(1, GetImgSource(200)),
                new IntroBindElement(2, GetImgSource(300)),
                new IntroBindElement(3, GetImgSource(400)),
                new IntroBindElement(4, GetImgSource(500)),
            };
        }

        public static ImageSource GetFileImgSource(int id)
        {
            return ImageSource.FromFile(Get(id));
        }

        public static string GetImgSource(int id)
        {
            return Get(id);
        }

        static string Get(int id)
        {
            switch (id)
            {
                case 0:
                    return "CircleHollow.webp";
                case 1:
                    return "CircleFill.webp";
                case 100:
                    return "Page1.webp";
                case 200:
                    return "Page2.webp";
                case 300:
                    return "Page3.webp";
                case 400:
                    return "Page4.webp";
                case 500:
                    return "Page5.webp";
                case 1000:
                    return "FirstButton.webp";
                case 2000:
                    return "SecondButton.webp";
                case 3000:
                    return "ThirdButton.webp";
                case 4000:
                    return "FourthButton.webp";
                default:
                    return string.Empty;
            }
        }
    }

    public class IntroBindElement
    {
        public IntroBindElement(int key, ImageSource img)
        {
            Key = key;
            ImgSource = img;
        }

        public int Key { get; }

        public ImageSource ImgSource { get; }

        internal bool SkipButtonVisible()
        {
            return Key != 4;
        }
    }
}
