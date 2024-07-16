using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class CountryDisplayViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
