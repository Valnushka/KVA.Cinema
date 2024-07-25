using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using System;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class CountryService : BaseService<Country, CountryCreateViewModel, CountryDisplayViewModel, CountryEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for Name
        /// </summary>
        private const int NAME_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for Name
        /// </summary>
        private const int NAME_LENGHT_MAX = 128;

        public CountryService(CinemaContext context) : base(context) { }

        protected override CountryDisplayViewModel MapToDisplayViewModel(Country country)
        {
            return new CountryDisplayViewModel()
            {
                Id = country.Id,
                Name = country.Name
            };
        }

        protected override Country MapToEntity(CountryCreateViewModel countryData)
        {
            return new Country()
            {
                Id = Guid.NewGuid(),
                Name = countryData.Name
            };
        }

        protected override void UpdateFieldValues(Country country, CountryEditViewModel countryNewData)
        {
            country.Name = countryNewData.Name;
        }

        protected override void ValidateEntity(CountryCreateViewModel countryData)
        {
            ValidateName(countryData.Name);

            if (Context.Countries.FirstOrDefault(x => x.Name == countryData.Name) != default)
            {
                throw new DuplicatedEntityException($"Country with name \"{countryData.Name}\" is already exist");
            }
        }

        protected override void ValidateEntity(CountryEditViewModel countryNewData)
        {
            ValidateName(countryNewData.Name);

            if (Context.Countries.FirstOrDefault(x => x.Name == countryNewData.Name && x.Id != countryNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"Country with name \"{countryNewData.Name}\" is already exist");
            }
        }

        protected override void ValidateInput(CountryCreateViewModel countryData)
        {
            if (string.IsNullOrWhiteSpace(countryData.Name))
            {
                throw new ArgumentNullException(nameof(countryData.Name), "No value");
            }
        }

        protected override void ValidateInput(CountryEditViewModel countryNewData)
        {
            if (string.IsNullOrWhiteSpace(countryNewData.Name))
            {
                throw new ArgumentNullException(nameof(countryNewData.Name), "No value");
            }
        }

        private void ValidateName(string name)
        {
            if (name.Length < NAME_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {NAME_LENGHT_MIN} symbols");
            }

            if (name.Length > NAME_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {NAME_LENGHT_MAX} symbols");
            }
        }
    }
}
