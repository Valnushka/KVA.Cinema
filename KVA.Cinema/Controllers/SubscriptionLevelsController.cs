using System;
using System.Linq;
using KVA.Cinema.Services;
using KVA.Cinema.ViewModels;
using KVA.Cinema.Entities;
using System.Collections.Generic;
using KVA.Cinema.Utilities;

namespace KVA.Cinema.Controllers
{
    public class SubscriptionLevelsController : BaseController<SubscriptionLevel, SubscriptionLevelCreateViewModel, SubscriptionLevelDisplayViewModel, SubscriptionLevelEditViewModel, SubscriptionLevelService>
    {
        protected override string ModuleCaption { get { return "SubscriptionLevels"; } }

        public SubscriptionLevelsController(SubscriptionLevelService subscriptionLevelService, CacheManager cacheManager) : base(subscriptionLevelService, cacheManager) { }

        protected override SubscriptionLevelEditViewModel MapToEditViewModel(SubscriptionLevelDisplayViewModel displayViewModel)
        {
            return new SubscriptionLevelEditViewModel()
            {
                Id = displayViewModel.Id,
                Title = displayViewModel.Title
            };
        }

        protected override IEnumerable<SubscriptionLevelDisplayViewModel> GetFilterResult(IEnumerable<SubscriptionLevelDisplayViewModel> subscriptionLevels, string query)
        {
            query = query.ToLower();

            return subscriptionLevels.Where(x => x.Title.ToLower().Contains(query));
        }

        protected override IEnumerable<SubscriptionLevelDisplayViewModel> Sort(IEnumerable<SubscriptionLevelDisplayViewModel> subscriptionLevels, string sortColumn, bool isSortDescending)
        {
            switch (sortColumn)
            {
                default:
                    return isSortDescending ? subscriptionLevels.OrderByDescending(x => x.Title) : subscriptionLevels.OrderBy(x => x.Title);
            }
        }
    }
}
