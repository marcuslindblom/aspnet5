using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace src.Localization
{
    public interface IAsyncLocalizationOperation
    {
        string Key { get; set; }

        object Entity { get; set; }

        Page Page { get; set; }

        Task<T> LoadAsync<T>(string id, CancellationToken token = default(CancellationToken));

        Task<T[]> LoadAsync<T>(IEnumerable<string> ids, CancellationToken token = default(CancellationToken));

        Task StoreAsync(Page page, CancellationToken token = default(CancellationToken));
    }
}