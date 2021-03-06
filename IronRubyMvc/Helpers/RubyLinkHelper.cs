#region Usings

using System.Collections.Generic;
using System.Web.Mvc.Html;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Routing;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Helpers
{
    public partial class RubyHtmlHelper
    {
        public MvcHtmlString ActionLink(string linkText, Hash values)
        {
            return _helper.RouteLink(linkText, values.ToRouteDictionary());
        }

        public MvcHtmlString ActionLink(string linkText, string actionName, Hash values)
        {
            return _helper.ActionLink(linkText, actionName, values.ToRouteDictionary());
        }

        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName)
        {
            return _helper.ActionLink(linkText, actionName, controllerName);
        }

        public MvcHtmlString ActionLink(string linkText, string actionName)
        {
            return _helper.ActionLink(linkText, actionName);
        }

        public MvcHtmlString ActionLink(string linkText, string actionName, Hash routeValues, Hash htmlAttributes)
        {
            return _helper.ActionLink(linkText, actionName, routeValues.ToRouteDictionary(), htmlAttributes.ToDictionary());
        }

        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, Hash routeValues, Hash htmlAttributes)
        {
            return _helper.ActionLink(linkText, actionName, controllerName, routeValues.ToRouteDictionary(), htmlAttributes.ToDictionary());
        }


        public MvcHtmlString ActionLink(string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, Hash routeValues, Hash htmlAttributes)
        {
            return _helper.ActionLink(linkText, actionName, controllerName, protocol, hostName, fragment, routeValues.ToRouteDictionary(), htmlAttributes.ToDictionary());
        }

        public MvcHtmlString RouteLink(string linkText, Hash routeValues)
        {
            return _helper.RouteLink(linkText, routeValues.ToRouteDictionary());
        }

        public MvcHtmlString RouteLink(string linkText, string routeName, Hash routeValues)
        {
            return _helper.RouteLink(linkText, routeName, routeValues.ToRouteDictionary());
        }

        public MvcHtmlString RouteLink(string linkText, Hash routeValues, Hash htmlAttributes)
        {
            return _helper.RouteLink(linkText, routeValues.ToRouteDictionary(), htmlAttributes.ToDictionary());
        }

        public MvcHtmlString RouteLink(string linkText, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return _helper.RouteLink(linkText, null /* routeName */, routeValues, htmlAttributes);
        }

        public MvcHtmlString RouteLink(string linkText, string routeName, Hash routeValues, Hash htmlAttributes)
        {
            return _helper.RouteLink(linkText, routeName, routeValues.ToRouteDictionary(), htmlAttributes.ToDictionary());
        }


        public MvcHtmlString RouteLink(string linkText, string routeName, string protocol, string hostName, string fragment, Hash routeValues, Hash htmlAttributes)
        {
            return _helper.RouteLink(linkText, routeName, protocol, hostName, fragment, routeValues.ToRouteDictionary(), htmlAttributes.ToDictionary());
        }
    }
}