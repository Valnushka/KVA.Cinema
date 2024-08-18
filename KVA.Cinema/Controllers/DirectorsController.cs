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
    public class DirectorsController : BaseController<Director, DirectorCreateViewModel, DirectorDisplayViewModel, DirectorEditViewModel, DirectorService>
    {
        protected override string ModuleCaption { get { return "Directors"; } }

        protected override string CacheKeyCaption { get { return "DirectorsSelectedList"; } }

        public DirectorsController(DirectorService directorService, CacheManager cacheManager) : base(directorService, cacheManager) { }

        protected override DirectorEditViewModel MapToEditViewModel(DirectorDisplayViewModel displayViewModel)
        {
            return new DirectorEditViewModel()
            {
                Id = displayViewModel.Id,
                Name = displayViewModel.Name
            };
        }

        protected override IEnumerable<DirectorDisplayViewModel> GetFilterResult(IEnumerable<DirectorDisplayViewModel> directors, string query)
        {
            query = query.ToLower();

            return directors.Where(x => x.Name.ToLower().Contains(query));
        }

        protected override IEnumerable<DirectorDisplayViewModel> Sort(IEnumerable<DirectorDisplayViewModel> directors, string sortColumn, bool isSortDescending)
        {
            switch (sortColumn)
            {
                default:
                    return isSortDescending ? directors.OrderByDescending(x => x.Name) : directors.OrderBy(x => x.Name);
            }
        }
    }
}
