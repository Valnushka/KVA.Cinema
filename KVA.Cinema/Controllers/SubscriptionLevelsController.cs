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
using KVA.Cinema.Models.ViewModels.SubscriptionLevel;

namespace KVA.Cinema.Controllers
{
    public class SubscriptionLevelsController : Controller
    {
        private SubscriptionLevelService SubscriptionLevelService { get; set; }

        public SubscriptionLevelsController(SubscriptionLevelService subscriptionLevelService)
        {
            SubscriptionLevelService = subscriptionLevelService;
        }

        // GET: SubscriptionLevels
        public IActionResult Index(SubscriptionLevelSort sortingField = SubscriptionLevelSort.Title, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            var subscriptionLevels = SubscriptionLevelService.ReadAll();

            if (sortingField == SubscriptionLevelSort.Title && isSortDescending)
            {
                subscriptionLevels = subscriptionLevels.OrderByDescending(s => s.Title);
            }
            else
            {
                subscriptionLevels = subscriptionLevels.OrderBy(s => s.Title);
            }

            return View(subscriptionLevels.ToList());
        }

        // GET: SubscriptionLevels/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionLevel = SubscriptionLevelService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (subscriptionLevel == null)
            {
                return NotFound();
            }

            return View(subscriptionLevel);
        }

        // GET: SubscriptionLevels/Create
        public IActionResult Create()
        {
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
            return View(subscriptionLevelData);
        }

        // GET: SubscriptionLevels/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionLevel = SubscriptionLevelService.ReadAll()
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
            return View(subscriptionLevelNewData);
        }

        // GET: SubscriptionLevels/Delete/5
        public IActionResult Delete(Guid? id)
        {
            var subscriptionLevel = SubscriptionLevelService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (subscriptionLevel == null)
            {
                return NotFound();
            }

            return View(subscriptionLevel);
        }

        // POST: SubscriptionLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var subscriptionLevel = SubscriptionLevelService.ReadAll()
                .FirstOrDefault(m => m.Id == id);
            SubscriptionLevelService.Delete(subscriptionLevel.Id);

            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionLevelExists(Guid id)
        {
            return SubscriptionLevelService.IsEntityExist(id);
        }
    }
}
