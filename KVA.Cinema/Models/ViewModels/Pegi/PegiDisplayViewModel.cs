using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.Models.ViewModels.Pegi
{
    public class PegiDisplayViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Age restriction")]
        public byte Type { get; set; }
    }
}
