using System;
using System.Web.Mvc;
using System.Web.Mvc.IronRuby.Controllers;
using System.Web.Mvc.IronRuby.Core;
using System.Web.Mvc.IronRuby.Extensions;
using System.Web.Routing;
using Moq;
using Moq.Mvc;
using Xunit;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Text;
using IronRuby.Builtins;

namespace System.Web.Mvc.IronRuby.Tests.Controllers
{
    public abstract class with_ironruby_mvc_and_routes_file : InstanceContextSpecification<RubyControllerFactory>
    {
        protected IPathProvider _pathProvider;
        protected IRubyEngine _rubyEngine;
        protected RequestContext _requestContext;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            //create a routes.rb file in current directory
            createRoutesFile("routes.rb");

            _pathProvider = An<IPathProvider>();
            _pathProvider.WhenToldTo(pp => pp.ApplicationPhysicalPath).Return(Environment.CurrentDirectory);
            _pathProvider.WhenToldTo(pp => pp.FileExists("~/routes.rb")).Return(true);
            _pathProvider.WhenToldTo(pp => pp.MapPath("~/routes.rb")).Return("routes.rb");
            RouteTable.Routes.Clear();

            _requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());

            RubyEngine _theRubyEngine = RubyEngine.InitializeIronRubyMvc(_pathProvider, "~/routes.rb");
            _rubyEngine = _theRubyEngine;
        }

        /// <summary>
        /// create a default routes file in path
        /// </summary>
        /// <param name="path">full path name for the routes file to create</param>
        private void createRoutesFile(string path)
        {
            var script = new StringBuilder();

            script.AppendLine("#default routes");
            script.AppendLine("");
            script.AppendLine("$routes.ignore_route(\"{resource}.axd/{*pathInfo}\");");
            script.AppendLine("");
            script.AppendLine("$routes.map_route(\"default\", \"{controller}/{action}/{id}\",");
            script.AppendLine("  {:controller => 'Home', :action => 'index', :id => ''}");
            script.AppendLine(")");
            string value = script.ToString();

            CreateFile(path, value);
        }

        /// <summary>
        /// Creates a file with content value in path
        /// </summary>
        /// <param name="path">file full path name</param>
        /// <param name="value">file content as string</param>
        protected void CreateFile(string path, string value)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(ASCIIEncoding.Default.GetBytes(value));
            bw.Flush();
            bw.Close();
            fs.Close();
        }
    }

    [Concern(typeof(RubyControllerFactory))]
    public abstract class with_ironruby_mvc_and_routes_and_controller_file : with_ironruby_mvc_and_routes_file
    {
        protected const string _controllerName = "My";
        protected const string _controllerClassName = _controllerName + "Controller";

        protected string _mappedControllerPath = _controllerClassName + ".rb";
        protected string _virtualControllerPath = @"~\Controllers\{0}.rb"
            .FormattedWith(_controllerClassName);

        protected IControllerFactory _controllerFactory;
        protected IController _controller;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            createControllerFile(_mappedControllerPath, _controllerClassName);

            _pathProvider.WhenToldTo(pp => pp.FileExists(_virtualControllerPath)).Return(true);
            _pathProvider.WhenToldTo(pp => pp.MapPath(_virtualControllerPath)).Return(_mappedControllerPath);
        }

        protected virtual void createControllerFile(string path, string controllerName)
        {
            var script = new StringBuilder();
            script.AppendLine("class {0} < Controller".FormattedWith(controllerName));
            script.AppendLine("  def my_action");
            script.AppendLine("    $counter = $counter + 5");
            script.AppendLine("    \"Can't see ninjas\".to_clr_string");
            script.AppendLine("  end");
            script.AppendLine("end");
            string value = script.ToString();

            CreateFile(path, value);
        }
    }

    
    [Concern(typeof(RubyControllerFactory))]
    public class when_a_ruby_controller_needs_to_be_resolved : with_ironruby_mvc_and_routes_and_controller_file
    {
        protected override RubyControllerFactory CreateSut()
        {
            return (RubyControllerFactory)ControllerBuilder.Current.GetControllerFactory();
        }

        protected override void Because()
        {
            _controller = Sut.CreateController(_requestContext, _controllerName);
        }

        [Observation]
        public void should_have_returned_a_result()
        {
            _controller.ShouldNotBeNull();
        }

        [Observation]
        public void should_have_returned_a_controller()
        {
            _controller.ShouldBeAnInstanceOf<IController>();
        }

        [Observation]
        public void should_have_the_correct_controller_name()
        {
            (_controller as RubyController).ControllerName.ShouldBeEqualTo(_controllerName);
        }

        [Observation]
        public void should_have_the_correct_controller_class_name()
        {
            (_controller as RubyController).ControllerClassName.ShouldBeEqualTo(_controllerClassName);
        }

        //[Observation]
        //public void it_should_have_called_the_ruby_engine()
        //{
        //    _rubyEngine.WasToldTo(eng => eng.GetRubyClass(_controllerName)).OnlyOnce();
        //}

        //[Observation]
        //public void should_have_called_the_inner_controller_factory()
        //{
        //    _controllerFactory.WasToldTo(factory => factory.CreateController(_requestContext, _controllerName)).OnlyOnce();
        //}
    }

    [Concern(typeof(RubyControllerFactory))]
    public class when_a_ruby_controller_was_resolved_twice : with_ironruby_mvc_and_routes_and_controller_file
    {
        private const string methodToFilter = "index";

        private int actionFiltersCountFirst;
        private int actionFiltersCountSecond;

        protected override void createControllerFile(string path, string controllerName)
        {
            var script = new StringBuilder();
            script.AppendLine("class {0} < Controller".FormattedWith(controllerName));
            script.AppendLine("");
            script.AppendLine("  before_action :index do |context|");
            script.AppendLine("    context.request_context.http_context.response.write(\"Hello world<br />\")");
            script.AppendLine("  end");
            script.AppendLine("");
            script.AppendLine("  def {0}".FormattedWith(methodToFilter));
            script.AppendLine("    $counter = $counter + 5");
            script.AppendLine("    \"Can't see ninjas\".to_clr_string");
            script.AppendLine("  end");
            script.AppendLine("end");
            string value = script.ToString();

            CreateFile(path, value);
        }

        protected override RubyControllerFactory CreateSut()
        {
            return (RubyControllerFactory) ControllerBuilder.Current.GetControllerFactory();
        }

        protected override void Because()
        {
            _controller = Sut.CreateController(_requestContext, _controllerName);
            actionFiltersCountFirst = getActionFiltersCount(_controller);
            _controller = Sut.CreateController(_requestContext, _controllerName);
            actionFiltersCountSecond = getActionFiltersCount(_controller);
        }

        private int getActionFiltersCount(IController _controller)
        {
            var rubyType = ((RubyController)_controller).RubyType;
            var controllerFilters = (Hash)_rubyEngine.CallMethod(rubyType, "action_filters");

            int count = 0;
            controllerFilters.ToActionFilters(methodToFilter).ForEach(action => count++);
            return count;
        }

        [Observation]
        public void action_filters_count_should_be_equal()
        {
            actionFiltersCountSecond.ShouldBeEqualTo(actionFiltersCountFirst);
        }
    }



//    [Concern(typeof(RubyControllerFactory))]
//    public class when_a_ruby_controller_needs_to_be_resolved : InstanceContextSpecification<RubyControllerFactory>
//    {
//        private IRubyEngine _rubyEngine;
//        private IControllerFactory _controllerFactory;
//        private RequestContext _requestContext;
//        private const string _controllerName = "my_controller";
//        private IController _controller;
//
//        protected override void EstablishContext()
//        {
//            _rubyEngine = Dependency<IRubyEngine>();
//            _controllerFactory = Dependency<IControllerFactory>();
//            _requestContext = new RequestContext(new HttpContextMock().Object, new RouteData());
//
//            _controllerFactory
//                .WhenToldTo(factory => factory.CreateController(_requestContext, _controllerName))
//                .Throw(new InvalidOperationException());
//            
//            _rubyEngine.WhenToldTo(eng => eng.LoadController(_requestContext, _controllerName)).Return(Dependency<RubyController>());
//            
//        }
//
//        protected override RubyControllerFactory CreateSut()
//        {
//            return new RubyControllerFactory(_controllerFactory, _rubyEngine);
//        }
//
//        protected override void Because()
//        {
//            _controller = Sut.CreateController(_requestContext, _controllerName);
//        }
//
//        [Observation]
//        public void should_have_returned_a_result()
//        {
//            _controller.ShouldNotBeNull();
//        }
//
//        [Observation]
//        public void should_have_returned_a_controller()
//        {
//            _controller.ShouldBeAnInstanceOf<IController>();
//        }
//
//        [Observation]
//        public void it_should_have_called_the_ruby_engine()
//        {
//            _rubyEngine.WasToldTo(eng => eng.LoadController(_requestContext, _controllerName)).OnlyOnce();
//        }
//
//        [Observation]
//        public void should_have_called_the_inner_controller_factory()
//        {
//            _controllerFactory.WasToldTo(factory => factory.CreateController(_requestContext, _controllerName)).OnlyOnce();
//        }
//    }

    [Concern(typeof(RubyControllerFactory))]
    public class when_a_ruby_controller_needs_to_be_disposed: InstanceContextSpecification<RubyControllerFactory>
    {
        private IRubyEngine _rubyEngine;
        private IControllerFactory _controllerFactory;
        private IController _controller;
        private IPathProvider _pathProvider;

        protected override void EstablishContext()
        {
            _rubyEngine = An<IRubyEngine>();
            _controllerFactory = An<IControllerFactory>();
            _pathProvider = An<IPathProvider>();

            _controller = An<RubyController>();
        }

        protected override RubyControllerFactory CreateSut()
        {
            return new RubyControllerFactory(_pathProvider, _controllerFactory, _rubyEngine);
        }

        protected override void Because()
        {
            Sut.ReleaseController(_controller);
        }

        [Observation]
        public void should_have_called_dispose()
        {
            _controller.WasToldTo(c => ((IDisposable)c).Dispose()).OnlyOnce();
        }
        
    }
}