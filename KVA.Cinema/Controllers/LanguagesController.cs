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
using KVA.Cinema.Models.ViewModels.Language;

namespace KVA.Cinema.Controllers
{
    public class LanguagesController : Controller
    {
        private LanguageService LanguageService { get; }

        public LanguagesController(LanguageService languageService)
        {
            LanguageService = languageService;
        }

        // GET: Languages
        public IActionResult Index(LanguageSort sortingField = LanguageSort.Name, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            var languages = LanguageService.ReadAll();

            if (sortingField == LanguageSort.Name && isSortDescending)
            {
                languages = languages.OrderByDescending(s => s.Name);
            }
            else
            {
                languages = languages.OrderBy(s => s.Name);
            }

            return View(languages.ToList());
        }

        // GET: Languages/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = LanguageService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        // GET: Languages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Languages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LanguageCreateViewModel languageData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    LanguageService.CreateAsync(languageData);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(languageData);
        }

        // GET: Languages/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = LanguageService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (language == null)
            {
                return NotFound();
            }

            var languageEditModel = new LanguageEditViewModel()
            {
                Id = language.Id,
                Name = language.Name
            };

            return View(languageEditModel);
        }

        // POST: Languages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, LanguageEditViewModel languageNewData)
        {
            if (id != languageNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    LanguageService.Update(id, languageNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(languageNewData);
        }

        // GET: Languages/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = LanguageService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        // POST: Languages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var language = LanguageService.ReadAll()
                .FirstOrDefault(m => m.Id == id);
            LanguageService.Delete(language.Id);

            return RedirectToAction(nameof(Index));
        }

        private bool LanguageExists(Guid id)
        {
            return LanguageService.IsEntityExist(id);
        }
    }
}
