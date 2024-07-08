﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KVA.Cinema.Models;
using KVA.Cinema.Models.Entities;
using KVA.Cinema.Services;
using KVA.Cinema.Models.ViewModels.Subscription;
using KVA.Cinema.Models.ViewModels.Video;
using KVA.Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KVA.Cinema.Controllers
{
    public class SubscriptionsController : Controller
    {
        private static Breadcrumb homeBreadcrumb;
        private static Breadcrumb indexBreadcrumb;
        private static Breadcrumb detailsBreadcrumb;
        private static Breadcrumb createBreadcrumb;
        private static Breadcrumb editBreadcrumb;
        private static Breadcrumb deleteBreadcrumb;

        private SubscriptionService SubscriptionService { get; }

        private SubscriptionLevelService SubscriptionLevelService { get; }

        private UserService UserService { get; }

        private VideoService VideoService { get; }

        public SubscriptionsController(SubscriptionService subscriptionService,
                                       SubscriptionLevelService subscriptionLevelService,
                                       UserService userService,
                                       VideoService videoService)
        {
            SubscriptionService = subscriptionService;
            SubscriptionLevelService = subscriptionLevelService;
            UserService = userService;
            VideoService = videoService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            homeBreadcrumb = new Breadcrumb { Title = "Home", Url = Url.Action("Index", "Home") };
            indexBreadcrumb = new Breadcrumb { Title = "Subscriptions", Url = Url.Action("Index", "Subscriptions") };
            detailsBreadcrumb = new Breadcrumb { Title = "Details", Url = Url.Action("Details", "Subscriptions") };
            createBreadcrumb = new Breadcrumb { Title = "Create", Url = Url.Action("Create", "Subscriptions") };
            editBreadcrumb = new Breadcrumb { Title = "Edit", Url = Url.Action("Edit", "Subscriptions") };
            deleteBreadcrumb = new Breadcrumb { Title = "Delete", Url = Url.Action("Delete", "Subscriptions") };
        }

        // GET: Subscriptions
        public IActionResult Index(SubscriptionSort sortingField = SubscriptionSort.Title, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);

            var subscriptions = SubscriptionService.ReadAll();

            switch (sortingField)
            {
                case SubscriptionSort.Title:
                    subscriptions = isSortDescending ? subscriptions.OrderByDescending(s => s.Title) : subscriptions.OrderBy(s => s.Title);
                    break;
                case SubscriptionSort.Cost:
                    subscriptions = isSortDescending ? subscriptions.OrderByDescending(s => s.Cost) : subscriptions.OrderBy(s => s.Cost);
                    break;
                case SubscriptionSort.ReleasedIn:
                    subscriptions = isSortDescending ? subscriptions.OrderByDescending(s => s.ReleasedIn) : subscriptions.OrderBy(s => s.ReleasedIn);
                    break;
                case SubscriptionSort.Duration:
                    subscriptions = isSortDescending ? subscriptions.OrderByDescending(s => s.Duration) : subscriptions.OrderBy(s => s.Duration);
                    break;
                case SubscriptionSort.AvailableUntil:
                    subscriptions = isSortDescending ? subscriptions.OrderByDescending(s => s.AvailableUntil) : subscriptions.OrderBy(s => s.AvailableUntil);
                    break;
                case SubscriptionSort.Level:
                    subscriptions = isSortDescending ? subscriptions.OrderByDescending(s => s.LevelName) : subscriptions.OrderBy(s => s.LevelName);
                    break;
                default:
                    subscriptions = subscriptions.OrderBy(s => s.Title);
                    break;
            }

            return View(subscriptions.ToList());
        }

        // GET: Subscriptions/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SubscriptionDisplayViewModel subscription = null;

            try
            {
                subscription = SubscriptionService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (subscription == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, detailsBreadcrumb);

            return View(subscription);
        }

        // GET: Subscriptions/Create
        public IActionResult Create()
        {
            ViewBag.LevelId = new SelectList(SubscriptionLevelService.ReadAll(), "Id", "Title");
            ViewBag.VideoIds = new SelectList(VideoService.ReadAll(), "Id", nameof(VideoDisplayViewModel.Name));

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View();
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubscriptionCreateViewModel subscriptionData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SubscriptionService.CreateAsync(subscriptionData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewBag.LevelId = new SelectList(SubscriptionLevelService.ReadAll(), "Id", "Title");
            ViewBag.VideoIds = new SelectList(VideoService.ReadAll(), "Id", nameof(VideoDisplayViewModel.Name));

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View(subscriptionData);
        }

        // GET: Subscriptions/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = SubscriptionService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (subscription == null)
            {
                return NotFound();
            }

            ViewBag.LevelId = new SelectList(SubscriptionLevelService.ReadAll(), "Id", "Title");
            ViewBag.VideoIds = new SelectList(VideoService.ReadAll(), "Id", nameof(VideoDisplayViewModel.Name));

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            var subscriptionEditModel = new SubscriptionEditViewModel()
            {
                Id = subscription.Id,
                Title = subscription.Title,
                Description = subscription.Description,
                Cost = subscription.Cost,
                LevelId = subscription.LevelId,
                ReleasedIn = subscription.ReleasedIn,
                Duration = subscription.Duration,
                AvailableUntil = subscription.AvailableUntil,
                VideoIds = subscription.VideosInSubscription.Select(x => x.VideoId)
            };

            return View(subscriptionEditModel);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, SubscriptionEditViewModel subscriptionNewData)
        {
            if (id != subscriptionNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SubscriptionService.Update(id, subscriptionNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewBag.LevelId = new SelectList(SubscriptionLevelService.ReadAll(), "Id", "Title");
            ViewBag.VideoIds = new SelectList(VideoService.ReadAll(), "Id", nameof(VideoDisplayViewModel.Name));

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(subscriptionNewData);
        }

        // GET: Subscriptions/Delete/5
        public IActionResult Delete(Guid? id)
        {
            var subscription = SubscriptionService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (subscription == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var subscription = SubscriptionService.ReadAll()
                .FirstOrDefault(m => m.Id == id);
            SubscriptionService.Delete(subscription.Id);

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(Guid id)
        {
            return SubscriptionService.IsEntityExist(id);
        }

        private void AddBreadcrumbs(params Breadcrumb[] breadcrumbs)
        {
            ViewBag.Breadcrumbs = new List<Breadcrumb>(breadcrumbs);
        }
    }
}
