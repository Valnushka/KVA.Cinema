using KVA.Cinema.Models.Entities;
using System;
using System.Collections.Generic;

namespace KVA.Cinema.Entities
{
    public class Video : IEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Length { get; set; }

        public Guid CountryId { get; set; }

        public DateTime ReleasedIn { get; set; }

        public int Views { get; set; }

        public string Preview { get; set; }

        public Guid PegiId { get; set; }

        public Guid LanguageId { get; set; }

        public Guid DirectorId { get; set; }

        //public Guid? NextPartId { get; set; }

        //public Guid? PreviousPartId { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual Country Country { get; set; }

        public virtual Director Director { get; set; }

        public virtual ICollection<Frame> Frames { get; set; }

        public virtual Language Language { get; set; }

        public virtual Pegi Pegi { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<Subtitle> Subtitles { get; set; }

        //public virtual ICollection<Video> Video1 { get; set; } //следующие видео

        //public virtual Video NextPart { get; set; }

        //public virtual Video PreviousPart { get; set; }

        public virtual ICollection<Genre> Genres { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<VideoInSubscription> VideoInSubscriptions { get; set; }

        public virtual ICollection<VideoRate> VideoRates { get; set; }

        public Video()
        {
            Comments = new HashSet<Comment>();
            Frames = new HashSet<Frame>();
            Reviews = new HashSet<Review>();
            Subtitles = new HashSet<Subtitle>();
            //Video1 = new HashSet<Video>();
            Genres = new HashSet<Genre>();
            Tags = new HashSet<Tag>();
            VideoInSubscriptions = new HashSet<VideoInSubscription>();
            VideoRates = new HashSet<VideoRate>();
        }
    }
}
