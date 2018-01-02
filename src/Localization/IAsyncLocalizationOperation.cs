using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using src;

namespace src.Localization
{
    public interface IAsyncLocalizationOperation
    {
        string Key { get; set; }

        object Entity { get; set; }

        Page Page { get; set; }

        Task<Page> LoadAsync(string id, CancellationToken token = default(CancellationToken));

        Task<List<Page>> LoadAsync(IEnumerable<string> ids, CancellationToken token = default(CancellationToken));

        Task StoreAsync(Page page, CancellationToken token = default(CancellationToken));
    }
}