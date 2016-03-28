using System;
using src.Routing.Trie;

namespace src.Routing
{
    public interface IResolveResult
    {
        TrieNode TrieNode { get; set; }
        string Controller { get; set; }
        string Action { get; set; }
    }

    public class ResolveResult : IResolveResult
    {
        public TrieNode TrieNode { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public ResolveResult(TrieNode trieNode, string controller, string action)
        {
            if (trieNode == null)
            {
                throw new ArgumentNullException(nameof(trieNode));
            }

            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            TrieNode = trieNode;
            Controller = controller;
            Action = action;
        }
    }
}