using System.Threading.Tasks;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Routing;

namespace src.Routing
{
    public interface IRouteResolver
    {
        Task<IResolveResult> Resolve(RouteContext context, RequestCulture culture);
    }
}