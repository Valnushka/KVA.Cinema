using KVA.Cinema.Models.Entities;
using System;
using System.Collections.Generic;

namespace KVA.Cinema.Entities
{
    /// <summary>
    /// Language of video and its subtitles
    /// </summary>
    public class Language : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Subtitle> Subtitles { get; set; }

        public virtual ICollection<Video> Videos { get; set; }

        public Language()
        {
            Subtitles = new HashSet<Subtitle>();
            Videos = new HashSet<Video>();
        }
    }
}
