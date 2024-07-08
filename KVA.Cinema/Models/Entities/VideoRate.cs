using System;

namespace KVA.Cinema.Models.Entities
{
    public class VideoRate
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Rate from 1 (lowest) to 10
        /// </summary>
        public byte Rate { get; set; }

        public Guid VideoId { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public virtual Video Video { get; set; }
    }
}
