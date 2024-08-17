using System;
using System.Linq;
using KVA.Cinema.Services;
using KVA.Cinema.ViewModels;
using KVA.Cinema.Utilities;
using KVA.Cinema.Entities;
using System.Collections.Generic;

namespace KVA.Cinema.Controllers
{
    public class CountriesController : BaseController<Country, CountryCreateViewModel, CountryDisplayViewModel, CountryEditViewModel, CountryService>
    {
        protected override string ModuleCaption { get { return "Countries"; } }

        protected override string CacheKeyCaption { get { return "CountriesSelectedList"; } }

        public CountriesController(CountryService countryService, CacheManager cacheManager) : base(countryService, cacheManager) { }

        protected override CountryEditViewModel MapToEditViewModel(CountryDisplayViewModel displayViewModel)
        {
            return new CountryEditViewModel()
            {
                Id = displayViewModel.Id,
                Name = displayViewModel.Name
            };
        }

        protected override IEnumerable<CountryDisplayViewModel> GetFilterResult(IEnumerable<CountryDisplayViewModel> countries, string query)
        {
            query = query.ToLower();

            return countries.Where(x => x.Name.ToLower().Contains(query));
        }

        protected override IEnumerable<CountryDisplayViewModel> Sort(IEnumerable<CountryDisplayViewModel> countries, string sortColumn, bool isSortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortColumn)
                || !Enum.TryParse(sortColumn, out CountrySort parsedSortColumn))
            {
                parsedSortColumn = CountrySort.Name;
            }

            switch (parsedSortColumn)
            {
                case CountrySort.Name:
                default:
                    return isSortDescending ? countries.OrderByDescending(x => x.Name) : countries.OrderBy(x => x.Name);
            }
        }
    }
}
