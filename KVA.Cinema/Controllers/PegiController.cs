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
using KVA.Cinema.Models.ViewModels.Pegi;

namespace KVA.Cinema.Controllers
{
    public class PegiController : Controller
    {
        private PegiService PegiService { get; }

        public PegiController(PegiService pegiService)
        {
            PegiService = pegiService;
        }

        // GET: Pegi
        public IActionResult Index(PegiSort sortingField = PegiSort.Type, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

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

            return View(pegi);
        }

        // GET: Pegi/Create
        public IActionResult Create()
        {
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

            return RedirectToAction(nameof(Index));
        }
    }
}
