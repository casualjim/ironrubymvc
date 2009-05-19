using System;
using System.Web.Mvc.IronRuby.Extensions;
using IronRuby;
using IronRuby.Runtime;
using IronRuby.Builtins;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;

namespace IronRubyMvcWorkarounds
{
    public class BugWorkarounds
    {
        // IronRuby currently is a little bit confused about Actions and extension methods when called with nil/null
        // These methods get around those confusions
        public static bool IsNull(object value) { return value.IsNull(); }
        public static bool IsNotNull(object value) { return value.IsNotNull(); }
        public static bool IsNullOrBlank(string value) { return value.IsNullOrBlank(); }
        public static bool IsNotNullOrBlank(string value) { return value.IsNotNullOrBlank(); }
        public static bool IsEmpty(IEnumerable collection) { return collection.IsEmpty(); }
        public static bool IsEmpty<T>(IEnumerable<T> collection) { return collection.IsEmpty(); }
        public static Action<object> WrapProc(Proc proc) { return obj => proc.Call(obj); }
        public static Action<object, object> WrapProc2(Proc proc) { return (obj1, obj2) => proc.Call(obj1, obj2); }
        public static Action<T> WrapProc<T>(Proc proc) { return obj => proc.Call(obj); }

        // I couldn't get to the static Ruby class to get the ScriptRuntime going
        public static ScriptRuntime CreateScriptRuntime(){
            var rubySetup = Ruby.CreateRubySetup();
                rubySetup.Options["InterpretedMode"] = true;

                var runtimeSetup = new ScriptRuntimeSetup();
                runtimeSetup.LanguageSetups.Add(rubySetup);
                runtimeSetup.DebugMode = true;

            return Ruby.CreateRuntime(runtimeSetup);
        }

        public static ScriptEngine GetRubyEngine(ScriptRuntime runtime){
            return runtime.GetRubyEngine();        
        }

        public static RubyContext GetExecutionContext(ScriptEngine engine){
            return Ruby.GetExecutionContext(engine);
        }
    }

    #region MockTest

    // These classes are here just for a while to figure out how mocking could work when mixing IronRuby and CLR

    public interface IWeapon{
        void Attack(IWarrior warrior);
        int Damage();
    }

    public interface IWarrior
    {
        bool IsKilledBy(IWeapon weapon);
        void Attack(IWarrior target, IWeapon weapon);
    }

    public class Ninja : IWarrior{

        public void Attack(IWarrior target, IWeapon weapon){
            weapon.Attack(target);
        }
        
        public bool IsKilledBy(IWeapon weapon)
        {
            return weapon.Damage() > 3;
        }
    }
    #endregion
}