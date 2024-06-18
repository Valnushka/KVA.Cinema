using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KVA.Cinema.Models;
using KVA.Cinema.Models.Entities;
using KVA.Cinema.Models.Genre;
using KVA.Cinema.Services;
using KVA.Cinema.Models.ViewModels.Genre;

namespace KVA.Cinema.Controllers
{
    public class GenresController : Controller
    {
        private GenreService GenreService { get; }

        public GenresController(GenreService genreService)
        {
            GenreService = genreService;
        }

        // GET: Genres
        public IActionResult Index(GenreSort sortingField = GenreSort.Title, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            var genres = GenreService.ReadAll();

            if (sortingField == GenreSort.Title && isSortDescending)
            {
                genres = genres.OrderByDescending(s => s.Title);
            }
            else
            {
                genres = genres.OrderBy(s => s.Title);
            }

            return View(genres.ToList());
        }

        // GET: Genres/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = GenreService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
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
                    GenreService.CreateAsync(genreData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(genreData);
        }

        // GET: Genres/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = GenreService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (genre == null)
            {
                return NotFound();
            }

            var genreEditModel = new GenreEditViewModel()
            {
                Id = genre.Id,
                Title = genre.Title
            };

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
            return View(genreNewData);
        }

        // GET: Genres/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = GenreService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var genre = GenreService.ReadAll()
                .FirstOrDefault(m => m.Id == id);
            GenreService.Delete(genre.Id);

            return RedirectToAction(nameof(Index));
        }

        private bool GenreExists(Guid id)
        {
            return GenreService.IsEntityExist(id);
        }
    }
}
