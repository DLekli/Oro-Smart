using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Oro_Smart.Models
{
    public class LanguageService
    {
        private readonly IStringLocalizer _localizer;

        public LanguageService(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResources);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResources", assemblyName.Name);
        }

        //public LocalizedString Getkey(string key)
        //{


        //    return _localizer[key];

        //}
        public string GetKey(string key)
        {
            var localizedString = _localizer[key];

            // Log or debug the key and localized string
            Console.WriteLine($"Key: {key}, Localized String: {localizedString?.Value}");

            return localizedString;
        }

    }
}
