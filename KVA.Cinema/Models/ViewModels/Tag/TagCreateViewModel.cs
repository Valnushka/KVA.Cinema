using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.Models.ViewModels.Tag
{
    public class TagCreateViewModel
    {
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
