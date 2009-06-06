extern alias clr3;
#region Usings

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;

#endregion

namespace System.Web.Mvc.IronRuby.ViewEngine
{
    public class RubyViewEngine : VirtualPathProviderViewEngine
    {
        private const string _cacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:";
        private const string _cacheKeyPrefix_Master = "Master";
        private const string _cacheKeyPrefix_Partial = "Partial";
        private const string _cacheKeyPrefix_View = "View";
        private static readonly string[] _emptyLocations = new string[0];

        private readonly IRubyEngine _rubyEngine;

        public RubyViewEngine(IRubyEngine rubyEngine)
        {
            _rubyEngine = rubyEngine;
            PartialViewLocationFormats = new[]
                                             {
                                                 "~/Views/{1}/_{0}.html.erb",
                                                 "~/Views/Shared/_{0}.html.erb"
                                             };

            ViewLocationFormats = new[]
                                      {
                                          "~/Views/{1}/{0}.html.erb",
                                          "~/Views/Shared/{0}.html.erb",
                                      };

            MasterLocationFormats = new[]
                                        {
                                            "~/Views/{1}/{0}.html.erb",
                                            "~/Views/Shared/{0}.html.erb"
                                        };
        }

        private string GetContents(string path)
        {
            using (var stream = VirtualPathProvider.GetFile(path).Open())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private RubyView GetView(string virtualPath, RubyView masterView)
        {
            if (String.IsNullOrEmpty(virtualPath))
                return null;

            return new RubyView(_rubyEngine, GetContents(virtualPath), masterView);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return GetView(partialPath, null);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return GetView(viewPath, GetView(masterPath, null));
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (String.IsNullOrEmpty(partialViewName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "partialViewName");
            }

            string[] searched;
            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            string partialPath = GetPath(controllerContext, PartialViewLocationFormats, "PartialViewLocationFormats", partialViewName, controllerName, _cacheKeyPrefix_Partial, useCache, out searched);

            if (String.IsNullOrEmpty(partialPath))
            {
                return new ViewEngineResult(searched);
            }

            return new ViewEngineResult(CreatePartialView(controllerContext, partialPath), this);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (String.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "viewName");
            }

            string[] viewLocationsSearched;
            string[] masterLocationsSearched;

            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            string viewPath = GetPath(controllerContext, ViewLocationFormats, "ViewLocationFormats", viewName, controllerName, _cacheKeyPrefix_View, useCache, out viewLocationsSearched);
            string masterPath = GetPath(controllerContext, MasterLocationFormats, "MasterLocationFormats", masterName, controllerName, _cacheKeyPrefix_Master, useCache, out masterLocationsSearched);

            if (String.IsNullOrEmpty(viewPath) || (String.IsNullOrEmpty(masterPath) && !String.IsNullOrEmpty(masterName)))
            {
                return new ViewEngineResult(clr3::System.Linq.Enumerable.Union(viewLocationsSearched, masterLocationsSearched));
            }

            return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
        }

        private string GetPath(ControllerContext controllerContext, string[] locations, string locationsPropertyName, string name, string controllerName, string cacheKeyPrefix, bool useCache, out string[] searchedLocations)
        {
            searchedLocations = _emptyLocations;

            if (String.IsNullOrEmpty(name))
            {
                return String.Empty;
            }

            if (locations == null || locations.Length == 0)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture,
                    "Property: {0} cannot be null or empty.", locationsPropertyName));
            }

            bool nameRepresentsPath = IsSpecificPath(name);
            string cacheKey = CreateCacheKey(cacheKeyPrefix, name, (nameRepresentsPath) ? String.Empty : controllerName);

            if (useCache)
            {
                string result = ViewLocationCache.GetViewLocation(controllerContext.HttpContext, cacheKey);
                if (result != null)
                {
                    return result;
                }
            }

            return (nameRepresentsPath) ?
                GetPathFromSpecificName(controllerContext, name, cacheKey, ref searchedLocations) :
                GetPathFromGeneralName(controllerContext, locations, name, controllerName, cacheKey, ref searchedLocations);
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, string[] locations, string name, string controllerName, string cacheKey, ref string[] searchedLocations)
        {
            string result = String.Empty;
            var searchedLocationsList = new List<string>();

            for (int i = 0; i < locations.Length; i++)
            {
                var virtualPath = GetViewLocation(controllerContext, locations, i, name.Pascalize(), controllerName,
                                                  searchedLocationsList, cacheKey);
                searchedLocationsList.Add(virtualPath);
                if (string.IsNullOrEmpty(virtualPath))
                    virtualPath = GetViewLocation(controllerContext, locations, i, name.Underscore(), controllerName,
                                                  searchedLocationsList, cacheKey);
                if(!string.IsNullOrEmpty(virtualPath))
                {
                    searchedLocationsList = new List<string>();
                    result = virtualPath;
                    break;
                }
                searchedLocationsList.Add(virtualPath);
            }
            searchedLocations = searchedLocationsList.ToArray();
            return result;
        }

        private string GetViewLocation(ControllerContext controllerContext, string[] locations, int i, string name, string controllerName, IList<string> searchedLocations, string cacheKey)
        {
            string virtualPath = String.Format(CultureInfo.InvariantCulture, locations[i], name, controllerName);

            if (FileExists(controllerContext, virtualPath))
            {
                ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
                return virtualPath;
            }
            return string.Empty;
        }

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
        {
            string result = name;

            if (!FileExists(controllerContext, name))
            {
                result = String.Empty;
                searchedLocations = new[] { name };
            }

            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
            return result;
        }

        private static bool IsSpecificPath(string name)
        {
            char c = name[0];
            return (c == '~' || c == '/');
        }
        
        private string CreateCacheKey(string prefix, string name, string controllerName)
        {
            return String.Format(CultureInfo.InvariantCulture, _cacheKeyFormat,
                GetType().AssemblyQualifiedName, prefix, name, controllerName);
        }
    }
}