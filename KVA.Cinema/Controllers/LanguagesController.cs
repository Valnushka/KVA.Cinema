using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KVA.Cinema.Services;
using KVA.Cinema.ViewModels;
using KVA.Cinema.Utilities;
using KVA.Cinema.Entities;
using System.Collections.Generic;

namespace KVA.Cinema.Controllers
{
    public class LanguagesController : BaseController<Language, LanguageCreateViewModel, LanguageDisplayViewModel, LanguageEditViewModel, LanguageService>
    {
        protected override string ModuleCaption { get { return "Languages"; } }

        protected override string CacheKeyCaption { get { return "LanguagesSelectedList"; } }

        public LanguagesController(LanguageService languageService, CacheManager cacheManager) : base(languageService, cacheManager) { }

        protected override LanguageEditViewModel MapToEditViewModel(LanguageDisplayViewModel displayViewModel)
        {
            return new LanguageEditViewModel()
            {
                Id = displayViewModel.Id,
                Name = displayViewModel.Name
            };
        }

        protected override IEnumerable<LanguageDisplayViewModel> GetFilterResult(IEnumerable<LanguageDisplayViewModel> languages, string query)
        {
            query = query.ToLower();

            return languages.Where(x => x.Name.ToLower().Contains(query));
        }

        protected override IEnumerable<LanguageDisplayViewModel> Sort(IEnumerable<LanguageDisplayViewModel> languages, string sortColumn, bool isSortDescending)
        {
            switch (sortColumn)
            {
                default:
                    return isSortDescending ? languages.OrderByDescending(x => x.Name) : languages.OrderBy(x => x.Name);
            }
        }
    }
}
