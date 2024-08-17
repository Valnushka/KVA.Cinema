namespace KVA.Cinema.Controllers
{
    using KVA.Cinema.Models.Entities;
    using KVA.Cinema.Services;
    using KVA.Cinema.Utilities;
    using KVA.Cinema.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.Collections.Generic;

    public abstract class BaseController<TEntity, TEntityCreateViewModel, TEntityDisplayViewModel, TEntityEditViewModel, TService> : Controller
        where TEntity : class, IEntity
        where TEntityCreateViewModel : class, IViewModel
        where TEntityDisplayViewModel : class, IViewModel
        where TEntityEditViewModel : class, IViewModel
        where TService : BaseService<TEntity, TEntityCreateViewModel, TEntityDisplayViewModel, TEntityEditViewModel>
    {
        protected static Breadcrumb homeBreadcrumb;
        protected static Breadcrumb indexBreadcrumb;
        protected static Breadcrumb detailsBreadcrumb;
        protected static Breadcrumb createBreadcrumb;
        protected static Breadcrumb editBreadcrumb;
        protected static Breadcrumb deleteBreadcrumb;

        protected abstract string ModuleCaption { get; }

        protected virtual string CacheKeyCaption { get; }

        protected CacheManager CacheManager { get; init; }

        protected TService EntityService { get; init; }

        public BaseController(TService entityService, CacheManager cacheManager)
        {
            EntityService = entityService;
            CacheManager = cacheManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (!IsBreadCrumbsInitialized())
            {
                InitBreadCrumbs();
            }
        }

        protected virtual void InitBreadCrumbs()
        {
            homeBreadcrumb = new Breadcrumb { Title = "Home", Url = Url.Action("Index", "Home") };
            indexBreadcrumb = new Breadcrumb { Title = ModuleCaption, Url = Url.Action("Index") };
            detailsBreadcrumb = new Breadcrumb { Title = "Details", Url = Url.Action("Details") };
            createBreadcrumb = new Breadcrumb { Title = "Create", Url = Url.Action("Create") };
            editBreadcrumb = new Breadcrumb { Title = "Edit", Url = Url.Action("Edit") };
            deleteBreadcrumb = new Breadcrumb { Title = "Delete", Url = Url.Action("Delete") };
        }

        protected void AddIndexCrumbs()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);
        }

        protected void AddDetailCrumbs()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, detailsBreadcrumb);
        }

        protected void AddCreateCrumbs()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);
        }

        protected void AddEditCrumbs()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);
        }

        protected void AddDeleteCrumbs()
        {
            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);
        }

        private static bool IsBreadCrumbsInitialized()
        {
            return homeBreadcrumb != default;
        }

        [HttpGet, HttpPost]
        public IActionResult Index(int? pageNumber, string searchString, string sortColumn, bool isSortDescending = false)
        {
            return Index(
                new PaginationConfig { Page = pageNumber, ItemsOnPage = 3 },
                new SortConfig { IsDescending = isSortDescending, SortColumn = sortColumn },
                new FilterConfig { Query = searchString }
            );
        }

        protected virtual IActionResult Index(PaginationConfig paginationConfig, SortConfig sortConfig, FilterConfig filterConfig)
        {
            FillViewBag(sortConfig, filterConfig);

            AddIndexCrumbs();

            var entities = EntityService.ReadAll();

            if (!string.IsNullOrEmpty(filterConfig?.Query))
            {
                entities = GetFilterResult(entities, filterConfig?.Query);
            }

            entities = Sort(entities, sortConfig.SortColumn, sortConfig.IsDescending);

            return View(
                PaginatedList<TEntityDisplayViewModel>.CreateAsync(
                    entities,
                    paginationConfig?.Page ?? 1,
                    paginationConfig?.ItemsOnPage ?? 15
                    )
                );
        }

        protected virtual void FillViewBag(SortConfig sortConfig, FilterConfig filterConfig)
        {
            ViewBag.SortColumn = sortConfig.SortColumn;
            ViewBag.IsSortDescending = sortConfig?.IsDescending ?? false;
            ViewBag.CurrentFilter = filterConfig?.Query;
        }

        protected abstract IEnumerable<TEntityDisplayViewModel> GetFilterResult(IEnumerable<TEntityDisplayViewModel> entities, string query);

        protected abstract IEnumerable<TEntityDisplayViewModel> Sort(IEnumerable<TEntityDisplayViewModel> entities, string sortColumn, bool isSortDescending);

        [HttpGet]
        public virtual IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TEntityDisplayViewModel entity = null;

            try
            {
                entity = EntityService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (entity == null)
            {
                return NotFound();
            }

            AddDetailCrumbs();

            return View(entity);
        }

        [HttpGet]
        public virtual IActionResult Create()
        {
            GetCachedEntities();

            AddCreateCrumbs();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual IActionResult Create(TEntityCreateViewModel entityData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    EntityService.Create(entityData);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            if (CacheKeyCaption != null)
            {
                CacheManager.RemoveFromCache(CacheKeyCaption);
            }

            GetCachedEntities();

            AddCreateCrumbs();

            return View(entityData);
        }

        [HttpGet]
        public virtual IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TEntityDisplayViewModel entity = null;

            try
            {
                entity = EntityService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            GetCachedEntities();

            AddEditCrumbs();

            TEntityEditViewModel userEditModel = MapToEditViewModel(entity);

            return View(userEditModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual IActionResult Edit(Guid id, TEntityEditViewModel entityNewData)
        {
            if (id != entityNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    EntityService.Update(id, entityNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            if (CacheKeyCaption != null)
            {
                CacheManager.RemoveFromCache(CacheKeyCaption);
            }

            GetCachedEntities();

            AddEditCrumbs();

            return View(entityNewData);
        }

        public virtual IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TEntityDisplayViewModel entity = null;

            try
            {
                entity = EntityService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (entity == null)
            {
                return NotFound();
            }

            AddDeleteCrumbs();

            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                TEntityDisplayViewModel entity = EntityService.Read(id);
                EntityService.Delete(entity.Id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (CacheKeyCaption != null)
            {
                CacheManager.RemoveFromCache(CacheKeyCaption);
            }

            AddDeleteCrumbs();

            return RedirectToAction(nameof(Index));
        }

        protected abstract TEntityEditViewModel MapToEditViewModel(TEntityDisplayViewModel displayViewModel);

        protected void AddBreadcrumbs(params Breadcrumb[] breadcrumbs)
        {
            ViewBag.Breadcrumbs = new List<Breadcrumb>(breadcrumbs);
        }

        protected virtual void GetCachedEntities() { }

    }
}
