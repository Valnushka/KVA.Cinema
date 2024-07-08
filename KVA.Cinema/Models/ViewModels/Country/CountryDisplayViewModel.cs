using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.Models.Country
{
    public class CountryDisplayViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
