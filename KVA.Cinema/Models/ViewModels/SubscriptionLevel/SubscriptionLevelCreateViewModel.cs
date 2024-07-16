using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class SubscriptionLevelCreateViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Required field")]
        [StringLength(50, ErrorMessage = "Title length cannot be more than 50 symbols")]
        [MinLength(2, ErrorMessage = "Title length cannot be less than 2 symbols")]
        [Display(Name = "Title")]
        public string Title { get; set; }
    }
}
