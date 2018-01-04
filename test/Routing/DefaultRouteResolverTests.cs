using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using src.Mvc;
using src.Routing;
using src.Routing.Trie;
using Xunit;

namespace Tests.Routing
{
    public class DefaultRouteResolverTests
    {
        public class Resolve
        {
            [Theory]
            [InlineData("")]
            [InlineData("/")]
            public async Task Can_Resolve_Home_Page_With_Default_Action(string path)
            {
                // Arrange
                var context = CreateRouteContext(path);
                var trie = CreateTrie();
                var requestCulture = new RequestCulture("en");

                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync(requestCulture)).ReturnsAsync(trie);

                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);
                mapper.Setup(x => x.ControllerHasAction("Home", "Index")).Returns(true);

                var routeResolver = new DefaultRouteResolver(trieResolver.Object, mapper.Object);

                // Act
                IResolveResult result = await routeResolver.Resolve(context, requestCulture);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Home", result.Controller);
                Assert.Equal("Index", result.Action);
            }

            [Theory]
            [InlineData("/about")]
            [InlineData("/about/")]
            public async Task Can_Resolve_Home_Page_With_Custom_Action(string path)
            {
                // Arrange
                var context = CreateRouteContext(path);
                var trie = CreateTrie();
                var requestCulture = new RequestCulture("en");

                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync(requestCulture)).ReturnsAsync(trie);

                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);
                mapper.Setup(x => x.ControllerHasAction("Home", "about")).Returns(true);

                var routeResolver = new DefaultRouteResolver(trieResolver.Object, mapper.Object);

                // Act
                IResolveResult result = await routeResolver.Resolve(context, requestCulture);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Home", result.Controller);
                Assert.Equal("about", result.Action);
            }

            [Theory]
            [InlineData("/article")]
            [InlineData("/article/")]
            public async Task Can_Resolve_Sub_Page_With_Default_Action(string path)
            {
                // Arrange
                var context = CreateRouteContext(path);
                var trie = CreateTrie();
                var requestCulture = new RequestCulture("en");

                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync(requestCulture)).ReturnsAsync(trie);

                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);
                mapper.Setup(x => x.ControllerHasAction("Article", "article")).Returns(false);
                mapper.Setup(x => x.ControllerHasAction("Article", "Index")).Returns(true);

                var routeResolver = new DefaultRouteResolver(trieResolver.Object, mapper.Object);

                // Act
                IResolveResult result = await routeResolver.Resolve(context, requestCulture);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Article", result.Controller);
                Assert.Equal("Index", result.Action);
            }

            [Theory]
            [InlineData("/en")]
            [InlineData("/en/")]
            public async Task Can_Resolve_Localized_Home_Page_With_Default_Action(string path)
            {
                // Arrange
                var context = CreateRouteContext(path);
                var trie = CreateTrie();
                var requestCulture = new RequestCulture("en");

                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync(requestCulture)).ReturnsAsync(trie);

                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);
                mapper.Setup(x => x.ControllerHasAction("Home", "Index")).Returns(true);

                var routeResolver = new DefaultRouteResolver(trieResolver.Object, mapper.Object);

                // Act
                IResolveResult result = await routeResolver.Resolve(context, requestCulture);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Home", result.Controller);
                Assert.Equal("Index", result.Action);
            }
        }

        private static RouteContext CreateRouteContext(string requestPath, ILoggerFactory factory = null)
        {
            if (factory == null)
            {
                factory = new LoggerFactory();
            }

            var request = new Mock<HttpRequest>(MockBehavior.Strict);
            request.SetupGet(r => r.Path).Returns(new PathString(requestPath));

            var context = new Mock<HttpContext>(MockBehavior.Strict);
            context.Setup(m => m.RequestServices.GetService(typeof(ILoggerFactory)))
                .Returns(factory);
            context.SetupGet(c => c.Request)
                .Returns(request.Object);
            context.SetupGet(c => c.Items)
                   .Returns(new Dictionary<object, object>());

            return new RouteContext(context.Object);
        }

        private static Trie CreateTrie()
        {
            var trie = new Trie
                {
                    {"/", new TrieNode("homes/1", "Home")},
                    {"/about", new TrieNode("homes/1", "Home")},
                    {"/article", new TrieNode("articles/1", "Article")}
                };
            return trie;
        }
    }
}
