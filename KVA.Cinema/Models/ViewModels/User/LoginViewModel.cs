using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
