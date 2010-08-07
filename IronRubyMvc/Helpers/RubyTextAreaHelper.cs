#region Usings

using System.Web.Mvc.Html;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Helpers
{
    public partial class RubyHtmlHelper
    {
        public MvcHtmlString TextArea(string name)
        {
            return _helper.TextArea(name);
        }

        public MvcHtmlString TextArea(string name, Hash htmlAttributes)
        {
            return _helper.TextArea(name, htmlAttributes.ToDictionary());
        }

        public MvcHtmlString TextArea(string name, string value)
        {
            return _helper.TextArea(name, value);
        }

        public MvcHtmlString TextArea(string name, string value, Hash htmlAttributes)
        {
            return _helper.TextArea(name, value, htmlAttributes.ToDictionary());
        }

        public MvcHtmlString TextArea(string name, string value, int rows, int columns, Hash htmlAttributes)
        {
            return _helper.TextArea(name, value, rows, columns, htmlAttributes.ToDictionary());
        }
    }
}