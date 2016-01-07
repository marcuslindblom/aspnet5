namespace src.Models
{
    [Model]
    public class Home
    {
        public string Heading { get; set; }
    }

    [Model]
    public class About
    {
        public string Heading { get; set; }
    }

    public class ViewModel
    {
        public Page CurrentPage { get; set; }
    }
}
