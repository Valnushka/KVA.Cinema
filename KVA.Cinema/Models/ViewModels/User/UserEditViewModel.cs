using KVA.Cinema.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class UserEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Required field")]
        [StringLength(30, ErrorMessage = "Last name length cannot be more than 30 symbols")]
        [MinLength(2, ErrorMessage = "Last name length cannot be less than 2 symbols")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Required field")]
        [StringLength(30, ErrorMessage = "First name length cannot be more than 30 symbols")]
        [MinLength(2, ErrorMessage = "First name length cannot be less than 2 symbols")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Required field")]
        [StringLength(20, ErrorMessage = "Nickname length cannot be more than 20 symbols")]
        [MinLength(3, ErrorMessage = "Nickname length cannot be less than 3 symbols")]
        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.Date)]
        [ValidAge(14, 120, ErrorMessage = "Age must be in 14-120")]
        [Display(Name = "Birth date")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Required field")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Incorrect email format")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
