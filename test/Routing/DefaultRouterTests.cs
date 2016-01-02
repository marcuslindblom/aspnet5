using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using src.Routing;
using Xunit;

namespace Tests.Routing
{
    public class DefaultRouterTests
    {
        private static readonly RequestDelegate NullHandler = (c) => Task.FromResult(0);

        public class RouteAsync
        {
            [Theory(Skip = "Fucked up test")]
            [InlineData("")]
            [InlineData("/")]
            public async Task Can_Resolve_Home_Page_With_Default_Action(string path)
            {
                // Arrange
                var context = CreateRouteContext(path);
                var route = CreateDefaultRouter(context, new RequestCulture("en"), It.IsAny<IVirtualPathResolver>());

                // Act
                await route.RouteAsync(context);

                // Assert
                Assert.NotNull(context);
                Assert.Equal(2, context.RouteData.Values.Count);
                Assert.Equal("Home", context.RouteData.Values["controller"]);
                Assert.Equal("Index", context.RouteData.Values["action"]);
            }           
        }

        public class GetVirtualPath
        {
            [Theory]
            [InlineData("/")]
            public void Can_Resolve(string path)
            {
                // Arrange
                var context = CreateRouteContext(path);
                var virtualPathContext = CreateVirtualPathContext(new { });

                var virtualPathResolver = new Mock<IVirtualPathResolver>(MockBehavior.Strict);
                virtualPathResolver.Setup(v => v.Resolve(virtualPathContext))
                    .Returns("/about");
                
                var route = CreateDefaultRouter(context, new RequestCulture("en"), virtualPathResolver.Object);

                // Act
                VirtualPathData data = route.GetVirtualPath(virtualPathContext);

                // Assert
                Assert.NotNull(data);
                Assert.Equal("/about", data.VirtualPath);
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

            var context = new Mock<HttpContext>();
            context.SetupGet(c => c.Items)
                   .Returns(new Dictionary<object, object>());
            context.Setup(m => m.RequestServices.GetService(typeof(ILoggerFactory)))
                .Returns(factory);
            context.SetupGet(c => c.Request).Returns(request.Object);

            return new RouteContext(context.Object);
        }

        private static VirtualPathContext CreateVirtualPathContext(object routeValues, object ambientValues = null)
        {
            return new VirtualPathContext(
                new Mock<HttpContext>(MockBehavior.Strict).Object,
                new RouteValueDictionary(ambientValues),
                new RouteValueDictionary(routeValues));
        }

        private static DefaultRouter CreateDefaultRouter(RouteContext context, RequestCulture culture, IVirtualPathResolver virtualPathResolver)
        {
            var result = new Mock<IResolveResult>(MockBehavior.Strict);
            result.SetupGet(x => x.Controller).Returns("Home");
            result.SetupGet(x => x.Action).Returns("Index");

            var routeResolver = new Mock<IRouteResolver>(MockBehavior.Strict);
            routeResolver.Setup(x => x.Resolve(context, culture))
                .ReturnsAsync(result.Object);



            return new DefaultRouter(
                CreateTarget(),
                routeResolver.Object,
                virtualPathResolver);
        }

        private static IRouter CreateTarget(bool handleRequest = true)
        {
            var target = new Mock<IRouter>(MockBehavior.Strict);
            target
                .Setup(e => e.GetVirtualPath(It.IsAny<VirtualPathContext>()))
                .Callback<VirtualPathContext>(c => c.IsBound = handleRequest)
                .Returns<VirtualPathContext>(rc => null);

            target
                .Setup(e => e.RouteAsync(It.IsAny<RouteContext>()))
                .Callback<RouteContext>((c) => c.IsHandled = handleRequest)
                .Returns(Task.FromResult<object>(null));

            return target.Object;
        }
    }
}
