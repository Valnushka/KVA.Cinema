using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KVA.Cinema.Models;
using KVA.Cinema.Models.Entities;
using KVA.Cinema.Services;
using KVA.Cinema.Models.Director;
using KVA.Cinema.Models.ViewModels.Director;

namespace KVA.Cinema.Controllers
{
    public class DirectorsController : Controller
    {
        private DirectorService DirectorService { get; }

        public DirectorsController(DirectorService directorService)
        {
            DirectorService = directorService;
        }

        // GET: Directors
        public IActionResult Index(DirectorSort sortingField = DirectorSort.Name, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

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

            return View(director);
        }

        // GET: Directors/Create
        public IActionResult Create()
        {
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

            return RedirectToAction(nameof(Index));
        }

        private bool DirectorExists(Guid id)
        {
            return DirectorService.IsEntityExist(id);
        }
    }
}
