using System;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.Models.Genre
{
    public class GenreDisplayViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }
    }
}
