using System;
using System.Collections.Generic;

namespace KVA.Cinema.Models.Entities
{
    public class Pegi
    {
        public Guid Id { get; set; }

        public byte Type { get; set; }

        public virtual ICollection<Video> Videos { get; set; }

        public Pegi()
        {
            Videos = new HashSet<Video>();
        }
    }
}
