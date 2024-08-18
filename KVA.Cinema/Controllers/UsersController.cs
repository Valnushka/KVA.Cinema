using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KVA.Cinema.Entities;
using KVA.Cinema.Services;
using KVA.Cinema.Exceptions;
using Microsoft.AspNetCore.Identity;
using KVA.Cinema.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Web;
using KVA.Cinema.Utilities;
using System.Collections.Generic;

namespace KVA.Cinema.Controllers    //TODO: replace NotFound()
{
    public class UsersController : BaseController<User, UserCreateViewModel, UserDisplayViewModel, UserEditViewModel, UserService>
    {
        private static Breadcrumb loginBreadcrumb;
        private static Breadcrumb logoutBreadcrumb;
        private static Breadcrumb buySubscriptionBreadcrumb;
        private static Breadcrumb cancelSubscriptionBreadcrumb;
        private static Breadcrumb subscriptionsBreadcrumb;

        protected override string ModuleCaption { get { return "Users"; } }

        private SubscriptionService SubscriptionService { get; }

        public UsersController(UserService userService, SubscriptionService subscriptionService, CacheManager cacheManager) : base(userService, cacheManager)
        {
            SubscriptionService = subscriptionService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            loginBreadcrumb = new Breadcrumb { Title = "Login", Url = Url.Action("Login", ModuleCaption) };
            logoutBreadcrumb = new Breadcrumb { Title = "Logout", Url = Url.Action("Logout", ModuleCaption) };
            buySubscriptionBreadcrumb = new Breadcrumb { Title = "Buy subscription", Url = Url.Action("BuySubscription", ModuleCaption) };
            cancelSubscriptionBreadcrumb = new Breadcrumb { Title = "Cancel subscription", Url = Url.Action("CancelSubscription", ModuleCaption) };
            subscriptionsBreadcrumb = new Breadcrumb { Title = "Subscriptions", Url = Url.Action("Index", "Subscriptions") };
        }

        //TODO: add to details page:
        //user.SubscriptionNamesAndDates = user.Subscriptions.Count() == 0
        //     ? Enumerable.Empty<string>()
        //     : user.UserSubscriptions.Select(x => $"{x.Subscription.Title}: {x.LastUntil}").ToList();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel userData, string arg)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await EntityService.CreateAsync(userData);
                    return RedirectToAction(nameof(AccountCreated), new { email = userData.Email });
                }
                catch (FailedToCreateEntityException ex)
                {
                    var identityErrors = ex.Errors.OfType<IdentityError>();

                    foreach (var error in identityErrors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddCreateCrumbs();

            return View(userData);
        }

        [Obsolete("This method is obsolete, use async version instead", true)]
        public override IActionResult Create(UserCreateViewModel entityData)
        {
            throw new Exception("This method is obsolete");
        }

        public IActionResult AccountCreated(string email)
        {
            ViewBag.Email = email;

            return View();
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> ConfirmEmail(string userId, string userToken)
        {
            if (userId == null || userToken == null)
            {
                return NotFound();
            }

            string userIdDecoded = HttpUtility.UrlDecode(userId);
            string userTokenDecoded = HttpUtility.UrlDecode(userToken);

            try
            {
                IdentityResult result = await EntityService.ActivateAccountAsync(userIdDecoded, userTokenDecoded);

                if (result.Succeeded)
                {
                    User user = await EntityService.UserManager.FindByIdAsync(userId);
                    await EntityService.SignInManager.SignInAsync(user, true);

                    ViewBag.IsActivationSucceeded = true;

                    return View();
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            ViewBag.IsActivationSucceeded = false;

            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            AddLoginCrumbs();

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                AddLoginCrumbs();
                return View(model);
            }

            User user = await EntityService.UserManager.FindByNameAsync(model.Nickname);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User with this nickname is not found");
                AddLoginCrumbs();
                return View(model);
            }

            if (!user.IsActive || !await EntityService.UserManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Your account is not active. Please confirm your email to access");
                AddLoginCrumbs();
                return View(model);
            }

            var result = await EntityService.SignInManager.PasswordSignInAsync(model.Nickname, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                AddLoginCrumbs();
                return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Incorrect login or/and password");
            AddLoginCrumbs();
            return View(model);
        }

        /// <summary>
        /// Logs out user with authentication cookies removing 
        /// </summary>
        /// <returns>Index action of Home controller</returns>
        [HttpGet, HttpPost]
        public async Task<IActionResult> Logout()
        {
            await EntityService.SignInManager.SignOutAsync();

            AddLogoutCrumbs();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public IActionResult BuySubscription(Guid? subscriptionId)
        {
            if (subscriptionId == null)
            {
                return NotFound();
            }

            var subscription = SubscriptionService.Read(subscriptionId.Value);

            if (subscription == null)
            {
                return NotFound();
            }

            var activatedOn = DateTime.UtcNow; //TODO: use User's time zone
            var lastUntil = activatedOn.Date.AddDays(subscription.Duration + 1);

            subscription.LastUntil = lastUntil;

            AddBuySubscriptionCrumbs();

            return View(subscription);
        }

        [HttpPost, ActionName("BuySubscription")]
        [ValidateAntiForgeryToken]
        public IActionResult BuySubscriptionConfirmed(Guid subscriptionId)
        {
            try
            {
                var user = EntityService.Read(User.Identity.Name);
                EntityService.AddSubscription(user.Nickname, subscriptionId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            AddBuySubscriptionCrumbs();

            return RedirectToAction("Index", "Subscriptions");
        }

        [HttpGet]
        public IActionResult CancelSubscription(Guid? subscriptionId)
        {
            if (subscriptionId == null)
            {
                return NotFound();
            }

            var subscription = SubscriptionService.Read(subscriptionId.Value);

            if (subscription == null)
            {
                return NotFound();
            }

            AddCancelSubscriptionCrumbs();

            return View(subscription);
        }

        [HttpPost, ActionName("CancelSubscription")]
        [ValidateAntiForgeryToken]
        public IActionResult CancelSubscriptionConfirmed(Guid subscriptionId)
        {
            try
            {
                var user = EntityService.Read(User.Identity.Name);
                EntityService.RemoveSubscription(user.Nickname, subscriptionId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            AddCancelSubscriptionCrumbs();

            return RedirectToAction("Index", "Subscriptions");
        }

        protected override UserEditViewModel MapToEditViewModel(UserDisplayViewModel displayViewModel)
        {
            return new UserEditViewModel()
            {
                Id = displayViewModel.Id,
                FirstName = displayViewModel.FirstName,
                LastName = displayViewModel.LastName,
                Nickname = displayViewModel.Nickname,
                BirthDate = displayViewModel.BirthDate,
                Email = displayViewModel.Email
            };
        }

        protected override IEnumerable<UserDisplayViewModel> GetFilterResult(IEnumerable<UserDisplayViewModel> users, string query)
        {
            query = query.ToLower();

            return users.Where(x => x.Nickname.ToLower().Contains(query)
                                                   || x.FirstName.ToLower().Contains(query)
                                                   || x.LastName.ToLower().Contains(query));
        }

        protected override IEnumerable<UserDisplayViewModel> Sort(IEnumerable<UserDisplayViewModel> users, string sortColumn, bool isSortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortColumn)
                || !Enum.TryParse(sortColumn, out UserSort parsedSortColumn))
            {
                parsedSortColumn = UserSort.Nickname;
            }

            switch (parsedSortColumn)
            {
                case UserSort.Nickname:
                    return isSortDescending ? users.OrderByDescending(s => s.Nickname) : users.OrderBy(s => s.Nickname);
                case UserSort.FirstName:
                    return isSortDescending ? users.OrderByDescending(s => s.FirstName) : users.OrderBy(s => s.FirstName);
                case UserSort.LastName:
                    return isSortDescending ? users.OrderByDescending(s => s.LastName) : users.OrderBy(s => s.LastName);
                case UserSort.BirthDate:
                    return isSortDescending ? users.OrderByDescending(s => s.BirthDate) : users.OrderBy(s => s.BirthDate);
                case UserSort.Email:
                    return isSortDescending ? users.OrderByDescending(s => s.Email) : users.OrderBy(s => s.Email);
                default:
                    return users.OrderBy(s => s.Nickname);
            }
        }

        protected void AddLoginCrumbs()
        {
            AddBreadcrumbs(homeBreadcrumb, loginBreadcrumb);
        }

        protected void AddLogoutCrumbs()
        {
            AddBreadcrumbs(homeBreadcrumb, logoutBreadcrumb);
        }

        protected void AddBuySubscriptionCrumbs()
        {
            AddBreadcrumbs(homeBreadcrumb, subscriptionsBreadcrumb, buySubscriptionBreadcrumb);
        }

        protected void AddCancelSubscriptionCrumbs()
        {
            AddBreadcrumbs(homeBreadcrumb, subscriptionsBreadcrumb, buySubscriptionBreadcrumb);
        }
    }
}
