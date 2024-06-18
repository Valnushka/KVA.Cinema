using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KVA.Cinema.Models;
using KVA.Cinema.Models.Entities;
using KVA.Cinema.Models.User;
using KVA.Cinema.Services;
using KVA.Cinema.Exceptions;
using Microsoft.AspNetCore.Identity;
using KVA.Cinema.Models.ViewModels.User;
using KVA.Cinema.Models.ViewModels.Subscription;

namespace KVA.Cinema.Controllers    //TODO: replace NotFound()
{
    public class UsersController : Controller
    {
        private UserService UserService { get; }

        private SubscriptionService SubscriptionService { get; }

        public UsersController(UserService userService, SubscriptionService subscriptionService)
        {
            UserService = userService;
            SubscriptionService = subscriptionService;
        }

        // GET: Users
        public IActionResult Index(UserSort sortingField = UserSort.Nickname, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

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

            return View(users.ToList());
        }

        // GET: Users/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = UserService.Read(id.Value);

            if (user == null)
            {
                return NotFound();
            }

            //user.SubscriptionNamesAndDates = user.Subscriptions.Count() == 0
            //     ? Enumerable.Empty<string>()
            //     : user.UserSubscriptions.Select(x => $"{x.Subscription.Title}: {x.LastUntil}").ToList();

            return View(user);
        }

        // GET: Users/Create
        [HttpGet]
        public IActionResult Create()
        {
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
                    return RedirectToAction(nameof(Index));
                }
                catch (FailedToCreateEntityException ex)
                {
                    foreach (var error in ex.Errors)
                    {
                        ModelState.AddModelError(string.Empty, (error as IdentityError).Description);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(userData);
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
            return View(userNewData);
        }

        // GET: Users/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = UserService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var user = UserService.ReadAll()
                .FirstOrDefault(m => m.Id == id);
            UserService.Delete(user.Id);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await UserService.SignInManager.PasswordSignInAsync(model.Nickname, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    //проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login or/and password");
                }
            }
            return View(model);
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await UserService.SignInManager.SignOutAsync();
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

            return View(subscription);
        }

        [HttpPost, ActionName("BuySubscription")]
        [ValidateAntiForgeryToken]
        public IActionResult BuySubscriptionConfirmed(Guid subscriptionId)
        {
            var user = UserService.ReadAll()
                .FirstOrDefault(m => m.Nickname == User.Identity.Name);

            try
            {
                UserService.AddSubscription(user.Nickname, subscriptionId);
                return RedirectToAction(nameof(Index), "Subscriptions");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var subscription = SubscriptionService.Read(subscriptionId);

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

            return View(subscription);
        }

        [HttpPost, ActionName("CancelSubscription")]
        [ValidateAntiForgeryToken]
        public IActionResult CancelSubscriptionConfirmed(Guid subscriptionId)
        {
            var user = UserService.ReadAll()
                .FirstOrDefault(m => m.Nickname == User.Identity.Name);

            try
            {
                UserService.RemoveSubscription(user.Nickname, subscriptionId);
                return RedirectToAction(nameof(Index), "Subscriptions");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var subscription = SubscriptionService.Read(subscriptionId);

            return View(subscription);
        }

        private bool UserExists(Guid id)
        {
            return UserService.IsEntityExist(id);
        }
    }
}
