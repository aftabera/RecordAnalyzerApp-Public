using RecordAnalyzerApp.ResourcesProvider;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecordAnalyzerApp.MarkupExtensions
{
    [ContentProperty("ResourceId")]
    public class EmbeddedImage : IMarkupExtension
    {
        public int ResourceId { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return ImageResourceHelper.GetImgSource(ResourceId);
        }
    }
}

