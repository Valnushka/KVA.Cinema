using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Models.Director;
using KVA.Cinema.Models.Entities;
using KVA.Cinema.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class DirectorService : IService<DirectorCreateViewModel, DirectorDisplayViewModel, DirectorEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for Name
        /// </summary>
        private const int NAME_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for Name
        /// </summary>
        private const int NAME_LENGHT_MAX = 128;

        public CinemaContext Context { get; }

        public DirectorService(CinemaContext db)
        {
            Context = db;
        }

        public IEnumerable<DirectorCreateViewModel> Read()
        {
            throw new NotImplementedException();
        }

        public DirectorDisplayViewModel Read(Guid directorId)
        {
            var director = Context.Directors.FirstOrDefault(x => x.Id == directorId);

            if (director == default)
            {
                throw new EntityNotFoundException($"Director with id \"{directorId}\" not found");
            }

            return MapToDisplayViewModel(director);
        }

        public IEnumerable<DirectorDisplayViewModel> ReadAll()
        {
            List<Director> directors = Context.Directors.ToList();

            return directors.Select(x => new DirectorDisplayViewModel()
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        public void CreateAsync(DirectorCreateViewModel directorData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(directorData.Name))
            {
                throw new ArgumentNullException("Name has no value");
            }

            if (directorData.Name.Length < NAME_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {NAME_LENGHT_MIN} symbols");
            }

            if (directorData.Name.Length > NAME_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {NAME_LENGHT_MAX} symbols");
            }

            if (Context.Directors.FirstOrDefault(x => x.Name == directorData.Name) != default)
            {
                throw new DuplicatedEntityException($"Director with name \"{directorData.Name}\" is already exist");
            }

            Director newDirector = new Director()
            {
                Id = Guid.NewGuid(),
                Name = directorData.Name
            };

            Context.Directors.Add(newDirector);
            Context.SaveChanges();
        }

        public void Delete(Guid directorId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(directorId))
            {
                throw new ArgumentNullException("Director Id has no value");
            }

            Director director = Context.Directors.FirstOrDefault(x => x.Id == directorId);

            if (director == default)
            {
                throw new EntityNotFoundException($"Director with Id \"{directorId}\" not found");
            }

            Context.Directors.Remove(director);
            Context.SaveChanges();
        }

        public void Update(Guid directorId, DirectorEditViewModel directorNewData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(directorId, directorNewData.Name))
            {
                throw new ArgumentNullException("Director name or id has no value");
            }

            Director director = Context.Directors.FirstOrDefault(x => x.Id == directorId);

            if (director == default)
            {
                throw new EntityNotFoundException($"Director with id \"{directorId}\" not found");
            }

            if (directorNewData.Name.Length < NAME_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {NAME_LENGHT_MIN} symbols");
            }

            if (directorNewData.Name.Length > NAME_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {NAME_LENGHT_MAX} symbols");
            }

            if (Context.Directors.FirstOrDefault(x => x.Name == directorNewData.Name && x.Id != directorNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"Director with name \"{directorNewData.Name}\" is already exist");
            }

            director.Name = directorNewData.Name;

            Context.SaveChanges();
        }

        public bool IsEntityExist(Guid directorId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(directorId))
            {
                return false;
            }

            Director director = Context.Directors.FirstOrDefault(x => x.Id == directorId);

            return director != default;
        }

        private DirectorDisplayViewModel MapToDisplayViewModel(Director director)
        {
            return new DirectorDisplayViewModel()
            {
                Id = director.Id,
                Name = director.Name
            };
        }
    }
}
