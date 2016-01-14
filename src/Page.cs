using System;
using System.ComponentModel.DataAnnotations;

namespace src
{
    /// <summary>
    /// Summary description for Home
    /// </summary>
    public sealed class Page
    {
        //[ScaffoldColumn(false)]
        public string Id { get; set; }

        public DateTime? Changed { get; set; }

        public Metadata Metadata { get; set; }

        public string Name { get; set; } // < Maybe a bad idea because of the navigtion

        public Page()
        {
            Metadata = new Metadata();
        }
    }
    public class Metadata
    {
        [Display(Name = "Meta Title", Description = "Recommended: 70 characters.")]
        public string MetaTitle { get; set; }

        [Display(Name = "Meta Description", Description = "Recommended: 156 characters.")]
        [DataType(DataType.MultilineText)]
        public string MetaDescription { get; set; }
    }
}