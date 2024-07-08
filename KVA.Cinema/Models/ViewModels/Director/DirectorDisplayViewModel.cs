using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.Models.Director
{
    public class DirectorDisplayViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
