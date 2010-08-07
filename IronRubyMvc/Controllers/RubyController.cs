#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Mvc.IronRuby.Helpers;
using System.Web.Routing;
using IronRuby.Builtins;
using Microsoft.Scripting;

#endregion

namespace System.Web.Mvc.IronRuby.Controllers
{
    public class RubyController : Controller
    {
        private readonly Dictionary<object, object> _viewData = new Dictionary<object, object>();
        private IRubyEngine _engine;
        private IDictionary<SymbolId, object> _params;

        public string ControllerName { get; internal set; }
        public RubyClass RubyType { get; private set; }

        public string ControllerClassName
        {
            get { return Constants.ControllerclassFormat.FormattedWith(ControllerName); }
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Params")]
        public IDictionary<SymbolId, object> Params
        {
            get
            {
                if (_params == null)
                {
                    PopulateParams();
                }

                return _params;
            }
        }

        private void PopulateParams()
        {
            var modelType = typeof (IDictionary<SymbolId, object>);
            var request = ControllerContext.HttpContext.Request;
            var binder = Binders.GetBinder(modelType);
            var modelBindingContext = new ModelBindingContext
                                          {
                                              Model = new Dictionary<SymbolId, object>(ControllerContext.RouteData.Values.Count + request.QueryString.Count + request.Form.Count),
                                              ModelName = "params",
                                              ModelState = ModelState,
                                              ModelType = modelType,
                                              ValueProvider = ValueProvider
                                          };
            _params = binder.BindModel(ControllerContext, modelBindingContext) as IDictionary<SymbolId, object>;
            
        }

        public void InternalInitialize(ControllerConfiguration config)
        {
            Initialize(config.Context);
            _engine = config.Engine;
            ControllerName = config.RubyClass.Name.Replace("Controller", string.Empty);
            RubyType = config.RubyClass;
            Binders = RubyModelBinders.Binders;
        }

        protected override void Execute(RequestContext requestContext)
        {
            PopulateParams();
            ActionInvoker = new RubyControllerActionInvoker(ControllerClassName, _engine);
            base.Execute(requestContext);
        }

        [NonAction]
        public ActionResult RedirectToRoute(Hash values)
        {
            return RedirectToRoute(values.ToRouteDictionary());
        }

        [NonAction]
        public ActionResult RedirectToAction(string actionName, Hash values)
        {
            return RedirectToAction(actionName, values.ToRouteDictionary());
        }

        [NonAction]
        public new ViewResult View()
        {
            return View(null /* viewName */, null /* masterName */, null /* model */);
        }

        [NonAction]
        public new ViewResult View(object model)
        {
            return View(null /* viewName */, null /* masterName */, model);
        }

        [NonAction]
        public ViewResult View(SymbolId viewName)
        {
            return View(viewName.ToString());
        }

        [NonAction]
        public new ViewResult View(string viewName)
        {
            return View(viewName, null /* masterName */, null /* model */);
        }

//        [NonAction]
//        public ViewResult View(SymbolId viewName)
//        {
//            return View(viewName, null /* masterName */, null /* model */);
//        }

        [NonAction]
        public new ViewResult View(string viewName, string masterName)
        {
            return View(viewName, masterName, null /* model */);
        }

//        [NonAction]
//        public ViewResult View(SymbolId viewName, SymbolId masterName)
//        {
//            return View(viewName, masterName, null /* model */);
//        }

//        [NonAction]
//        public ViewResult View(string viewName, SymbolId masterName)
//        {
//            return View(viewName, masterName, null /* model */);
//        }
//
        [NonAction]
        public new ViewResult View(string viewName, object model)
        {
            return View(viewName, null /* masterName */, model);
        }

//        [NonAction]
//        public ViewResult View(SymbolId viewName, object model)
//        {
//            return View(viewName, null /* masterName */, model);
//        }
//
//        [NonAction]
//        public ViewResult View(SymbolId viewName, string masterName, object model)
//        {
//            return View(viewName.ToString(), masterName, model);
//        }
//
//        [NonAction]
//        public ViewResult View(SymbolId viewName, SymbolId masterName, object model)
//        {
//            return View(viewName.ToString(), masterName.ToString(), model);
//        }
        
//        [NonAction]
//        public ViewResult View(string viewName, SymbolId masterName, object model)
//        {
//            return View(viewName, masterName.ToString(), model);
//        }
//
        [NonAction]
        public new ViewResult View(string viewName, string masterName, object model)
        {
            var vdd = new ViewDataDictionary();
//            vdd["__scriptRuntime"] = ((RubyEngine) _engine).Runtime;

            _engine.CallMethod(this, "fill_view_data");
            foreach (var entry in _viewData)
                vdd[Convert.ToString(entry.Key, CultureInfo.InvariantCulture)] = entry.Value;

            var hash = model as Hash;
            vdd.Model = (hash != null) ? new HashWrapper(hash) : model;
            ModelState.ForEach(pair => vdd.ModelState.Add(pair.Key.ToString(), pair.Value));

            return new ViewResult {ViewName = viewName, MasterName = masterName, ViewData = vdd, TempData = TempData};
        }

        public new virtual bool TryUpdateModel<TModel>(TModel model) where TModel : class
        {
            return TryUpdateModel(model, null, null, null, ValueProvider);
        }

        public new virtual bool TryUpdateModel<TModel>(TModel model, string prefix) where TModel : class
        {
            return TryUpdateModel(model, prefix, null, null, ValueProvider);
        }

        public new virtual bool TryUpdateModel<TModel>(TModel model, string[] includeProperties) where TModel : class
        {
            return TryUpdateModel(model, null, includeProperties, null, ValueProvider);
        }

        public new virtual bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties) where TModel : class
        {
            return TryUpdateModel(model, prefix, includeProperties, null, ValueProvider);
        }

        public new virtual bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) where TModel : class
        {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties, ValueProvider);
        }

        public new virtual bool TryUpdateModel<TModel>(TModel model, IValueProvider valueProvider) where TModel : class
        {
            return TryUpdateModel(model, null, null, null, valueProvider);
        }

        public new virtual bool TryUpdateModel<TModel>(TModel model, string prefix, IValueProvider valueProvider) where TModel : class
        {
            return TryUpdateModel(model, prefix, null, null, valueProvider);
        }

        public new virtual bool TryUpdateModel<TModel>(TModel model, string[] includeProperties, IValueProvider valueProvider) where TModel : class
        {
            return TryUpdateModel(model, null, includeProperties, null, valueProvider);
        }

        public new virtual bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, IValueProvider valueProvider) where TModel : class
        {
            return TryUpdateModel(model, prefix, includeProperties, null, valueProvider);
        }

        public new virtual bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties, IValueProvider valueProvider) where TModel : class
        {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties, valueProvider);
        }

        public new virtual void UpdateModel<TModel>(TModel model) where TModel : class
        {
            UpdateModel(model, null, null, null, ValueProvider);
        }

        public new virtual void UpdateModel<TModel>(TModel model, string prefix) where TModel : class
        {
            UpdateModel(model, prefix, null, null, ValueProvider);
        }

        public new virtual void UpdateModel<TModel>(TModel model, string[] includeProperties) where TModel : class
        {
            UpdateModel(model, null, includeProperties, null, ValueProvider);
        }

        public new virtual  void UpdateModel<TModel>(TModel model, string prefix, string[] includeProperties) where TModel : class
        {
            UpdateModel(model, prefix, includeProperties, null, ValueProvider);
        }

        public new virtual  void UpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) where TModel : class
        {
            UpdateModel(model, prefix, includeProperties, excludeProperties, ValueProvider);
        }

        public new virtual  void UpdateModel<TModel>(TModel model, IValueProvider valueProvider) where TModel : class
        {
            UpdateModel(model, null, null, null, valueProvider);
        }

        public new virtual  void UpdateModel<TModel>(TModel model, string prefix, IValueProvider valueProvider) where TModel : class
        {
            UpdateModel(model, prefix, null, null, valueProvider);
        }

        public new virtual  void UpdateModel<TModel>(TModel model, string[] includeProperties, IValueProvider valueProvider) where TModel : class
        {
            UpdateModel(model, null, includeProperties, null, valueProvider);
        }

        public new virtual  void UpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, IValueProvider valueProvider) where TModel : class
        {
            UpdateModel(model, prefix, includeProperties, null, valueProvider);
        }

        public new virtual  void UpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties, IValueProvider valueProvider) where TModel : class
        {
            base.UpdateModel(model, prefix, includeProperties, excludeProperties, valueProvider);
        }

       


        [NonAction]
        public new IDictionary ViewData()
        {
            return _viewData;
        }
    }
}