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
using KVA.Cinema.Models.ViewModels.Tag;

namespace KVA.Cinema.Controllers
{
    public class TagsController : Controller
    {
        private TagService TagService { get; }

        public TagsController(TagService tagService)
        {
            TagService = tagService;
        }

        // GET: Tags
        public IActionResult Index(TagSort sortingField = TagSort.Text, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            var tags = TagService.ReadAll();

            if (sortingField == TagSort.Text && isSortDescending)
            {
                tags = tags.OrderByDescending(s => s.Text);
            }
            else
            {
                tags = tags.OrderBy(s => s.Text);
            }

            return View(tags.ToList());
        }

        // GET: Tags/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = TagService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TagCreateViewModel tagData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TagService.CreateAsync(tagData);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(tagData);
        }

        // GET: Tags/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = TagService.ReadAll()
                            .FirstOrDefault(m => m.Id == id);

            if (tag == null)
            {
                return NotFound();
            }

            var tagEditModel = new TagEditViewModel()
            {
                Id = tag.Id,
                Text = tag.Text,
                Color = tag.Color
            };

            return View(tagEditModel);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, TagEditViewModel tagNewData)
        {
            if (id != tagNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TagService.Update(id, tagNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(tagNewData);
        }

        // GET: Tags/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = TagService.ReadAll()
                 .FirstOrDefault(m => m.Id == id);

            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var tag = TagService.ReadAll()
                 .FirstOrDefault(m => m.Id == id);
            TagService.Delete(tag.Id);

            return RedirectToAction(nameof(Index));
        }

        private bool TagExists(Guid id)
        {
            return TagService.IsEntityExist(id);
        }
    }
}
