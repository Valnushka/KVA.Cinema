using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KVA.Cinema.ViewModels
{
    public class VideoCreateViewModel : IViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Required field")]
        [StringLength(128, ErrorMessage = "Title length cannot be more than 128 symbols")]
        [Display(Name = "Title")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [StringLength(600, ErrorMessage = "Description length cannot be more than 600 symbols")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Length in minutes")]
        public int Length { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }

        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.Date)]
        [Display(Name = "Released in")]
        public DateTime ReleasedIn { get; set; }

        [Display(Name = "Views")]
        public int Views { get; set; }

        [Display(Name = "Poster (max 25 MB)")]
        public IFormFile Preview { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name = "PEGI")]
        public Guid PegiId { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name = "Language")]
        public Guid LanguageId { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name = "Director")]
        public Guid DirectorId { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name = "Genres (at least one)")]
        public IEnumerable<Guid> GenreIds { get; set; }

        [Display(Name = "Tags")]
        public IEnumerable<Guid> TagIds { get; set; }

        [Display(Name = "Tags")]
        public IEnumerable<TagDisplayViewModel> TagsViewModels { get; set; }
    }
}
