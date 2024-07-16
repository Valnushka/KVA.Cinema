using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KVA.Cinema.Services;
using KVA.Cinema.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KVA.Cinema.Controllers
{
    public class SubscriptionLevelsController : Controller
    {
        private static Breadcrumb homeBreadcrumb;
        private static Breadcrumb indexBreadcrumb;
        private static Breadcrumb detailsBreadcrumb;
        private static Breadcrumb createBreadcrumb;
        private static Breadcrumb editBreadcrumb;
        private static Breadcrumb deleteBreadcrumb;

        private SubscriptionLevelService SubscriptionLevelService { get; }

        public SubscriptionLevelsController(SubscriptionLevelService subscriptionLevelService)
        {
            SubscriptionLevelService = subscriptionLevelService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            homeBreadcrumb = new Breadcrumb { Title = "Home", Url = Url.Action("Index", "Home") };
            indexBreadcrumb = new Breadcrumb { Title = "SubscriptionLevels", Url = Url.Action("Index", "SubscriptionLevels") };
            detailsBreadcrumb = new Breadcrumb { Title = "Details", Url = Url.Action("Details", "SubscriptionLevels") };
            createBreadcrumb = new Breadcrumb { Title = "Create", Url = Url.Action("Create", "SubscriptionLevels") };
            editBreadcrumb = new Breadcrumb { Title = "Edit", Url = Url.Action("Edit", "SubscriptionLevels") };
            deleteBreadcrumb = new Breadcrumb { Title = "Delete", Url = Url.Action("Delete", "SubscriptionLevels") };
        }

        // GET: SubscriptionLevels
        [Route("SubscriptionLevels")]
        public IActionResult Index(int? pageNumber,
                                   string searchString,
                                   SubscriptionLevelSort sortingField = SubscriptionLevelSort.Title,
                                   bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;
            ViewBag.CurrentFilter = searchString;

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);

            var subscriptionLevels = SubscriptionLevelService.ReadAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                subscriptionLevels = subscriptionLevels.Where(x => x.Title.Contains(searchString));
            }

            if (sortingField == SubscriptionLevelSort.Title && isSortDescending)
            {
                subscriptionLevels = subscriptionLevels.OrderByDescending(s => s.Title);
            }
            else
            {
                subscriptionLevels = subscriptionLevels.OrderBy(s => s.Title);
            }

            int itemsOnPage = 15;

            return View(PaginatedList<SubscriptionLevelDisplayViewModel>.CreateAsync(subscriptionLevels, pageNumber ?? 1, itemsOnPage));
        }

        // GET: SubscriptionLevels/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SubscriptionLevelDisplayViewModel subscriptionLevel = null;

            try
            {
                subscriptionLevel = SubscriptionLevelService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (subscriptionLevel == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, detailsBreadcrumb);

            return View(subscriptionLevel);
        }

        // GET: SubscriptionLevels/Create
        public IActionResult Create()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View();
        }

        // POST: SubscriptionLevels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubscriptionLevelCreateViewModel subscriptionLevelData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SubscriptionLevelService.CreateAsync(subscriptionLevelData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View(subscriptionLevelData);
        }

        // GET: SubscriptionLevels/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionLevel = SubscriptionLevelService.Read()
                .FirstOrDefault(m => m.Id == id);

            if (subscriptionLevel == null)
            {
                return NotFound();
            }

            var subscriptionLevelEditModel = new SubscriptionLevelEditViewModel()
            {
                Id = subscriptionLevel.Id,
                Title = subscriptionLevel.Title
            };

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(subscriptionLevelEditModel);
        }

        // POST: SubscriptionLevels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, SubscriptionLevelEditViewModel subscriptionLevelNewData)
        {
            if (id != subscriptionLevelNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SubscriptionLevelService.Update(id, subscriptionLevelNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(subscriptionLevelNewData);
        }

        // GET: SubscriptionLevels/Delete/5
        public IActionResult Delete(Guid? id)
        {
            SubscriptionLevelDisplayViewModel subscriptionLevel = null;

            try
            {
                subscriptionLevel = SubscriptionLevelService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (subscriptionLevel == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return View(subscriptionLevel);
        }

        // POST: SubscriptionLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                SubscriptionLevelDisplayViewModel subscriptionLevel = SubscriptionLevelService.Read(id);
                SubscriptionLevelService.Delete(subscriptionLevel.Id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return RedirectToAction(nameof(Index));
        }

        private void AddBreadcrumbs(params Breadcrumb[] breadcrumbs)
        {
            ViewBag.Breadcrumbs = new List<Breadcrumb>(breadcrumbs);
        }
    }
}
