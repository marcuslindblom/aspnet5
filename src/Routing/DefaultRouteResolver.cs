using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.Routing;
using src.Mvc;
using src.Routing.Trie;

namespace src.Routing
{
    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly IRouteResolverTrie _routeResolverTrie;
        private readonly IControllerMapper _controllerMapper;        

        //private const string RavenClrType = "Raven-Clr-Type";

        public DefaultRouteResolver(IRouteResolverTrie routeResolverTrie, IControllerMapper controllerMapper)
	    {
            _routeResolverTrie = routeResolverTrie;
            _controllerMapper = controllerMapper;
	    }

        public async Task<IResolveResult> Resolve(RouteContext context, RequestCulture requestCulture)
        {
            var trie = await _routeResolverTrie.LoadTrieAsync(requestCulture);

            if (trie == null || !trie.Any()) return null;

            // Set the default action to index
            var action = DefaultRouter.DefaultAction;

            PathString remaining = context.HttpContext.Request.Path;

            if (context.HttpContext.Request.Path.Value.StartsWith("/" + requestCulture.Culture.TwoLetterISOLanguageName))
            {
                remaining = remaining.Value.Substring(3);
            }

            var segments = remaining.Value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            TrieNode currentNode;

            // The requested url is for the start page with no action
            // so just return the start page
            if (!segments.Any())
            {
                trie.TryGetNode("/", out currentNode);
            }
            else
            {
                var requestPath = NormalizeRequestPath(remaining);

                // The normal behaviour is to load the page based on the incoming url
                trie.TryGetNode(requestPath, out currentNode);

                // Try to find the node without the last segment of the url and set the last segment as action
                if (currentNode == null)
                {
                    action = segments.Last();
                    requestPath = string.Join("/", segments, 0, (segments.Length - 1));
                    trie.TryGetNode("/" + requestPath, out currentNode);
                }
            }

            if (currentNode == null)
            {
                return null;
            }

            if(segments.Any()) { 
                // We always need to check if the last segment is a valid action on the controller for the node we found
                string possibleAction = segments.Last();
                if (_controllerMapper.ControllerHasAction(currentNode.ControllerName, possibleAction))
                {
                    action = possibleAction;
                }
            }

            if (!_controllerMapper.ControllerHasAction(currentNode.ControllerName, action))
            {
                return null;
            }

            return new ResolveResult(currentNode, currentNode.ControllerName, action);
        }

        private string NormalizeRequestPath(string path)
        {
            return path.TrimEnd('/');
        }

		//private string ResolveControllerName(JsonDocumentMetadata metadata)
		//{
        //    var ravenClrType = metadata.Metadata.Value<string>(RavenClrType);
		//    var start = ravenClrType.LastIndexOf(".", StringComparison.Ordinal) + 1;
		//	var end = ravenClrType.IndexOf(",", StringComparison.Ordinal);
		//	return ravenClrType.Substring(start, end - start);
		//}
    }
}