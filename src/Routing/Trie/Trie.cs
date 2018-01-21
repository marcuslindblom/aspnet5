using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace src.Routing.Trie
{
    public class Trie : IDictionary<string, TrieNode>
    {
        private int _count;

        private readonly Dictionary<string, TrieNode> nodes;

        public string Id { get; set; }

        public Trie()
        {
            Id = "brickpile/trie";
            nodes = new Dictionary<string, TrieNode>(StringComparer.Ordinal);
        }

        public TrieNode this[string key]
        {
            get
            {
                TrieNode value;

                if (!nodes.TryGetValue(key, out value))
                {
                    throw new KeyNotFoundException("The given charKey was not present in the trie.");
                }

                return value;
            }

            set
            {
                TrieNode node;

                if (!nodes.TryGetValue(key, out node))
                {
                    Add(key, value);
                }
            }
        }

        public int Count => _count;

        public bool IsReadOnly => false;

        public ICollection<string> Keys
        {
            get
            {
                return nodes.Select(x => x.Key).ToArray();
            }
        }

        public ICollection<TrieNode> Values
        {
            get
            {
                return nodes.Select(x => x.Value).ToArray();
            }
        }

        public void Add(KeyValuePair<string, TrieNode> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(string key, TrieNode value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (nodes.ContainsKey(key))
            {
                throw new ArgumentException(string.Format("An element with the same key already exists: '{0}'", key), "key");

            }

            nodes.Add(key, value);

            _count++;
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public bool Contains(KeyValuePair<string, TrieNode> item)
        {
            return nodes.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return nodes.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, TrieNode>[] array, int arrayIndex)
        {
            Array.Copy(nodes.Select(n => new KeyValuePair<string, TrieNode>(n.Key, n.Value)).ToArray(), 0, array, arrayIndex, Count);
        }

        public IEnumerator<KeyValuePair<string, TrieNode>> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, TrieNode> item)
        {
            return nodes.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            return nodes.Remove(key);
        }

        public bool TryGetValue(string key, out TrieNode value)
        {
            return nodes.TryGetValue(key, out value);
        }

        public bool TryGetNode(string key, out TrieNode node)
        {
            return nodes.TryGetValue(key, out node);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        public IEnumerable<KeyValuePair<string, TrieNode>> ChildrenOf(TrieNode parent, bool includeRoot = false)
        {
            return ChildrenOf(parent.PageId, includeRoot);
        }

        public IEnumerable<KeyValuePair<string, TrieNode>> ChildrenOf(string key, bool includeRoot = false)
        {
            var matches = nodes.Keys.Select(x => Regex.Match(x, "^/([^/]*)[^/]$"));
            foreach (var match in matches)
            {
                if (match.Success)
                {
                    yield return nodes.Where(x => x.Key == match.Value).FirstOrDefault();
                }
            }
            //return includeRoot ? nodes.Where(x => x.Key.StartsWith(key)) : nodes.Where(x => x.Key != key && x.Key.StartsWith(key));
        }

        public IEnumerable<KeyValuePair<string, TrieNode>> AncestorsOf(string key)
        {
            var ancestors = new Dictionary<string, TrieNode>();
            AncestorsOf(key, ancestors);
            return ancestors.Reverse();
        }

        private void AncestorsOf(string key, Dictionary<string, TrieNode> ancestors)
        {
            var trieNode = nodes.First(x => x.Key == key);

            while (trieNode.Key != null && trieNode.Key != "/")
            {
                var segments = trieNode.Key.Split(new[] { '/' });

                trieNode = nodes.FirstOrDefault(x => x.Key.Trim(new[] { '/' }) == segments.Take(segments.Length -1).LastOrDefault());

                if (trieNode.Value != null && !ancestors.Contains(trieNode))
                {
                    ancestors.Add(trieNode.Key, trieNode.Value);
                }
            }
        }

        public Dictionary<string, TrieNode> BuildTrie(string key)
        {
            var ancestors = AncestorsOf(key);
            var result = new Dictionary<string, TrieNode>();
            foreach (var ancestor in ancestors)
            {
                if (!result.Contains(ancestor))
                    result.Add(ancestor.Key, ancestor.Value);

                foreach (var child in ChildrenOf(ancestor.Key))
                {
                    if (!result.Contains(child))
                        result.Add(child.Key, child.Value);
                }
            }
            return result;
        }
    }
}