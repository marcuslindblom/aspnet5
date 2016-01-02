using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using src.Routing.Trie;

namespace src
{
    public class Site
    {
        public Site(string name, CultureInfo culture, bool isDefaultCulture = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            Name = name;

            Culture = culture;

            IsDefaultCulture = isDefaultCulture;

            Trie = new Trie();
        }
        public string Id { get; set; }

        public bool IsDefaultCulture { get; set; }

        public string Name { get; set; }

        public CultureInfo Culture { get; set; }

        public Trie Trie { get; set; }
    }
}
