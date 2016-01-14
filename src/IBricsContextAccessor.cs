using src.Routing.Trie;

namespace src
{
    public interface IBricsContextAccessor
    {
        TrieNode CurrentNode { get; }

        Page CurrentPage { get; }
    }
}