using System;

namespace src.Routing.Trie
{
    public class TrieNode
    {
        public TrieNode(string pageId, string name, string controllerName)
        {
            if (string.IsNullOrEmpty(pageId))
            {
                throw new ArgumentNullException(nameof(pageId));
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentNullException(nameof(controllerName));
            }
            PageId = pageId;
            Name = name;
            ControllerName = controllerName;
        }

        public string Name { get; set; }

        public string PageId { get; set; }

        public string ControllerName { get; set; }
    }
}