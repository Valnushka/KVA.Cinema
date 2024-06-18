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
using KVA.Cinema.Models.Country;
using KVA.Cinema.Models.ViewModels.Country;

namespace KVA.Cinema.Controllers
{
    public class CountriesController : Controller
    {
        private CountryService CountryService { get; }

        public CountriesController(CountryService countryService)
        {
            CountryService = countryService;
        }

        // GET: Countries
        public IActionResult Index(CountrySort sortingField = CountrySort.Name, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            var countries = CountryService.ReadAll();

            if (sortingField == CountrySort.Name && isSortDescending)
            {
                countries = countries.OrderByDescending(s => s.Name);
            }
            else
            {
                countries = countries.OrderBy(s => s.Name);
            }

            return View(countries.ToList());
        }

        // GET: Countries/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = CountryService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CountryCreateViewModel countryData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CountryService.CreateAsync(countryData);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(countryData);
        }

        // GET: Countries/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = CountryService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            var countryEditModel = new CountryEditViewModel()
            {
                Id = country.Id,
                Name = country.Name
            };

            return View(countryEditModel);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, CountryEditViewModel countryNewData)
        {
            if (id != countryNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    CountryService.Update(id, countryNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(countryNewData);
        }

        // GET: Countries/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = CountryService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var country = CountryService.ReadAll()
                .FirstOrDefault(m => m.Id == id);
            CountryService.Delete(country.Id);

            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(Guid id)
        {
            return CountryService.IsEntityExist(id);
        }
    }
}
