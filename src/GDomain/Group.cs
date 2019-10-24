using System.Collections.Generic;

namespace GDomain
{
    public class Group<T> where T : Meaning
    {
        public Group()
        {
            Meanings = new List<T>();
        }
        public string Type { get; set; }
        public string TypeNote { get; set; }
        public List<T> Meanings { get; set; }
    }
}