#region Usings

using System.Web.Mvc.Html;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby.Builtins;

#endregion

namespace System.Web.Mvc.IronRuby.Helpers
{
    public partial class RubyHtmlHelper
    {
        public MvcHtmlString ValidationMessage(string modelName)
        {
            return _helper.ValidationMessage(modelName);
        }

        public MvcHtmlString ValidationMessage(string modelName, Hash htmlAttributes)
        {
            return _helper.ValidationMessage(modelName, htmlAttributes.ToDictionary());
        }

        public MvcHtmlString ValidationMessage(string modelName, string validationMessage)
        {
            return _helper.ValidationMessage(modelName, validationMessage);
        }

        public MvcHtmlString ValidationMessage(string modelName, string validationMessage, Hash htmlAttributes)
        {
            return _helper.ValidationMessage(modelName, validationMessage, htmlAttributes.ToDictionary());
        }

        public MvcHtmlString ValidationSummary()
        {
            return _helper.ValidationSummary();
        }

        public MvcHtmlString ValidationSummary(string message)
        {
            return _helper.ValidationSummary(message);
        }

        public MvcHtmlString ValidationSummary(string message, Hash htmlAttributes)
        {
            return _helper.ValidationSummary(message, htmlAttributes.ToDictionary());
        }
    }
}