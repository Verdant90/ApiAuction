using System;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.OptionsModel;
using System.Configuration;

namespace Localization.JsonLocalizer.StringLocalizer
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory

    {
        public static string appName = "ApiAuctionShop";
        private static readonly string[] KnownViewExtensions = new[] { ".cshtml" };

        private readonly ConcurrentDictionary<string, JsonStringLocalizer> _localizerCache =
            new ConcurrentDictionary<string, JsonStringLocalizer>();

        private readonly IHostingEnvironment _applicationEnvironment;
        private string _resourcesRelativePath;

        public JsonStringLocalizerFactory(IHostingEnvironment applicationEnvironment,
                                          IOptions<JsonLocalizationOptions> localizationOptions)
        {
            if (applicationEnvironment == null)
            {
                throw new ArgumentNullException(nameof(applicationEnvironment));
            }
            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }

            this._applicationEnvironment = applicationEnvironment;

            _resourcesRelativePath = localizationOptions.Value.ResourcesPath ?? string.Empty;
            if (!string.IsNullOrEmpty(_resourcesRelativePath))
            {
                _resourcesRelativePath = _resourcesRelativePath
                    .Replace(Path.AltDirectorySeparatorChar, '.')
                    .Replace(Path.DirectorySeparatorChar, '.') + ".";
            }
            
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }
            

            var typeInfo = resourceSource.GetTypeInfo();
            var assembly = typeInfo.Assembly;

            // Re-root the base name if a resources path is set.
            var resourceBaseName = string.IsNullOrEmpty(_resourcesRelativePath)
                ? typeInfo.FullName
                : appName + "." + _resourcesRelativePath +
                    LocalizerUtil.TrimPrefix(typeInfo.FullName, appName + ".");

            return _localizerCache.GetOrAdd(
                resourceBaseName, new JsonStringLocalizer(resourceBaseName, appName));
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }
            
            location = location ?? appName;

            // Re-root base name if a resources path is set and strip the cshtml part.
            var resourceBaseName = location + "." + _resourcesRelativePath + LocalizerUtil.TrimPrefix(baseName, location + ".");

            var viewExtension = KnownViewExtensions.FirstOrDefault(extension => resourceBaseName.EndsWith(extension));
            if (viewExtension != null)
            {
                resourceBaseName = resourceBaseName.Substring(0, resourceBaseName.Length - viewExtension.Length);
            }
            

            return _localizerCache.GetOrAdd(
                resourceBaseName, new JsonStringLocalizer(resourceBaseName, appName));
        }
    }
}