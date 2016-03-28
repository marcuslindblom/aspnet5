using System;

namespace src
{
    public class ViewModelAttribute : Attribute {
        public int Order { get; set; }
    }
}