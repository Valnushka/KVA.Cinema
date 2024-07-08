using System;

namespace KVA.Cinema.Models.Entities
{
    public class VideoInSubscription
    {
        public Guid Id { get; set; }

        public Guid SubscriptionId { get; set; }

        public Guid VideoId { get; set; }

        public virtual Subscription Subscription { get; set; }

        public virtual Video Video { get; set; }
    }
}
