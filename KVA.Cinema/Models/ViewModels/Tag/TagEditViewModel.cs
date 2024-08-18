using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class TagEditViewModel : IViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Required field")]
        [StringLength(20, ErrorMessage = "Tag text length cannot be more than 20 symbols")]
        [MinLength(2, ErrorMessage = "Tag text length cannot be less than 2 symbols")]
        [Display(Name = "Text")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name = "Color")]
        public string Color { get; set; }
    }
}
