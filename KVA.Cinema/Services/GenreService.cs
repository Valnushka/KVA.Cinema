using KVA.Cinema.Exceptions;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using System;
using System.Linq;
using KVA.Cinema.Models;

namespace KVA.Cinema.Services
{
    public class GenreService : BaseService<Genre, GenreCreateViewModel, GenreDisplayViewModel, GenreEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for Title
        /// </summary>
        private const int TITLE_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for Title
        /// </summary>
        private const int TITLE_LENGHT_MAX = 50;

        public GenreService(CinemaContext context) : base(context) { }

        protected override GenreDisplayViewModel MapToDisplayViewModel(Genre genre)
        {
            return new GenreDisplayViewModel()
            {
                Id = genre.Id,
                Title = genre.Title
            };
        }

        protected override void ValidateInput(GenreCreateViewModel genreData)
        {
            if (string.IsNullOrWhiteSpace(genreData.Title))
            {
                throw new ArgumentException("No value", nameof(genreData.Title));
            }
        }

        protected override void ValidateInput(GenreEditViewModel genreNewData)
        {
            if (string.IsNullOrWhiteSpace(genreNewData.Title))
            {
                throw new ArgumentException("No value", nameof(genreNewData.Title));
            }
        }

        protected override void ValidateEntity(GenreCreateViewModel genreData)
        {
            if (genreData.Title.Length < TITLE_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {TITLE_LENGHT_MIN} symbols");
            }

            if (genreData.Title.Length > TITLE_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {TITLE_LENGHT_MAX} symbols");
            }

            if (Context.Genres.FirstOrDefault(x => x.Title == genreData.Title) != default)
            {
                throw new DuplicatedEntityException($"Genre with title \"{genreData.Title}\" is already exist");
            }
        }

        protected override void ValidateEntity(GenreEditViewModel genreNewData)
        {
            if (genreNewData.Title.Length < TITLE_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {TITLE_LENGHT_MIN} symbols");
            }

            if (genreNewData.Title.Length > TITLE_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {TITLE_LENGHT_MAX} symbols");
            }

            if (Context.Genres.FirstOrDefault(x => x.Title == genreNewData.Title && x.Id != genreNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"Genre with title \"{genreNewData.Title}\" is already exist");
            }
        }

        protected override Genre MapToEntity(GenreCreateViewModel genreData)
        {
            return new Genre()
            {
                Id = Guid.NewGuid(),
                Title = genreData.Title
            };
        }

        protected override void UpdateFieldValues(Genre genre, GenreEditViewModel genreNewData)
        {
            genre.Title = genreNewData.Title;
        }
    }
}
