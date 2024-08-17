using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class TagDisplayViewModel : IViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Text")]
        public string Text { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }
    }
}
