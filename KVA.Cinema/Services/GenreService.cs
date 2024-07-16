using KVA.Cinema.Exceptions;
using KVA.Cinema.Entities;
using KVA.Cinema.Utilities;
using KVA.Cinema.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using KVA.Cinema.Models;

namespace KVA.Cinema.Services
{
    public class GenreService : IService<GenreCreateViewModel, GenreDisplayViewModel, GenreEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for Title
        /// </summary>
        private const int TITLE_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for Title
        /// </summary>
        private const int TITLE_LENGHT_MAX = 50;

        public CinemaContext Context { get; }

        public GenreService(CinemaContext db)
        {
            Context = db;
        }
        public IEnumerable<GenreCreateViewModel> Read()
        {
            return Context.Genres.Select(x => new GenreCreateViewModel()
            {
                Id = x.Id,
                Title = x.Title
            }).ToList();
        }

        public GenreDisplayViewModel Read(Guid genreId)
        {
            var genre = Context.Genres.FirstOrDefault(x => x.Id == genreId);

            if (genre == default)
            {
                throw new EntityNotFoundException($"Genre with id \"{genreId}\" not found");
            }

            return MapToDisplayViewModel(genre);
        }

        public IEnumerable<GenreDisplayViewModel> ReadAll()
        {
            List<Genre> genres = Context.Genres.ToList();

            return genres.Select(x => new GenreDisplayViewModel()
            {
                Id = x.Id,
                Title = x.Title
            });
        }

        public void CreateAsync(GenreCreateViewModel genreData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(genreData.Title))
            {
                throw new ArgumentNullException("Title has no value");
            }

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

            Genre newGenre = new Genre()
            {
                Id = Guid.NewGuid(),
                Title = genreData.Title
            };

            Context.Genres.Add(newGenre);
            Context.SaveChanges();
        }

        public void Delete(Guid genreId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(genreId))
            {
                throw new ArgumentNullException("Genre Id has no value");
            }

            Genre genre = Context.Genres.FirstOrDefault(x => x.Id == genreId);

            if (genre == default)
            {
                throw new EntityNotFoundException($"Genre with Id \"{genreId}\" not found");
            }

            Context.Genres.Remove(genre);
            Context.SaveChanges();
        }

        public void Update(Guid genreId, GenreEditViewModel genreNewData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(genreId, genreNewData.Title))
            {
                throw new ArgumentNullException("Genre title or id has no value");
            }

            Genre genre = Context.Genres.FirstOrDefault(x => x.Id == genreId);

            if (genre == default)
            {
                throw new EntityNotFoundException($"Genre with id \"{genreId}\" not found");
            }

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

            genre.Title = genreNewData.Title;

            Context.SaveChanges();
        }

        private GenreDisplayViewModel MapToDisplayViewModel(Genre genre)
        {
            return new GenreDisplayViewModel()
            {
                Id = genre.Id,
                Title = genre.Title
            };
        }
    }
}
