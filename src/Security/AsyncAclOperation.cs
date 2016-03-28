using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client;
using src.Localization;
using src.Mvc;
using src.Routing.Trie;

namespace src.Security
{
    public class AsyncAclOperation : IAsyncAclOperation
    {
        private readonly IAsyncDocumentSession _session;
        private readonly AccessControl _acl;

        public AsyncAclOperation(IAsyncDocumentSession session, AccessControl acl)
        {
            _session = session;
            _acl = acl;
        }

        public Task StoreAsync(Page page, CancellationToken token = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }

    public interface IAsyncAclOperation
    {
        Task StoreAsync(Page page, CancellationToken token = default(CancellationToken));
    }
}