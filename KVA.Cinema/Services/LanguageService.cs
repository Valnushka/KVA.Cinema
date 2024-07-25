using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using System;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class LanguageService : BaseService<Language, LanguageCreateViewModel, LanguageDisplayViewModel, LanguageEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for Name
        /// </summary>
        private const int NAME_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for Name
        /// </summary>
        private const int NAME_LENGHT_MAX = 128;

        public LanguageService(CinemaContext context) : base(context) { }

        protected override void ValidateInput(LanguageCreateViewModel languageData)
        {
            if (string.IsNullOrWhiteSpace(languageData.Name))
            {
                throw new ArgumentException("No value", nameof(languageData.Name));
            }
        }

        protected override void ValidateInput(LanguageEditViewModel languageNewData)
        {
            if (string.IsNullOrWhiteSpace(languageNewData.Name))
            {
                throw new ArgumentException("No value", nameof(languageNewData.Name));
            }
        }

        protected override void ValidateEntity(LanguageCreateViewModel languageData)
        {
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
        }

        protected override void ValidateEntity(LanguageEditViewModel languageNewData)
        {
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
        }

        protected override Language MapToEntity(LanguageCreateViewModel languageData)
        {
            return new Language()
            {
                Id = Guid.NewGuid(),
                Name = languageData.Name
            };
        }

        protected override LanguageDisplayViewModel MapToDisplayViewModel(Language language)
        {
            return new LanguageDisplayViewModel()
            {
                Id = language.Id,
                Name = language.Name
            };
        }

        protected override void UpdateFieldValues(Language language, LanguageEditViewModel languageNewData)
        {
            language.Name = languageNewData.Name;
        }
    }
}
