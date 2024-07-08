﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KVA.Cinema.Services;
using KVA.Cinema.Models.Country;
using KVA.Cinema.Models.ViewModels.Country;
using KVA.Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KVA.Cinema.Controllers
{
    public class CountriesController : Controller
    {
        private static Breadcrumb homeBreadcrumb;
        private static Breadcrumb indexBreadcrumb;
        private static Breadcrumb detailsBreadcrumb;
        private static Breadcrumb createBreadcrumb;
        private static Breadcrumb editBreadcrumb;
        private static Breadcrumb deleteBreadcrumb;

        private CountryService CountryService { get; }

        public CountriesController(CountryService countryService)
        {
            CountryService = countryService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            homeBreadcrumb = new Breadcrumb { Title = "Home", Url = Url.Action("Index", "Home") };
            indexBreadcrumb = new Breadcrumb { Title = "Countries", Url = Url.Action("Index", "Countries") };
            detailsBreadcrumb = new Breadcrumb { Title = "Details", Url = Url.Action("Details", "Countries") };
            createBreadcrumb = new Breadcrumb { Title = "Create", Url = Url.Action("Create", "Countries") };
            editBreadcrumb = new Breadcrumb { Title = "Edit", Url = Url.Action("Edit", "Countries") };
            deleteBreadcrumb = new Breadcrumb { Title = "Delete", Url = Url.Action("Delete", "Countries") };
        }

        // GET: Countries
        public IActionResult Index(CountrySort sortingField = CountrySort.Name, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);

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

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, detailsBreadcrumb);

            return View(country);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

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

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

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

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

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

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

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

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

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

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(Guid id)
        {
            return CountryService.IsEntityExist(id);
        }

        private void AddBreadcrumbs(params Breadcrumb[] breadcrumbs)
        {
            ViewBag.Breadcrumbs = new List<Breadcrumb>(breadcrumbs);
        }
    }
}
