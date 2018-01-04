using System;
using System.ComponentModel.DataAnnotations;
using src.Mvc;

namespace src
{
    /// <summary>
    /// Summary description for Home
    /// </summary>
    public sealed class Page
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        public DateTime? Changed { get; set; }

        public DateTime? PublishedDate { get; set; } 

        public Metadata Metadata { get; set; }

        public string Name { get; set; } // < Maybe a bad idea because of the navigtion

        public AccessControl? Acl { get; set; } // < This could be stored as meta data on the document instead

        public Page()
        {
            Metadata = new Metadata();
        }
    }
    public interface IPage {
        string Id { get; }
        string Name { get; }
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