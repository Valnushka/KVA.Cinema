using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KVA.Cinema.Models.Entities;
using KVA.Cinema.Models.User;
using KVA.Cinema.Services;
using KVA.Cinema.Exceptions;
using Microsoft.AspNetCore.Identity;
using KVA.Cinema.Models.ViewModels.User;
using KVA.Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Web;

namespace KVA.Cinema.Controllers    //TODO: replace NotFound()
{
    public class UsersController : Controller
    {
        private static Breadcrumb homeBreadcrumb;
        private static Breadcrumb indexBreadcrumb;
        private static Breadcrumb detailsBreadcrumb;
        private static Breadcrumb createBreadcrumb;
        private static Breadcrumb editBreadcrumb;
        private static Breadcrumb deleteBreadcrumb;
        private static Breadcrumb loginBreadcrumb;
        private static Breadcrumb logoutBreadcrumb;
        private static Breadcrumb buySubscriptionBreadcrumb;
        private static Breadcrumb cancelSubscriptionBreadcrumb;
        private static Breadcrumb subscriptionsBreadcrumb;

        private UserService UserService { get; }

        private SubscriptionService SubscriptionService { get; }

        public UsersController(UserService userService, SubscriptionService subscriptionService)
        {
            UserService = userService;
            SubscriptionService = subscriptionService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            homeBreadcrumb = new Breadcrumb { Title = "Home", Url = Url.Action("Index", "Home") };
            indexBreadcrumb = new Breadcrumb { Title = "Users", Url = Url.Action("Index", "Users") };
            detailsBreadcrumb = new Breadcrumb { Title = "Details", Url = Url.Action("Details", "Users") };
            createBreadcrumb = new Breadcrumb { Title = "Create", Url = Url.Action("Create", "Users") };
            editBreadcrumb = new Breadcrumb { Title = "Edit", Url = Url.Action("Edit", "Users") };
            deleteBreadcrumb = new Breadcrumb { Title = "Delete", Url = Url.Action("Delete", "Users") };
            loginBreadcrumb = new Breadcrumb { Title = "Login", Url = Url.Action("Login", "Users") }; ;
            logoutBreadcrumb = new Breadcrumb { Title = "Logout", Url = Url.Action("Logout", "Users") };
            buySubscriptionBreadcrumb = new Breadcrumb { Title = "Buy subscription", Url = Url.Action("BuySubscription", "Users") };
            cancelSubscriptionBreadcrumb = new Breadcrumb { Title = "Cancel subscription", Url = Url.Action("CancelSubscription", "Users") };
            subscriptionsBreadcrumb = new Breadcrumb { Title = "Subscriptions", Url = Url.Action("Index", "Subscriptions") };
        }

        // GET: Users
        [Route("Users")]
        public IActionResult Index(int? pageNumber,
                                   UserSort sortingField = UserSort.Nickname,
                                   bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);

            var users = UserService.ReadAll();

            switch (sortingField)
            {
                case UserSort.Nickname:
                    users = isSortDescending ? users.OrderByDescending(s => s.Nickname) : users.OrderBy(s => s.Nickname);
                    break;
                case UserSort.FirstName:
                    users = isSortDescending ? users.OrderByDescending(s => s.FirstName) : users.OrderBy(s => s.FirstName);
                    break;
                case UserSort.LastName:
                    users = isSortDescending ? users.OrderByDescending(s => s.LastName) : users.OrderBy(s => s.LastName);
                    break;
                case UserSort.BirthDate:
                    users = isSortDescending ? users.OrderByDescending(s => s.BirthDate) : users.OrderBy(s => s.BirthDate);
                    break;
                case UserSort.Email:
                    users = isSortDescending ? users.OrderByDescending(s => s.Email) : users.OrderBy(s => s.Email);
                    break;
                default:
                    users = users.OrderBy(s => s.Nickname);
                    break;
            }

            int itemsOnPage = 10;

            return View(PaginatedList<UserDisplayViewModel>.CreateAsync(users, pageNumber ?? 1, itemsOnPage));
        }

        // GET: Users/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserDisplayViewModel user = null;

            try
            {
                user = UserService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (user == null)
            {
                return NotFound();
            }

            //user.SubscriptionNamesAndDates = user.Subscriptions.Count() == 0
            //     ? Enumerable.Empty<string>()
            //     : user.UserSubscriptions.Select(x => $"{x.Subscription.Title}: {x.LastUntil}").ToList();

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, detailsBreadcrumb);

            return View(user);
        }

        // GET: Users/Create
        [HttpGet]
        public IActionResult Create()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel userData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await UserService.CreateAsync(userData);
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

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View(userData);
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
                IdentityResult result = await UserService.ActivateAccountAsync(userIdDecoded, userTokenDecoded);

                if (result.Succeeded)
                {
                    User user = await UserService.UserManager.FindByIdAsync(userId);
                    await UserService.SignInManager.SignInAsync(user, true);

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

        // GET: Users/Edit/5
        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = UserService.Read()
                .FirstOrDefault(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            var userEditModel = new UserEditViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Nickname = user.Nickname,
                BirthDate = user.BirthDate,
                Email = user.Email
            };

            return View(userEditModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, UserEditViewModel userNewData)
        {
            if (id != userNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    UserService.Update(id, userNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(userNewData);
        }

        // GET: Users/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserDisplayViewModel user = null;

            try
            {
                user = UserService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (user == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                var user = UserService.Read(id);
                UserService.Delete(user.Id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            AddBreadcrumbs(homeBreadcrumb, loginBreadcrumb);

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                AddBreadcrumbs(homeBreadcrumb, loginBreadcrumb);
                return View(model);
            }

            User user = await UserService.UserManager.FindByNameAsync(model.Nickname);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User with this nickname is not found");
                AddBreadcrumbs(homeBreadcrumb, loginBreadcrumb);
                return View(model);
            }

            if (!user.IsActive || !await UserService.UserManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Your account is not active. Please confirm your email to access");
                AddBreadcrumbs(homeBreadcrumb, loginBreadcrumb);
                return View(model);
            }

            var result = await UserService.SignInManager.PasswordSignInAsync(model.Nickname, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                AddBreadcrumbs(homeBreadcrumb, loginBreadcrumb);
                return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Incorrect login or/and password");
            AddBreadcrumbs(homeBreadcrumb, loginBreadcrumb);
            return View(model);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await UserService.SignInManager.SignOutAsync();

            AddBreadcrumbs(homeBreadcrumb, logoutBreadcrumb);

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

            var activatedOn = DateTime.UtcNow; //TODO: использовать часовой пояс пользователя
            var lastUntil = activatedOn.Date.AddDays(subscription.Duration + 1);

            subscription.LastUntil = lastUntil;

            AddBreadcrumbs(homeBreadcrumb, subscriptionsBreadcrumb, buySubscriptionBreadcrumb);

            return View(subscription);
        }

        [HttpPost, ActionName("BuySubscription")]
        [ValidateAntiForgeryToken]
        public IActionResult BuySubscriptionConfirmed(Guid subscriptionId)
        {
            try
            {
                var user = UserService.Read(User.Identity.Name);
                UserService.AddSubscription(user.Nickname, subscriptionId);
                return RedirectToAction(nameof(Index), "Users");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var subscription = SubscriptionService.Read(subscriptionId);

            AddBreadcrumbs(homeBreadcrumb, subscriptionsBreadcrumb, buySubscriptionBreadcrumb);

            return View(subscription);
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

            AddBreadcrumbs(homeBreadcrumb, subscriptionsBreadcrumb, cancelSubscriptionBreadcrumb);

            return View(subscription);
        }

        [HttpPost, ActionName("CancelSubscription")]
        [ValidateAntiForgeryToken]
        public IActionResult CancelSubscriptionConfirmed(Guid subscriptionId)
        {
            try
            {
                var user = UserService.Read(User.Identity.Name);
                UserService.RemoveSubscription(user.Nickname, subscriptionId);
                return RedirectToAction(nameof(Index), "Users");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var subscription = SubscriptionService.Read(subscriptionId);

            AddBreadcrumbs(homeBreadcrumb, subscriptionsBreadcrumb, cancelSubscriptionBreadcrumb);

            return View(subscription);
        }

        private void AddBreadcrumbs(params Breadcrumb[] breadcrumbs)
        {
            ViewBag.Breadcrumbs = new List<Breadcrumb>(breadcrumbs);
        }
    }
}
