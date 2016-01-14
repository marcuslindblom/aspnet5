using Microsoft.AspNet.Http;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Routing;
using Moq;
using src;
using src.Mvc;
using src.Routing;
using src.Routing.Trie;
using Xunit;

namespace Tests.Routing
{
    public class DefaultVirtualPathResolverTests
    {
        public class Resolve
        {
            [Fact]
            public void Can_Resolve_Path_To_Page_With_Default_Culture()
            {
                // Arrange
                var trie = CreateTrie();

                var currentRequestCulture = new Mock<RequestCulture>("en");

                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync(currentRequestCulture.Object))
                    .ReturnsAsync(trie);

                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);                

                var page = new Page
                {
                    Id = "homes/1"
                };

                var virtualPathResolver = new DefaultVirtualPathResolver(trieResolver.Object, mapper.Object);
                var virtualPathContext = CreateVirtualPathContext(new { page });

                var defaultRequestCulture = new RequestCulture("en");

                // Act
                var result = virtualPathResolver.Resolve(virtualPathContext, defaultRequestCulture, currentRequestCulture.Object);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.HasValue);
                Assert.Equal("/", result.Value);
            }

            [Fact]
            public void Can_Resolve_Path_To_Page_To_Specified_Culture()
            {
                // Arrange
                var trie = CreateTrie();
                var currentRequestCulture = new Mock<RequestCulture>("en");

                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync(It.IsAny<RequestCulture>()))
                    .ReturnsAsync(trie);
                
                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);
                
                var page = new Page
                {
                    Id = "articles/1"
                };

                var virtualPathResolver = new DefaultVirtualPathResolver(trieResolver.Object, mapper.Object);
                var virtualPathContext = CreateVirtualPathContext(new { page, culture = "sv" });
                var defaultRequestCulture = new RequestCulture("en");

                // Act
                var result = virtualPathResolver.Resolve(virtualPathContext, defaultRequestCulture, currentRequestCulture.Object);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.HasValue);
                Assert.Equal("/sv/article", result.Value);
            }

            [Fact]
            public void Can_Resolve_Path_To_Page_With_Default_Culture_Using_Id()
            {
                // Arrange
                var trie = CreateTrie();
                var currentRequestCulture = new Mock<RequestCulture>("en");

                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync(It.IsAny<RequestCulture>()))
                    .ReturnsAsync(trie);

                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);

                var page = new Page
                {
                    Id = "articles/1"
                };

                var virtualPathResolver = new DefaultVirtualPathResolver(trieResolver.Object, mapper.Object);
                var virtualPathContext = CreateVirtualPathContext(new { id = page.Id });
                var defaultRequestCulture = new RequestCulture("en");

                // Act
                var result = virtualPathResolver.Resolve(virtualPathContext, defaultRequestCulture, currentRequestCulture.Object);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.HasValue);
                Assert.Equal("/article", result.Value);
            }

            [Fact]
            public void Can_Resolve_Path_To_Page_With_Culture_Set_To_Same_As_Default_Culture()
            {
                // Arrange
                var trie = CreateTrie();
                var currentRequestCulture = new Mock<RequestCulture>("en");

                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync(It.IsAny<RequestCulture>()))
                    .ReturnsAsync(trie);

                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);

                var page = new Page
                {
                    Id = "articles/1"
                };

                var virtualPathResolver = new DefaultVirtualPathResolver(trieResolver.Object, mapper.Object);
                var virtualPathContext = CreateVirtualPathContext(new { page, culture = "en" });
                var defaultRequestCulture = new RequestCulture("en");

                // Act
                var result = virtualPathResolver.Resolve(virtualPathContext, defaultRequestCulture, currentRequestCulture.Object);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.HasValue);
                Assert.Equal("/article", result.Value);
            }

            [Fact]
            public void Can_Resolve_Path_To_Page_With_Extra_Route_Values()
            {
                // Arrange
                var trie = CreateTrie();
                var currentRequestCulture = new Mock<RequestCulture>("en");

                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync(It.IsAny<RequestCulture>()))
                    .ReturnsAsync(trie);

                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);

                var page = new Page
                {
                    Id = "articles/1"
                };

                var virtualPathResolver = new DefaultVirtualPathResolver(trieResolver.Object, mapper.Object);
                var virtualPathContext = CreateVirtualPathContext(new { page, foo = "bar" });
                var defaultRequestCulture = new RequestCulture("en");

                // Act
                var result = virtualPathResolver.Resolve(virtualPathContext, defaultRequestCulture, currentRequestCulture.Object);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.HasValue);
                Assert.Equal("/article?foo=bar", result.Value);
            }
        }

        private static VirtualPathContext CreateVirtualPathContext(object routeValues, object ambientValues = null)
        {
            return new VirtualPathContext(
                new Mock<HttpContext>(MockBehavior.Strict).Object,
                new RouteValueDictionary(ambientValues),
                new RouteValueDictionary(routeValues));
        }

        private static Trie CreateTrie()
        {
            var trie = new Trie
                {
                    {"/", new TrieNode("homes/1", "Home", "Home")},
                    {"/about", new TrieNode("homes/1", "About", "Home")},
                    {"/article", new TrieNode("articles/1", "Article", "Article")}
                };
            return trie;
        }
    }
}
