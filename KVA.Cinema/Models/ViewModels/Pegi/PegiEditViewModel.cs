using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class PegiEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Range(0, 99, ErrorMessage = "Value is not valid for age restriction")]
        [Display(Name = "Age restriction")]
        public byte Type { get; set; }
    }
}
