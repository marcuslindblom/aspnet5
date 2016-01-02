﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync())
                    .ReturnsAsync(trie);
                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);
                var httpAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);

                var page = new Page
                {
                    Id = "homes/1"
                };

                var virtualPathResolver = new DefaultVirtualPathResolver(trieResolver.Object, mapper.Object, httpAccessor.Object);
                var virtualPathContext = CreateVirtualPathContext(new { page });

                // Act
                var result = virtualPathResolver.Resolve(virtualPathContext);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.HasValue);
                Assert.Equal("/", result.Value);
            }

            [Fact]
            public void Can_Resolve_Path_To_Page_With_Culture_Set_To_SV()
            {
                // Arrange
                var trie = CreateTrie();
                var trieResolver = new Mock<IRouteResolverTrie>(MockBehavior.Strict);
                trieResolver.Setup(t => t.LoadTrieAsync())
                    .ReturnsAsync(trie);
                var mapper = new Mock<IControllerMapper>(MockBehavior.Strict);
                var httpAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
                var page = new Page
                {
                    Id = "articles/1"
                };

                var virtualPathResolver = new DefaultVirtualPathResolver(trieResolver.Object, mapper.Object, httpAccessor.Object);
                var virtualPathContext = CreateVirtualPathContext(new { page = page, culture = "sv" });

                // Act
                var result = virtualPathResolver.Resolve(virtualPathContext);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.HasValue);
                Assert.Equal("/sv/article", result.Value);
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
