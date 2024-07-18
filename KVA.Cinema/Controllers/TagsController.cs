using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KVA.Cinema.Services;
using KVA.Cinema.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KVA.Cinema.Controllers
{
    public class TagsController : Controller
    {
        private static Breadcrumb homeBreadcrumb;
        private static Breadcrumb indexBreadcrumb;
        private static Breadcrumb detailsBreadcrumb;
        private static Breadcrumb createBreadcrumb;
        private static Breadcrumb editBreadcrumb;
        private static Breadcrumb deleteBreadcrumb;

        private TagService TagService { get; }

        public TagsController(TagService tagService)
        {
            TagService = tagService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            homeBreadcrumb = new Breadcrumb { Title = "Home", Url = Url.Action("Index", "Home") };
            indexBreadcrumb = new Breadcrumb { Title = "Tags", Url = Url.Action("Index", "Tags") };
            detailsBreadcrumb = new Breadcrumb { Title = "Details", Url = Url.Action("Details", "Tags") };
            createBreadcrumb = new Breadcrumb { Title = "Create", Url = Url.Action("Create", "Tags") };
            editBreadcrumb = new Breadcrumb { Title = "Edit", Url = Url.Action("Edit", "Tags") };
            deleteBreadcrumb = new Breadcrumb { Title = "Delete", Url = Url.Action("Delete", "Tags") };
        }

        // GET: Tags
        [Route("Tags")]
        public IActionResult Index(int? pageNumber,
                                   string searchString,
                                   TagSort sortingField = TagSort.Text,
                                   bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;
            ViewBag.CurrentFilter = searchString;

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);

            var tags = TagService.ReadAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                tags = tags.Where(x => x.Text.Contains(searchString));
            }

            if (sortingField == TagSort.Text && isSortDescending)
            {
                tags = tags.OrderByDescending(s => s.Text);
            }
            else
            {
                tags = tags.OrderBy(s => s.Text);
            }

            int itemsOnPage = 15;

            return View(PaginatedList<TagDisplayViewModel>.CreateAsync(tags, pageNumber ?? 1, itemsOnPage));
        }

        // GET: Tags/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TagDisplayViewModel tag = null;

            try
            {
                tag = TagService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (tag == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, detailsBreadcrumb);

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

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
                    TagService.Create(tagData);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View(tagData);
        }

        // GET: Tags/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TagDisplayViewModel tag = null;

            try
            {
                tag = TagService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

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

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

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

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

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

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            TagDisplayViewModel tag = null;

            try
            {
                tag = TagService.Read(id);
                TagService.Delete(tag.Id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return RedirectToAction(nameof(Index));
        }

        private void AddBreadcrumbs(params Breadcrumb[] breadcrumbs)
        {
            ViewBag.Breadcrumbs = new List<Breadcrumb>(breadcrumbs);
        }
    }
}
