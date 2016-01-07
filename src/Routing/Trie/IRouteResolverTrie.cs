using System.Threading.Tasks;
using Microsoft.AspNet.Localization;

namespace src.Routing.Trie
{
    public interface IRouteResolverTrie
    {
        Task<Trie> LoadTrieAsync(RequestCulture requestCulture);
    }
}