using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class SubscriptionLevelDisplayViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }
    }
}
