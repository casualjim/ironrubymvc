#region Usings

using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using IronRuby.Builtins;
using System.Web.Mvc.IronRuby.Extensions;
using Xunit;

#endregion

namespace IronRubyMvc.Tests.Extensions
{
    public class DictionaryExtensionsFixture
    {

        // Moved to bacon spec
        /*[Fact]
        public void ShouldBeAbleToConvertHashToRouteDictionary()
        {
            var expected = new RouteValueDictionary {{"first", "first_action"}, {"second", "second action"}};

            var hash = new Hash(new Dictionary<object, object> {{"first", "first_action"}, {"second", "second action"}});

            var actual = hash.ToRouteDictionary();

            foreach (var pair in expected)
            {
                Assert.NotNull(actual[pair.Key]);
                Assert.Equal(pair.Value, actual[pair.Key]);
            }
        }

        [Fact]
        public void ShouldBeAbleToConverToViewDataDictionary()
        {
            var expected = new ViewDataDictionary {{"first", "first_action"}, {"second", "second action"}};

            var hash = new Hash(new Dictionary<object, object> {{"first", "first_action"}, {"second", "second action"}});

            var actual = hash.ToViewDataDictionary();

            foreach (var pair in expected)
            {
                Assert.NotNull(actual[pair.Key]);
                Assert.Equal(pair.Value, actual[pair.Key]);
            }
        }*/
    }
}