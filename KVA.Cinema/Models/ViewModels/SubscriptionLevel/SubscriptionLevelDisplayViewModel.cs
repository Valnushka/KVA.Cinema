using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.Models.ViewModels.SubscriptionLevel
{
    public class SubscriptionLevelDisplayViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }
    }
}
