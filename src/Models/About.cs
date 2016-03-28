
using System.ComponentModel.DataAnnotations;

namespace src.Models
{
    [ViewModel]
    public class About
    {
        public About()
        {
            Link = new Link();
        }
        public string Heading { get; set; }

        [DataType(DataType.MultilineText), UIHint("LineBreak")]
        public string Message { get; set; }

        public Link Link { get; set; }
    }

    public class Link
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}