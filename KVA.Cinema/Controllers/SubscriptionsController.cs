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
    public class SubscriptionsController : BaseController<Subscription, SubscriptionCreateViewModel, SubscriptionDisplayViewModel, SubscriptionEditViewModel, SubscriptionService>
    {
        protected override string ModuleCaption { get { return "Subscriptions"; } }

        private SubscriptionLevelService SubscriptionLevelService { get; }

        private UserService UserService { get; }

        private VideoService VideoService { get; }

        public SubscriptionsController(SubscriptionService subscriptionService,
                                       SubscriptionLevelService subscriptionLevelService,
                                       UserService userService,
                                       VideoService videoService,
                                       CacheManager memoryCache) : base(subscriptionService, memoryCache)
        {
            SubscriptionLevelService = subscriptionLevelService;
            UserService = userService;
            VideoService = videoService;
        }

        // GET: Subscriptions
        [Route("Subscriptions")]
        protected override IActionResult Index(PaginationConfig paginationConfig, SortConfig sortConfig, FilterConfig filterConfig)
        {
            FillViewBag(sortConfig, filterConfig);

            AddIndexCrumbs();

            var subscriptions = EntityService.ReadAll();

            if (User.Identity.IsAuthenticated)
            {
                var user = UserService.ReadAll().FirstOrDefault(m => m.Nickname == User.Identity.Name);

                if (user == null) //TODO: add kicking out of account after user deleting 
                {
                    return NotFound();
                }

                foreach (var subscription in subscriptions)
                {
                    subscription.IsPurchasedByCurrentUser = user.UserSubscriptions.Any(m => m.SubscriptionId == subscription.Id);
                }
            }

            if (!string.IsNullOrEmpty(filterConfig.Query))
            {
                subscriptions = GetFilterResult(subscriptions, filterConfig?.Query);
            }

            subscriptions = Sort(subscriptions, sortConfig.SortColumn, sortConfig.IsDescending);

            return View(
                PaginatedList<SubscriptionDisplayViewModel>.CreateAsync(
                    subscriptions,
                    paginationConfig?.Page ?? 1,
                    paginationConfig?.ItemsOnPage ?? 15
                    )
                );
        }

        protected override void GetCachedEntities()
        {
            ViewBag.LevelId = CacheManager.GetCachedSelectList("LevelsSelectedList", SubscriptionLevelService.ReadAll, nameof(SubscriptionLevelDisplayViewModel.Id), nameof(SubscriptionLevelDisplayViewModel.Title));
            ViewBag.VideoIds = CacheManager.GetCachedSelectList("VideosSelectedList", VideoService.ReadAll, nameof(VideoDisplayViewModel.Id), nameof(VideoDisplayViewModel.Name));
        }

        protected override SubscriptionEditViewModel MapToEditViewModel(SubscriptionDisplayViewModel displayViewModel)
        {
            return new SubscriptionEditViewModel()
            {
                Id = displayViewModel.Id,
                Title = displayViewModel.Title,
                Description = displayViewModel.Description,
                Cost = displayViewModel.Cost,
                LevelId = displayViewModel.LevelId,
                ReleasedIn = displayViewModel.ReleasedIn,
                Duration = displayViewModel.Duration,
                AvailableUntil = displayViewModel.AvailableUntil,
                VideoIds = displayViewModel.VideosInSubscription.Select(x => x.VideoId)
            };
        }

        protected override IEnumerable<SubscriptionDisplayViewModel> GetFilterResult(IEnumerable<SubscriptionDisplayViewModel> subscriptions, string query)
        {
            query = query.ToLower();

            return subscriptions = subscriptions.Where(x => x.Title.Contains(query));
        }

        protected override IEnumerable<SubscriptionDisplayViewModel> Sort(IEnumerable<SubscriptionDisplayViewModel> subscriptions, string sortColumn, bool isSortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortColumn)
                            || !Enum.TryParse(sortColumn, out SubscriptionSort parsedSortColumn))
            {
                parsedSortColumn = SubscriptionSort.Title;
            }

            switch (parsedSortColumn)
            {
                case SubscriptionSort.Title:
                    return isSortDescending ? subscriptions.OrderByDescending(s => s.Title) : subscriptions.OrderBy(s => s.Title);
                case SubscriptionSort.Cost:
                    return isSortDescending ? subscriptions.OrderByDescending(s => s.Cost) : subscriptions.OrderBy(s => s.Cost);
                case SubscriptionSort.ReleasedIn:
                    return isSortDescending ? subscriptions.OrderByDescending(s => s.ReleasedIn) : subscriptions.OrderBy(s => s.ReleasedIn);
                case SubscriptionSort.Duration:
                    return isSortDescending ? subscriptions.OrderByDescending(s => s.Duration) : subscriptions.OrderBy(s => s.Duration);
                case SubscriptionSort.AvailableUntil:
                    return isSortDescending ? subscriptions.OrderByDescending(s => s.AvailableUntil) : subscriptions.OrderBy(s => s.AvailableUntil);
                case SubscriptionSort.Level:
                    return isSortDescending ? subscriptions.OrderByDescending(s => s.LevelName) : subscriptions.OrderBy(s => s.LevelName);
                default:
                    return subscriptions.OrderBy(s => s.Title);
            }
        }
    }
}
