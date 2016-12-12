using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Localization.JsonLocalizer.StringLocalizer
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly ConcurrentDictionary<string, Lazy<JObject>> _resourceObjectCache =
            new ConcurrentDictionary<string, Lazy<JObject>>();

        private readonly string _baseName;
        private readonly string _applicationName;
        private readonly IEnumerable<string> _resourceFileLocations;

        public JsonStringLocalizer(string baseName, string applicationName)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }
            if (applicationName == null)
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            this._baseName = baseName;
            this._applicationName = applicationName;

            // Get a list of possible resource file locations.
            _resourceFileLocations = LocalizerUtil.ExpandPaths(baseName, applicationName).ToList();
            
        }

        public virtual LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var value = GetLocalizedString(name, CultureInfo.CurrentUICulture);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public virtual LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var format = GetLocalizedString(name, CultureInfo.CurrentUICulture);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures) =>
            GetAllStrings(includeAncestorCultures, CultureInfo.CurrentUICulture);

        protected IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures, CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            if (culture == null)
            {
                return new JsonStringLocalizer(_baseName, _applicationName);
            }
            throw new NotImplementedException();
        }

        protected string GetLocalizedString(string name, CultureInfo culture)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            // Attempt to get resource with the given name from the resource object. if not found, try parent
            // resource object until parent begets himself.
            var currentCulture = CultureInfo.CurrentCulture;
            CultureInfo previousCulture = null;
            do
            {
                var resourceObject = GetResourceObject(currentCulture);
                if (resourceObject != null)
                {
                    JToken value;
                    if (resourceObject.TryGetValue(name, out value))
                    {
                        var localizedString = value.ToString();
                        return localizedString;
                    }
                }

                // Consult parent culture.
                previousCulture = currentCulture;
                currentCulture = currentCulture.Parent;
            } while (previousCulture != currentCulture);

            return null;
        }

        private JObject GetResourceObject(CultureInfo currentCulture)
        {
            if (currentCulture == null)
            {
                throw new ArgumentNullException(nameof(currentCulture));
            }
            
            var cultureSuffix = "." + currentCulture.Name;
            cultureSuffix = cultureSuffix == "." ? "" : cultureSuffix;

            var lazyJObjectGetter = new Lazy<JObject>(() =>
            {
                // First attempt to find a resource file location that exists.
                string resourcePath = null;
                foreach (var resourceFileLocation in _resourceFileLocations)
                {
                    resourcePath = resourceFileLocation + cultureSuffix + ".json";
                    if (File.Exists(resourcePath))
                    {
                        break;
                    }
                    else
                    {
                        resourcePath = null;
                    }
                }
                if (resourcePath == null)
                {
                    return null;
                }

                // Found a resource file path: attempt to parse it into a JObject.
                try
                {
                    var resourceFileStream =
                        new FileStream(resourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                    using (resourceFileStream)
                    {
                        var resourceReader =
                            new JsonTextReader(new StreamReader(resourceFileStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true));
                        using (resourceReader)
                        {
                            return JObject.Load(resourceReader);
                        }
                    }
                }
                catch (Exception e)
                {
                    return null;
                }

            }, LazyThreadSafetyMode.ExecutionAndPublication);

            lazyJObjectGetter = _resourceObjectCache.GetOrAdd(cultureSuffix, lazyJObjectGetter);
            var resourceObject = lazyJObjectGetter.Value;
            return resourceObject;
        }

        private string[] GetCultureSuffixes(CultureInfo currentCulture)
        {
            // Get culture suffixes (e.g.: { "nl-NL.", "nl.", "" }).
            string[] cultureSuffixes;
            if (currentCulture == null)
            {
                cultureSuffixes = new[] { "" };
            }
            else
            {
                if (currentCulture.IsNeutralCulture)
                {
                    cultureSuffixes = new[] { currentCulture.Name + ".", "" };
                }
                else
                {
                    cultureSuffixes = new[] { currentCulture.Name + ".", currentCulture.Parent.Name + ".", "" };
                }
            }

            var cultureSuffixesLogString = string.Join(", ", cultureSuffixes);
            return cultureSuffixes;
        }
    }
}