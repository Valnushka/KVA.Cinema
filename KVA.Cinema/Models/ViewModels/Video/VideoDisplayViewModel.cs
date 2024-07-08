using KVA.Cinema.Models.ViewModels.Tag;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KVA.Cinema.Models.ViewModels.Video
{
    public class VideoDisplayViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Title")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Length")]
        public int Length { get; set; }

        public Guid CountryId { get; set; }

        [Display(Name = "Country")]
        public string CountryName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Released in")]
        public DateTime ReleasedIn { get; set; }

        [Display(Name = "Views")]
        public int Views { get; set; }

        [Display(Name = "Preview (poster)")]
        public IFormFile Preview { get; set; }

        public string PreviewFileName { get; set; }

        public Guid PegiId { get; set; }

        [Display(Name = "PEGI")]
        public string PegiName { get; set; }

        public Guid LanguageId { get; set; }

        [Display(Name = "Language")]
        public string LanguageName { get; set; }

        public Guid DirectorId { get; set; }

        [Display(Name = "Director")]
        public string DirectorName { get; set; }

        [Display(Name = "Genres")]
        public string GenreNames => string.Join(", ", Genres.Select(x => x.Title));

        [Display(Name = "Genres")]
        public IEnumerable<Entities.Genre> Genres { get; set; }

        [Display(Name = "Tags")]
        public string TagNames => string.Join(", ", Tags.Select(x => x.Text));

        [Display(Name = "Tags")]
        public IEnumerable<Entities.Tag> Tags { get; set; }

        [Display(Name = "Tags")]
        public IEnumerable<TagDisplayViewModel> TagViewModels { get; set; }
    }
}
