using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.Models.ViewModels.Subscription
{
    public class SubscriptionCreateViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Required field")]
        [StringLength(50, ErrorMessage = "Title length cannot be more than 50 symbols")]
        [MinLength(2, ErrorMessage = "Title length cannot be less than 2 symbols")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Required field")]
        [StringLength(150, ErrorMessage = "Description length cannot be more than 50 symbols")]
        [MinLength(2, ErrorMessage = "Description length cannot be less than 2 symbols")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Range(0, 10_000, ErrorMessage = "Value is not valid for cost")]
        [DataType(DataType.Currency)]
        [Display(Name = "Cost ($)")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name = "Level")]
        public Guid LevelId { get; set; }

        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.Date)]
        [Display(Name = "Released in")]
        public DateTime ReleasedIn { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Range(1, 366, ErrorMessage = "Value is not valid for duration")]
        [Display(Name = "Duration (days)")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.Date)]
        [Display(Name = "Available to buy until")]
        public DateTime AvailableUntil { get; set; }

        [Display(Name = "Videos")]
        public IEnumerable<Guid> VideoIds { get; set; }
    }
}
