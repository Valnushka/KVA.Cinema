using KVA.Cinema.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KVA.Cinema.ViewModels
{
    /// <summary>
    /// Person's data to display
    /// </summary>
    public class UserDisplayViewModel : IViewModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth date")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        public IEnumerable<Guid> SubscriptionIds { get; set; }

        [Display(Name = "Subscriptions")]
        public IEnumerable<Subscription> Subscriptions { get; set; }

        public ICollection<UserSubscription> UserSubscriptions { get; set; }

        public IEnumerable<string> SubscriptionNamesAndDates =>
            Subscriptions.Count() == 0
                 ? Enumerable.Empty<string>()
                 : UserSubscriptions.Select(x => $"{x.Subscription.Title} ({x.LastUntil.ToString("dd.MM.yyyy")})").ToList();

        [Display(Name = "Subscriptions and their expiration dates")]
        public string SubscriptionNamesAndDatesInOneString =>
            Subscriptions.Count() == 0
                ? string.Empty
                : string.Join(", ", SubscriptionNamesAndDates);
    }
}
