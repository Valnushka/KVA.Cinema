using KVA.Cinema.Models.Entities;
using System;
using System.Collections.Generic;

namespace KVA.Cinema.Entities
{
    public class Genre : IEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public virtual ICollection<Video> Videos { get; set; }

        public Genre()
        {
            Videos = new HashSet<Video>();
        }
    }
}
