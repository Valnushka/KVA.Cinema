using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KVA.Cinema.Services;
using KVA.Cinema.Models.Director;
using KVA.Cinema.Models.ViewModels.Director;
using KVA.Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KVA.Cinema.Controllers
{
    public class DirectorsController : Controller
    {
        private static Breadcrumb homeBreadcrumb;
        private static Breadcrumb indexBreadcrumb;
        private static Breadcrumb detailsBreadcrumb;
        private static Breadcrumb createBreadcrumb;
        private static Breadcrumb editBreadcrumb;
        private static Breadcrumb deleteBreadcrumb;

        private DirectorService DirectorService { get; }

        public DirectorsController(DirectorService directorService)
        {
            DirectorService = directorService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            homeBreadcrumb = new Breadcrumb { Title = "Home", Url = Url.Action("Index", "Home") };
            indexBreadcrumb = new Breadcrumb { Title = "Directors", Url = Url.Action("Index", "Directors") };
            detailsBreadcrumb = new Breadcrumb { Title = "Details", Url = Url.Action("Details", "Directors") };
            createBreadcrumb = new Breadcrumb { Title = "Create", Url = Url.Action("Create", "Directors") };
            editBreadcrumb = new Breadcrumb { Title = "Edit", Url = Url.Action("Edit", "Directors") };
            deleteBreadcrumb = new Breadcrumb { Title = "Delete", Url = Url.Action("Delete", "Directors") };
        }

        // GET: Directors
        [Route("Directors")]
        public IActionResult Index(DirectorSort sortingField = DirectorSort.Name, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);

            var directors = DirectorService.ReadAll();

            if (sortingField == DirectorSort.Name && isSortDescending)
            {
                directors = directors.OrderByDescending(s => s.Name);
            }
            else
            {
                directors = directors.OrderBy(s => s.Name);
            }

            return View(directors.ToList());
        }

        // GET: Directors/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = DirectorService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (director == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, detailsBreadcrumb);

            return View(director);
        }

        // GET: Directors/Create
        public IActionResult Create()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View();
        }

        // POST: Directors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DirectorCreateViewModel directorData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DirectorService.CreateAsync(directorData);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View(directorData);
        }

        // GET: Directors/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = DirectorService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (director == null)
            {
                return NotFound();
            }

            var directorEditModel = new DirectorEditViewModel()
            {
                Id = director.Id,
                Name = director.Name
            };

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(directorEditModel);
        }

        // POST: Directors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, DirectorEditViewModel directorNewData)
        {
            if (id != directorNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DirectorService.Update(id, directorNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(directorNewData);
        }

        // GET: Directors/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var director = DirectorService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (director == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return View(director);
        }

        // POST: Directors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var director = DirectorService.ReadAll()
                .FirstOrDefault(m => m.Id == id);
            DirectorService.Delete(director.Id);

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return RedirectToAction(nameof(Index));
        }

        private bool DirectorExists(Guid id)
        {
            return DirectorService.IsEntityExist(id);
        }

        private void AddBreadcrumbs(params Breadcrumb[] breadcrumbs)
        {
            ViewBag.Breadcrumbs = new List<Breadcrumb>(breadcrumbs);
        }
    }
}
