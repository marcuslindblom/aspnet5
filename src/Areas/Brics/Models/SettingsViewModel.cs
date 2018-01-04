// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

using Microsoft.AspNetCore.Mvc.Rendering;
using Raven.Client;

namespace src.Areas.Brics.Models
{
    public class SettingsViewModel
    {
        public Page CurrentPage { get; set; }
        public string Url { get; set; }
        public string Slug { get; set; }
        public string PublishedDate { get; set; }
        public SelectList Acl { get; set; }
    }
}
