#region Usings

using System.Collections.Generic;
using System.Web.Mvc.Html;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;
using Microsoft.Scripting;

#endregion

namespace System.Web.Mvc.IronRuby.Helpers
{
    public partial class RubyHtmlHelper
    {

        private readonly IDictionary<SymbolId, FormMethod> _formMethodMapping =
            new Dictionary<SymbolId, FormMethod>
                {
                    {SymbolTable.StringToId("get"), FormMethod.Get},
                    {SymbolTable.StringToId("post"), FormMethod.Post}
                };

        public MvcForm BeginForm()
        {
            return _helper.BeginForm();
        }

        public MvcForm BeginForm(Hash routeValues)
        {
            return _helper.BeginForm(routeValues.ToRouteDictionary());
        }


        public MvcForm BeginForm(string actionName, string controllerName)
        {
            return _helper.BeginForm(actionName, controllerName);
        }

        public MvcForm BeginForm(string actionName, string controllerName, Hash routeValues)
        {
            return _helper.BeginForm(actionName, controllerName, routeValues.ToRouteDictionary());
        }

        public MvcForm BeginForm(string actionName, string controllerName, SymbolId method)
        {
            return _helper.BeginForm(actionName, controllerName, _formMethodMapping[method]);
        }

        public MvcForm BeginForm(string actionName, string controllerName, FormMethod method)
        {
            return _helper.BeginForm(actionName, controllerName, method);
        }

        public MvcForm BeginForm(string actionName, string controllerName, Hash routeValues, SymbolId method)
        {
            return _helper.BeginForm(actionName, controllerName, routeValues.ToRouteDictionary(), _formMethodMapping[method]);
        }

        public MvcForm BeginForm(string actionName, string controllerName, Hash routeValues, FormMethod method)
        {
            return _helper.BeginForm(actionName, controllerName, routeValues.ToRouteDictionary(), method);
        }

        public MvcForm BeginForm(string actionName, string controllerName, FormMethod method, Hash htmlAttributes)
        {
            return _helper.BeginForm(actionName, controllerName, method, htmlAttributes.ToDictionary());
        }

        public MvcForm BeginForm(string actionName, string controllerName, Hash routeValues, SymbolId method, Hash htmlAttributes)
        {
            return _helper.BeginForm(actionName, controllerName, routeValues.ToRouteDictionary(), _formMethodMapping[method], htmlAttributes.ToDictionary());
        }

        public MvcForm BeginForm(string actionName, string controllerName, Hash routeValues, FormMethod method, Hash htmlAttributes)
        {
            return _helper.BeginForm(actionName, controllerName, routeValues.ToRouteDictionary(), method, htmlAttributes.ToDictionary());
        }

        public MvcForm BeginRouteForm(Hash routeValues)
        {
            return _helper.BeginRouteForm(routeValues.ToRouteDictionary());
        }

        public MvcForm BeginRouteForm(string routeName)
        {
            return _helper.BeginRouteForm(routeName);
        }

        public MvcForm BeginRouteForm(string routeName, Hash routeValues)
        {
            return _helper.BeginRouteForm(routeName, routeValues.ToRouteDictionary());
        }

        public MvcForm BeginRouteForm(string routeName, SymbolId method)
        {
            return _helper.BeginRouteForm(routeName, _formMethodMapping[method]);
        }

        public MvcForm BeginRouteForm(string routeName, FormMethod method)
        {
            return _helper.BeginRouteForm(routeName, method);
        }

        public MvcForm BeginRouteForm(string routeName, Hash routeValues, SymbolId method)
        {
            return _helper.BeginRouteForm(routeName, routeValues.ToRouteDictionary(), _formMethodMapping[method]);
        }

        public MvcForm BeginRouteForm(string routeName, Hash routeValues, FormMethod method)
        {
            return _helper.BeginRouteForm(routeName, routeValues.ToRouteDictionary(), method);
        }

        public MvcForm BeginRouteForm(string routeName, SymbolId method, Hash htmlAttributes)
        {
            return _helper.BeginRouteForm(routeName, _formMethodMapping[method], htmlAttributes.ToDictionary());
        }

        public MvcForm BeginRouteForm(string routeName, FormMethod method, Hash htmlAttributes)
        {
            return _helper.BeginRouteForm(routeName, method, htmlAttributes.ToDictionary());
        }

        public MvcForm BeginRouteForm(string routeName, Hash routeValues, SymbolId method, Hash htmlAttributes)
        {
            return _helper.BeginRouteForm(routeName, routeValues.ToRouteDictionary(), _formMethodMapping[method], htmlAttributes.ToDictionary());
        }

        public MvcForm BeginRouteForm(string routeName, Hash routeValues, FormMethod method, Hash htmlAttributes)
        {
            return _helper.BeginRouteForm(routeName, routeValues.ToRouteDictionary(), method, htmlAttributes.ToDictionary());
        }

        public void EndForm()
        {
            _helper.EndForm();
        }
    }
}