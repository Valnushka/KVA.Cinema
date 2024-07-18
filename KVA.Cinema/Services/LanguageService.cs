using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using KVA.Cinema.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class LanguageService : IService<LanguageCreateViewModel, LanguageDisplayViewModel, LanguageEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for Name
        /// </summary>
        private const int NAME_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for Name
        /// </summary>
        private const int NAME_LENGHT_MAX = 128;

        private CinemaContext Context { get; }

        public LanguageService(CinemaContext db)
        {
            Context = db;
        }

        public LanguageDisplayViewModel Read(Guid languageId)
        {
            var language = Context.Languages.FirstOrDefault(x => x.Id == languageId);

            if (language == default)
            {
                throw new EntityNotFoundException($"Language with id \"{languageId}\" not found");
            }

            return MapToDisplayViewModel(language);
        }

        public IEnumerable<LanguageDisplayViewModel> ReadAll()
        {
            return Context.Languages.Select(x => new LanguageDisplayViewModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }

        public void Create(LanguageCreateViewModel languageData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(languageData.Name))
            {
                throw new ArgumentNullException("Name has no value");
            }

            if (languageData.Name.Length < NAME_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {NAME_LENGHT_MIN} symbols");
            }

            if (languageData.Name.Length > NAME_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {NAME_LENGHT_MAX} symbols");
            }

            if (Context.Languages.FirstOrDefault(x => x.Name == languageData.Name) != default)
            {
                throw new DuplicatedEntityException($"Language \"{languageData.Name}\" is already exist");
            }

            Language newLanguage = new Language()
            {
                Id = Guid.NewGuid(),
                Name = languageData.Name
            };

            Context.Languages.Add(newLanguage);
            Context.SaveChanges();
        }

        public void Delete(Guid languageId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(languageId))
            {
                throw new ArgumentNullException("Language Id has no value");
            }

            Language language = Context.Languages.FirstOrDefault(x => x.Id == languageId);

            if (language == default)
            {
                throw new EntityNotFoundException($"Language with Id \"{languageId}\" not found");
            }

            Context.Languages.Remove(language);
            Context.SaveChanges();
        }

        public void Update(Guid languageId, LanguageEditViewModel languageNewData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(languageId, languageNewData.Name))
            {
                throw new ArgumentNullException("Language name or id has no value");
            }

            Language language = Context.Languages.FirstOrDefault(x => x.Id == languageId);

            if (language == default)
            {
                throw new EntityNotFoundException($"Language with id \"{languageId}\" not found");
            }

            if (languageNewData.Name.Length < NAME_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {NAME_LENGHT_MIN} symbols");
            }

            if (languageNewData.Name.Length > NAME_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {NAME_LENGHT_MAX} symbols");
            }

            if (Context.Languages.FirstOrDefault(x => x.Name == languageNewData.Name && x.Id != languageNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"Language \"{languageNewData.Name}\" is already exist");
            }

            language.Name = languageNewData.Name;

            Context.SaveChanges();
        }

        private LanguageDisplayViewModel MapToDisplayViewModel(Language language)
        {
            return new LanguageDisplayViewModel()
            {
                Id = language.Id,
                Name = language.Name
            };
        }
    }
}
