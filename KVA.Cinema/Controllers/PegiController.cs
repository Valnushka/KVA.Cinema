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
    public class PegiController : BaseController<Pegi, PegiCreateViewModel, PegiDisplayViewModel, PegiEditViewModel, PegiService>
    {
        protected override string ModuleCaption { get { return "Pegi"; } }

        protected override string CacheKeyCaption { get { return "PegisSelectedList"; } } //TODO: Rename list

        public PegiController(PegiService pegiService, CacheManager cacheManager) : base(pegiService, cacheManager) { }

        protected override PegiEditViewModel MapToEditViewModel(PegiDisplayViewModel displayViewModel)
        {
            return new PegiEditViewModel()
            {
                Id = displayViewModel.Id,
                Type = displayViewModel.Type
            };
        }

        protected override IEnumerable<PegiDisplayViewModel> GetFilterResult(IEnumerable<PegiDisplayViewModel> pegi, string query)
        {
            query = query.ToLower();

            return pegi.Where(x => x.Type.ToString().Contains(query));
        }

        protected override IEnumerable<PegiDisplayViewModel> Sort(IEnumerable<PegiDisplayViewModel> pegi, string sortColumn, bool isSortDescending)
        {
            switch (sortColumn)
            {
                default:
                    return isSortDescending ? pegi.OrderByDescending(x => x.Type) : pegi.OrderBy(x => x.Type);
            }
        }
    }
}
