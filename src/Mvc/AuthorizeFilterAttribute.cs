using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Raven.Abstractions.Data;
using Raven.Client;
using src.Routing;
using src.Routing.Trie;

namespace src.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeFilterAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(AuthorizationContext context)
        {
            if(!HasAllowAnonymous(context)) {                
                object value;
                if(context.HttpContext.Items.TryGetValue(DefaultRouter.CurrentNodeKey, out value))
                {
                    IDocumentStore documentStore = context.HttpContext.RequestServices.GetService(typeof(IDocumentStore)) as IDocumentStore;
                    if (documentStore == null) return;
                    var node = value as TrieNode;

                    if(node == null) return;
                    
                    JsonDocumentMetadata documentMetadata = documentStore.DatabaseCommands.Head(node.PageId);
                    if (documentMetadata == null) return;
                    var accessControl = documentMetadata.Metadata.Value<string>("Brics-AccessControl");

                    if (string.IsNullOrEmpty(accessControl))
                    {
                        context.Result = new HttpNotFoundResult();
                    }
                    if (accessControl == AccessControl.Anonymous)
                    {
                        return;
                    }
                    if (accessControl == AccessControl.Authenticated && !context.HttpContext.User.Identity.IsAuthenticated)
                    {
                        context.Result = new HttpNotFoundResult();
                    }
                    if (accessControl == AccessControl.Administrators && !context.HttpContext.User.IsInRole(accessControl))
                    {
                        context.Result = new HttpNotFoundResult();
                    }
                }
            }
        }
    }

    public struct AccessControl {
        public const string Administrators = "Administrators";
        public const string Anonymous = "Anonymous";
        public const string Authenticated = "Authenticated";
    }
}
