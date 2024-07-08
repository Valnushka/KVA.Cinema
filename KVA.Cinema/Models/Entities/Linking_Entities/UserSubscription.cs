using System;

namespace KVA.Cinema.Models.Entities
{
    public class UserSubscription
    {
        public Guid Id { get; set; }

        public Guid SubscriptionId { get; set; }

        public Guid UserId { get; set; }

        public DateTime ActivatedOn { get; set; }

        public DateTime LastUntil { get; set; }

        public virtual Subscription Subscription { get; set; }

        public virtual User User { get; set; }
    }
}
