using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KVA.Cinema.Services;
using KVA.Cinema.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KVA.Cinema.Controllers
{
    public class GenresController : Controller
    {
        private static Breadcrumb homeBreadcrumb;
        private static Breadcrumb indexBreadcrumb;
        private static Breadcrumb detailsBreadcrumb;
        private static Breadcrumb createBreadcrumb;
        private static Breadcrumb editBreadcrumb;
        private static Breadcrumb deleteBreadcrumb;

        private GenreService GenreService { get; }

        public GenresController(GenreService genreService)
        {
            GenreService = genreService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            homeBreadcrumb = new Breadcrumb { Title = "Home", Url = Url.Action("Index", "Home") };
            indexBreadcrumb = new Breadcrumb { Title = "Genres", Url = Url.Action("Index", "Genres") };
            detailsBreadcrumb = new Breadcrumb { Title = "Details", Url = Url.Action("Details", "Genres") };
            createBreadcrumb = new Breadcrumb { Title = "Create", Url = Url.Action("Create", "Genres") };
            editBreadcrumb = new Breadcrumb { Title = "Edit", Url = Url.Action("Edit", "Genres") };
            deleteBreadcrumb = new Breadcrumb { Title = "Delete", Url = Url.Action("Delete", "Genres") };
        }

        // GET: Genres
        [Route("Genres")]
        public IActionResult Index(int? pageNumber,
                                   string searchString,
                                   GenreSort sortingField = GenreSort.Title,
                                   bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;
            ViewBag.CurrentFilter = searchString;

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);

            var genres = GenreService.ReadAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                genres = genres.Where(x => x.Title.Contains(searchString));
            }

            if (sortingField == GenreSort.Title && isSortDescending)
            {
                genres = genres.OrderByDescending(s => s.Title);
            }
            else
            {
                genres = genres.OrderBy(s => s.Title);
            }

            int itemsOnPage = 15;

            return View(PaginatedList<GenreDisplayViewModel>.CreateAsync(genres, pageNumber ?? 1, itemsOnPage));
        }

        // GET: Genres/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GenreDisplayViewModel genre = null;

            try
            {
                genre = GenreService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (genre == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, detailsBreadcrumb);

            return View(genre);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GenreCreateViewModel genreData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    GenreService.Create(genreData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View(genreData);
        }

        // GET: Genres/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GenreDisplayViewModel genre = null;

            try
            {
                genre = GenreService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (genre == null)
            {
                return NotFound();
            }

            var genreEditModel = new GenreEditViewModel()
            {
                Id = genre.Id,
                Title = genre.Title
            };

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(genreEditModel);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, GenreEditViewModel genreNewData)
        {
            if (id != genreNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    GenreService.Update(id, genreNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(genreNewData);
        }

        // GET: Genres/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GenreDisplayViewModel genre = null;

            try
            {
                genre = GenreService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (genre == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                var genre = GenreService.Read(id);
                GenreService.Delete(genre.Id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return RedirectToAction(nameof(Index));
        }

        private void AddBreadcrumbs(params Breadcrumb[] breadcrumbs)
        {
            ViewBag.Breadcrumbs = new List<Breadcrumb>(breadcrumbs);
        }
    }
}
