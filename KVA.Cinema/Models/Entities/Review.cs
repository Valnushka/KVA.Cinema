using System;

namespace KVA.Cinema.Entities
{
    public class Review
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public Guid VideoId { get; set; }

        //public Guid VideoRateId { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public virtual Video Video { get; set; }

        //public virtual VideoRate VideoRate { get; set; }
    }
}
