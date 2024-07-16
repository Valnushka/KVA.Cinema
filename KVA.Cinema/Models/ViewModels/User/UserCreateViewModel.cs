using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KVA.Cinema.Attributes;

namespace KVA.Cinema.ViewModels
{
    /// <summary>
    /// Essential data to create person (written by user)
    /// </summary>
    public class UserCreateViewModel
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

        [Required]
        [StringLength(120, ErrorMessage = "Password length cannot be more than 120 symbols")]
        [MinLength(8, ErrorMessage = "Password length cannot be less than 8 symbols")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords are not equal")]
        [DataType(DataType.Password)]
        [Display(Name = "Repeat password")]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Subscriptions")]
        public IEnumerable<Entities.Subscription> Subscriptions { get; set; }
    }
}
