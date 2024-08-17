using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class LanguageCreateViewModel : IViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Required field")]
        [StringLength(128, ErrorMessage = "Name length cannot be more than 128 symbols")]
        [MinLength(2, ErrorMessage = "Name length cannot be less than 2 symbols")]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
