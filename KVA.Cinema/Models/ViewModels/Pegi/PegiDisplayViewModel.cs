using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class PegiDisplayViewModel : IViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Age restriction")]
        public byte Type { get; set; }
    }
}
