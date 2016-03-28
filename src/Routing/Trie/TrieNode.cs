using System;

namespace src.Routing.Trie
{
    public class TrieNode
    {
        public TrieNode(string pageId, string controllerName)
        {
            if (string.IsNullOrEmpty(pageId))
            {
                throw new ArgumentNullException(nameof(pageId));
            }

            if (string.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentNullException(nameof(controllerName));
            }
            PageId = pageId;
            ControllerName = controllerName;
        }

        public string PageId { get; set; }

        public string ControllerName { get; set; }
    }
}