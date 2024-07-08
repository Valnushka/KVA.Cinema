using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KVA.Cinema.Services;
using KVA.Cinema.Models.ViewModels.Pegi;
using KVA.Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KVA.Cinema.Controllers
{
    public class PegiController : Controller
    {
        private static Breadcrumb homeBreadcrumb;
        private static Breadcrumb indexBreadcrumb;
        private static Breadcrumb detailsBreadcrumb;
        private static Breadcrumb createBreadcrumb;
        private static Breadcrumb editBreadcrumb;
        private static Breadcrumb deleteBreadcrumb;

        private PegiService PegiService { get; }

        public PegiController(PegiService pegiService)
        {
            PegiService = pegiService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            homeBreadcrumb = new Breadcrumb { Title = "Home", Url = Url.Action("Index", "Home") };
            indexBreadcrumb = new Breadcrumb { Title = "Pegi", Url = Url.Action("Index", "Pegi") };
            detailsBreadcrumb = new Breadcrumb { Title = "Details", Url = Url.Action("Details", "Pegi") };
            createBreadcrumb = new Breadcrumb { Title = "Create", Url = Url.Action("Create", "Pegi") };
            editBreadcrumb = new Breadcrumb { Title = "Edit", Url = Url.Action("Edit", "Pegi") };
            deleteBreadcrumb = new Breadcrumb { Title = "Delete", Url = Url.Action("Delete", "Pegi") };
        }

        // GET: Pegi
        public IActionResult Index(PegiSort sortingField = PegiSort.Type, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);

            var pegi = PegiService.ReadAll();

            if (sortingField == PegiSort.Type && isSortDescending)
            {
                pegi = pegi.OrderByDescending(s => s.Type);
            }
            else
            {
                pegi = pegi.OrderBy(s => s.Type);
            }

            return View(pegi.ToList());
        }

        // GET: Pegi/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pegi = PegiService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (pegi == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, detailsBreadcrumb);

            return View(pegi);
        }

        // GET: Pegi/Create
        public IActionResult Create()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View();
        }

        // POST: Pegi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PegiCreateViewModel pegiData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    PegiService.CreateAsync(pegiData);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View(pegiData);
        }

        // GET: Pegi/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pegi = PegiService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (pegi == null)
            {
                return NotFound();
            }

            var pegiEditModel = new PegiEditViewModel()
            {
                Id = pegi.Id,
                Type = pegi.Type
            };

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(pegiEditModel);
        }

        // POST: Pegi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, PegiEditViewModel pegiNewData)
        {
            if (id != pegiNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    PegiService.Update(id, pegiNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(pegiNewData);
        }

        // GET: Pegi/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pegi = PegiService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (pegi == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return View(pegi);
        }

        // POST: Pegi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var pegi = PegiService.ReadAll()
                .FirstOrDefault(m => m.Id == id);
            PegiService.Delete(pegi.Id);

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return RedirectToAction(nameof(Index));
        }

        private void AddBreadcrumbs(params Breadcrumb[] breadcrumbs)
        {
            ViewBag.Breadcrumbs = new List<Breadcrumb>(breadcrumbs);
        }
    }
}
