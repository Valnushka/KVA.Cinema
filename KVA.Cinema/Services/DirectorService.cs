using System;
using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class DirectorService : BaseService<Director, DirectorCreateViewModel, DirectorDisplayViewModel, DirectorEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for Name
        /// </summary>
        private const int NAME_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for Name
        /// </summary>
        private const int NAME_LENGHT_MAX = 128;

        public DirectorService(CinemaContext context) : base(context) { }

        protected override DirectorDisplayViewModel MapToDisplayViewModel(Director director)
        {
            return new DirectorDisplayViewModel()
            {
                Id = director.Id,
                Name = director.Name
            };
        }

        protected override void ValidateInput(DirectorCreateViewModel directorData)
        {
            if (string.IsNullOrWhiteSpace(directorData.Name))
            {
                throw new ArgumentException("No value", nameof(directorData.Name));
            }
        }

        protected override void ValidateInput(DirectorEditViewModel directorData)
        {
            if (string.IsNullOrWhiteSpace(directorData.Name))
            {
                throw new ArgumentException("No value", nameof(directorData.Name));
            }
        }

        protected override void ValidateEntity(DirectorCreateViewModel directorData)
        {
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
        }

        protected override void ValidateEntity(DirectorEditViewModel directorNewData)
        {
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
        }

        protected override Director MapToEntity(DirectorCreateViewModel directorData)
        {
            return new Director()
            {
                Id = Guid.NewGuid(),
                Name = directorData.Name
            };
        }

        protected override void UpdateFieldValues(Director director, DirectorEditViewModel directorNewData)
        {
            director.Name = directorNewData.Name;
        }
    }
}
