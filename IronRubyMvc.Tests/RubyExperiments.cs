#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using IronRuby;
using IronRuby.Builtins;
using IronRuby.Runtime;
using IronRuby.Runtime.Calls;
using System.Web.Mvc.IronRuby.Extensions;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using Xunit;

#endregion

namespace System.Web.Mvc.IronRuby.Tests
{
    public class RubyRuntime
    {
        public ScriptRuntime CreateRuntime()
        {
            var rubySetup = Ruby.CreateRubySetup();
            rubySetup.Options["InterpretedMode"] = true;

            var runtimeSetup = new ScriptRuntimeSetup();
            runtimeSetup.LanguageSetups.Add(rubySetup);
            runtimeSetup.DebugMode = true;

            return Ruby.CreateRuntime(runtimeSetup);
        }
    }

    public class RubyVariableDictionary : CustomSymbolDictionary
    {
        public override SymbolId[] GetExtraKeys()
        {
            throw new NotImplementedException();
        }

        protected override bool TrySetExtraValue(SymbolId key, object value)
        {
            throw new NotImplementedException();
        }

        protected override bool TryGetExtraValue(SymbolId key, out object value)
        {
            throw new NotImplementedException();
        }
    }

    public class RubyExperiments : IUseFixture<RubyRuntime>
    {
        private static ScriptRuntime _scriptRuntime;
        private RubyContext _context;
        private ScriptEngine _engine;

        #region IUseFixture<RubyRuntime> Members

        public void SetFixture(RubyRuntime data)
        {
            if (_scriptRuntime == null) _scriptRuntime = data.CreateRuntime();

            _engine = _scriptRuntime.GetRubyEngine();
            _context = Ruby.GetExecutionContext(_engine);
        }

        #endregion

        [Fact]
        public void ShouldHaveAnEngine()
        {
            Assert.NotNull(_engine);
        }

        [Fact]
        public void ShouldHaveAContext()
        {
            Assert.NotNull(_context);
        }

        [Fact]
        public void ShouldSetAndFetchGlobalVariables()
        {
            var varName = "global_var";
            var varValue = "my variable";
            _context.DefineGlobalVariable(varName, varValue);

            _context.GetGlobalVariable(varName).ShouldBeEqualTo(varValue);
        }

        [Fact]
        public void ShouldExecuteScript()
        {
            var script =
                "class MyClass; def my_method; $text_var = \"Hello world\"; end; end; MyClass.new.my_method ";
            _context.DefineGlobalVariable("text_var", "String value");

            var scriptSource = _engine.CreateScriptSourceFromString(script, SourceCodeKind.AutoDetect);
            scriptSource.Execute();

            "Hello world".ShouldBeEqualTo(_context.GetGlobalVariable("text_var").ToString());
        }

        [Fact]
        public void ShouldGetARubyClass()
        {
            var method = MethodBase.GetCurrentMethod();
            AddClass(method);
            var className = "{0}Class".FormattedWith(method.Name);
            var rClass = _scriptRuntime.Globals.GetVariable<RubyClass>(className);
            rClass.ShouldNotBeNull();
            rClass.ShouldBeAnInstanceOf<RubyClass>();
            className.ShouldBeEqualTo(rClass.Name);
            var methods = new List<string>();
            using (_context.ClassHierarchyLocker())
            {
                rClass.EnumerateMethods((module, symbolId, memberInfo) =>
                                            {
                                                var message =
                                                    "Module: {0}, Name: {1}, memberInfoType: {2}".FormattedWith(
                                                        module.Name, symbolId, memberInfo.GetType().Name);
                                                Console.WriteLine(message);
                                                methods.Add(message);
                                                return false;
                                            });
            }
            methods.ShouldNotBeEmpty();
            methods.Count.ShouldBeEqualTo(2);
        }

        [Fact]
        public void ShouldGetMethodInfoThroughEnumerateMethods()
        {
            var method = MethodBase.GetCurrentMethod();
            AddClass(method);
            var className = "{0}Class".FormattedWith(method.Name);
            var rClass = _scriptRuntime.Globals.GetVariable<RubyClass>(className);
            var methods = new Dictionary<string, RubyMethodInfo>();
            using (_context.ClassHierarchyLocker())
            {
                rClass.EnumerateMethods((module, symbolId, memberInfo) =>
                                            {
                                                var message =
                                                    "Module: {0}, Name: {1}, memberInfoType: {2}".FormattedWith(
                                                        module.Name, symbolId, memberInfo.GetType().Name);
                                                Console.WriteLine(message);
                                                methods.Add(symbolId, (RubyMethodInfo) memberInfo);
                                                return true;
                                            });
            }

            var methodInfo = methods["my_method"];
            //methodInfo.Method.ShouldNotBeNull();
            methodInfo.DefinitionName.ShouldNotBeNull();
            methodInfo.DefinitionName.ShouldBeEqualTo("my_method");
        }

        [Fact]
        public void ShouldGetMethodInfoThroughGetMethods()
        {
            var method = MethodBase.GetCurrentMethod();
            AddClass(method);
            var className = "{0}Class".FormattedWith(method.Name);
            var rClass = _scriptRuntime.Globals.GetVariable<RubyClass>(className);
            var methods = new Dictionary<string, RubyMethodInfo>();

            methods["my_method"] = (RubyMethodInfo) rClass.GetMethod("my_method");

            var methodInfo = methods["my_method"];
            //methodInfo.Method.ShouldNotBeNull();
            methodInfo.DefinitionName.ShouldNotBeNull();
            methodInfo.DefinitionName.ShouldBeEqualTo("my_method");
        }

        [Fact]
        public void ShouldBeAbleToWorkWithScopedVariables()
        {
            var scope = _engine.CreateScope();
            AddClass(scope);
            var script =
                "my_instance = TestClass.new" + Environment.NewLine +
                "result1 = my_instance.add_is_expired 'first result'" + Environment.NewLine +
                "result2 = my_instance.add_is_expired a_test_var" + Environment.NewLine + " ";

            var operations = _engine.CreateOperations(scope);
            _engine.Execute("a_test_var = 'My content'", scope);
//            scope.SetVariable("a_test_var", "My content");
            var scriptResult = _engine.Execute(script, scope);

            _engine.Execute<string>("result1.to_clr_string", scope).ShouldBeEqualTo("first result is expired ");
            _engine.Execute<string>("result2.to_clr_string", scope).ShouldBeEqualTo("My content is expired ");
        }

        [Fact]
        public void ShouldBeAbleToGetClassName()
        {
            var scope = _engine.CreateScope();


            AddClass(scope);
            _scriptRuntime.Globals.ContainsVariable("TestClass").ShouldBeTrue();
            var result = _scriptRuntime.Globals.GetVariable<RubyClass>("TestClass");
//            _engine.ContainsVariable(scope, "TestClass").ShouldBeTrue();
            result.ShouldNotBeNull();
            result.ShouldBeAnInstanceOf<RubyClass>();
        }

        [Fact]
        public void ShouldBeAbleToCreateAnInstance()
        {
            var scope = _engine.CreateScope();
            var operations = _engine.CreateOperations(scope);
            AddClass(scope);
            var klass = _scriptRuntime.Globals.GetVariable<RubyClass>("TestClass");
//            var instance = operations.Call(klass);
//            instance.ShouldNotBeNull();

            var instanc2 = operations.CreateInstance(klass);
            instanc2.ShouldNotBeNull();
            operations.ContainsMember(instanc2, "add_is_expired").ShouldBeTrue();
        }

        [Fact]
        public void ShouldBeAbleToInvokeInstanceMethods()
        {
            var scope = _engine.CreateScope();
            var operations = _engine.CreateOperations(scope);
            AddClass(scope);
            var klass = _scriptRuntime.Globals.GetVariable<RubyClass>("TestClass");
            //            var instance = operations.Call(klass);
            //            instance.ShouldNotBeNull();

            var obj = operations.CreateInstance(klass);
            var member = operations.GetMember(obj, "add_is_expired");
            member.ShouldNotBeNull();
            member.ShouldBeAnInstanceOf<RubyMethod>();
            var method = (RubyMethod) member;
            operations.Invoke(method, "DLR context").ToString().ShouldBeEqualTo("DLR context is expired ");
        }

        [Fact]
        public void ShouldBeAbleToInvokeInstanceMethodsThatArePascalCased()
        {
            var scope = _engine.CreateScope();
            var operations = _engine.CreateOperations(scope);
            AddClass(scope);
            var klass = _scriptRuntime.Globals.GetVariable<RubyClass>("TestClass");
            //            var instance = operations.Call(klass);
            //            instance.ShouldNotBeNull();

            var obj = operations.CreateInstance(klass);
            var member = operations.GetMember<RubyMethod>(obj, "AddIsExpired".Underscore());
            member.ShouldNotBeNull();
            member.ShouldBeAnInstanceOf<RubyMethod>();
            operations.Invoke(member, "DLR context").ToString().ShouldBeEqualTo("DLR context is expired ");
        }

//        [Fact]
//        public void ShouldBeAbleToFindController()
//        {
//            ScriptScope scope = _engine.CreateScope();
//            object res = _engine.Execute("IronRubyMvc", scope);
//
//            Assert.True((bool) res);
//        }

        private void AddClass(ScriptScope scope)
        {
            var script =
                "class TestClass" + Environment.NewLine +
                "  def add_is_expired(value)" + Environment.NewLine +
                "    \"#{value} is expired \"" + Environment.NewLine +
                "  end" + Environment.NewLine +
                "end" + Environment.NewLine + Environment.NewLine;

            _engine.Execute(script, scope);
        }

        private void AddClass(MethodBase method)
        {
            var methodName = method.Name;
            var script =
                "class {0}; def my_method; $text_var = \"{1}\"; end; def another_method; $text_var = 'from other method'; end; end "
                    .FormattedWith("{0}Class".FormattedWith(methodName), methodName);
            _context.DefineGlobalVariable("text_var", "String value");

            _engine.CreateScriptSourceFromString(script, SourceCodeKind.AutoDetect).Execute();
        }
    }
}