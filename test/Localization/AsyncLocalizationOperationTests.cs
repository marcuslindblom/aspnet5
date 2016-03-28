using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Razor;
using Moq;
using Raven.Client;
using src;
using src.Localization;
using src.Routing.Trie;
using Xunit;

namespace Tests.Localization
{
    public class AsyncLocalizationOperationTests
    {
        public class StoreAsync
        {
            [Fact(Skip = "Skip for now")]
            public void Can_Store_New_Item()
            {

                // Arrange

                var page = new Page { Name = "New Page" };
                var model = new FakeModel();
                var culture = new CultureInfo("en");
                var site = new Site("English", culture);
                site.Trie = CreateTrie();

                var session = new Mock<IAsyncDocumentSession>();
                session.Setup(documentSession => documentSession.StoreAsync(page, CancellationToken.None));
                session.Setup(documentSession => documentSession.LoadAsync<Site>("sites/en", CancellationToken.None))
                    .ReturnsAsync(site);

                // Act

                var operation = new AsyncLocalizationOperation(session.Object, culture);
                operation.Page = new Page { Id = "pages/1" };

                Task task = Task.Run(() => operation.ForModel(model).ForUrl("/a-new-page").StoreAsync(page));
                task.Wait();

                // Assert

                Assert.True(task.IsCompleted);
                Assert.Equal(4, site.Trie.Count);

            }

            [Fact]
            public void Can_Update_Key_For_Item()
            {

                // Arrange

                var page = new Page { Name = "New Page" };
                var model = new FakeModel();
                var culture = new CultureInfo("en");
                var site = new Site("English", culture);
                site.Trie = CreateTrie();

                var session = new Mock<IAsyncDocumentSession>();
                session.Setup(documentSession => documentSession.StoreAsync(page, CancellationToken.None));
                session.Setup(documentSession => documentSession.LoadAsync<Site>("sites/en", CancellationToken.None))
                    .ReturnsAsync(site);

                // Act

                var operation = new AsyncLocalizationOperation(session.Object, culture);
                operation.Page = new Page { Id = "articles/1" };

                Task task = Task.Run(() => operation.ForModel(model).ForUrl("/changed-slug").StoreAsync(page));
                task.Wait();

                // Assert

                Assert.True(task.IsCompleted);
                Assert.Equal(3, site.Trie.Count);
                Assert.True(site.Trie.Keys.Contains("/changed-slug"));

            }
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

        public class FakeModel
        {
             
        }

    }
}
