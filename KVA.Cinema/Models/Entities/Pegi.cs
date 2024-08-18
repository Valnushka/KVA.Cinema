using KVA.Cinema.Models.Entities;
using System;
using System.Collections.Generic;

namespace KVA.Cinema.Entities
{
    public class Pegi : IEntity
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
