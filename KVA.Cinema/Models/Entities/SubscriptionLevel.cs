using System;
using System.Collections.Generic;

namespace KVA.Cinema.Models.Entities
{
    public class SubscriptionLevel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }

        public SubscriptionLevel()
        {
            Subscriptions = new HashSet<Subscription>();
        }
    }
}
