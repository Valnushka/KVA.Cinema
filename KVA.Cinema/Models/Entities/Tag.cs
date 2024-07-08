using System;
using System.Collections.Generic;

namespace KVA.Cinema.Models.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public string Color { get; set; }

        public virtual ICollection<Video> Videos { get; set; }

        public Tag()
        {
            Videos = new HashSet<Video>();
        }
    }
}
