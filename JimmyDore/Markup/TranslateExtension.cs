using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using JimmyDore.Services.Localise;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JimmyDore.Markup
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private const string ResourceId = "JimmyDore.AppResources";

        private static readonly Lazy<ResourceManager> _resourceManager = new Lazy<ResourceManager>(
           () => new ResourceManager(ResourceId, IntrospectionExtensions.GetTypeInfo(typeof(TranslateExtension)).Assembly));

        private readonly CultureInfo _cultureInfo = null;

        public TranslateExtension()
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                _cultureInfo = DependencyService.Resolve<ILocalise>()?.GetCurrentCultureInfo();

            }

            Uppercase = false;
        }

        public string Text { get; set; }

        public bool Uppercase { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
            {
                return string.Empty;
            }

            string translation = null;
            try
            {
                translation = _resourceManager.Value.GetString(Text, _cultureInfo);
            }
            catch (FileNotFoundException)
            {
                // ignore
            }

            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException($"Key '{Text}' was not found in resources '{ResourceId}' for culture '{_cultureInfo.Name}'.", nameof(Text));
#else
                translation = Text;
#endif
            }

            return Uppercase ? translation.ToUpper() : translation;
        }
    }
}
