using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;

namespace src.Routing
{
    public interface IRouteResolver
    {
        Task<IResolveResult> Resolve(RouteContext context, RequestCulture culture);
    }
}