using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using KVA.Cinema.Services;
using KVA.Cinema.Models.ViewModels.Video;
using KVA.Cinema.Models.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using KVA.Cinema.Utilities;
using Microsoft.Extensions.Caching.Memory;
using KVA.Cinema.Models.Country;
using KVA.Cinema.Models.Director;
using KVA.Cinema.Models.ViewModels.Language;
using KVA.Cinema.Models.ViewModels.Pegi;
using KVA.Cinema.Models.Genre;
using KVA.Cinema.Models.ViewModels.Tag;

namespace KVA.Cinema.Controllers
{
    public class VideosController : Controller
    {
        private static Breadcrumb homeBreadcrumb;
        private static Breadcrumb indexBreadcrumb;
        private static Breadcrumb detailsBreadcrumb;
        private static Breadcrumb createBreadcrumb;
        private static Breadcrumb editBreadcrumb;
        private static Breadcrumb deleteBreadcrumb;

        private VideoService VideoService { get; }

        private CountryService CountryService { get; }

        private DirectorService DirectorService { get; }

        private LanguageService LanguageService { get; }

        private PegiService PegiService { get; }

        private GenreService GenreService { get; }

        private TagService TagService { get; }

        private CacheManager CacheManager { get; }

        public VideosController(VideoService videoService,
                                CountryService countryService,
                                DirectorService directorService,
                                LanguageService languageService,
                                PegiService pegiService,
                                GenreService genreService,
                                TagService tagService,
                                CacheManager memoryCache)
        {
            VideoService = videoService;
            CountryService = countryService;
            DirectorService = directorService;
            LanguageService = languageService;
            PegiService = pegiService;
            GenreService = genreService;
            TagService = tagService;
            CacheManager = memoryCache;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            homeBreadcrumb = new Breadcrumb { Title = "Home", Url = Url.Action("Index", "Home") };
            indexBreadcrumb = new Breadcrumb { Title = "Videos", Url = Url.Action("Index", "Videos") };
            detailsBreadcrumb = new Breadcrumb { Title = "Details", Url = Url.Action("Details", "Videos") };
            createBreadcrumb = new Breadcrumb { Title = "Create", Url = Url.Action("Create", "Videos") };
            editBreadcrumb = new Breadcrumb { Title = "Edit", Url = Url.Action("Edit", "Videos") };
            deleteBreadcrumb = new Breadcrumb { Title = "Delete", Url = Url.Action("Delete", "Videos") };
        }

        // GET: Videos
        [Route("Videos")]
        public IActionResult Index(VideoSort sortingField = VideoSort.Name, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);

            var videos = VideoService.ReadAll();

            switch (sortingField)
            {
                case VideoSort.Name:
                    videos = isSortDescending ? videos.OrderByDescending(s => s.Name) : videos.OrderBy(s => s.Name);
                    break;
                case VideoSort.ReleasedIn:
                    videos = isSortDescending ? videos.OrderByDescending(s => s.ReleasedIn) : videos.OrderBy(s => s.ReleasedIn);
                    break;
                case VideoSort.Language:
                    videos = isSortDescending ? videos.OrderByDescending(s => s.LanguageName) : videos.OrderBy(s => s.LanguageName);
                    break;
                default:
                    videos = videos.OrderBy(s => s.Name);
                    break;
            }

            return View(videos.ToList());
        }

        // GET: Videos/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            VideoDisplayViewModel video = null;

            try
            {
                video = VideoService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (video == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb);

            return View(video);
        }

        // GET: Videos/Create
        public IActionResult Create()
        {
            GetCachedEntities();

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View();
        }

        // POST: Videos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VideoCreateViewModel videoData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    VideoService.CreateAsync(videoData);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    ModelState.AddModelError(string.Empty, "Please upload preview again");
                }
            }

            GetCachedEntities();

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, createBreadcrumb);

            return View(videoData);
        }


        // GET: Videos/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = VideoService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (video == null)
            {
                return NotFound();
            }

            GetCachedEntities();

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            var videoEditModel = new VideoEditViewModel()
            {
                Id = video.Id,
                Name = video.Name,
                Description = video.Description,
                Length = 1,
                CountryId = video.CountryId,
                ReleasedIn = video.ReleasedIn,
                Views = video.Views,
                Preview = video.Preview,
                PreviewFileName = video.PreviewFileName,
                PegiId = video.PegiId,
                LanguageId = video.LanguageId,
                DirectorId = video.DirectorId,
                GenreIds = video.Genres.Select(x => x.Id),
                TagIds = video.Tags.Select(x => x.Id)
            };

            return View(videoEditModel);
        }

        // POST: Videos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, VideoEditViewModel videoNewData)
        {

            if (id != videoNewData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    VideoService.Update(id, videoNewData);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    ModelState.AddModelError(string.Empty, "Please upload preview again");
                }
            }

            GetCachedEntities();

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, editBreadcrumb);

            return View(videoNewData);
        }

        // GET: Videos/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            VideoDisplayViewModel video = null;

            try
            {
                video = VideoService.Read(id.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            if (video == null)
            {
                return NotFound();
            }

            AddBreadcrumbs(homeBreadcrumb, indexBreadcrumb, deleteBreadcrumb);

            return View(video);
        }

        // POST: Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                VideoDisplayViewModel video = VideoService.Read(id);
                VideoService.Delete(video.Id);
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

        private void GetCachedEntities()
        {
            ViewBag.CountryId = CacheManager.GetCachedSelectList("CountriesSelectedList", CountryService.ReadAll, nameof(CountryDisplayViewModel.Id), nameof(CountryDisplayViewModel.Name));
            ViewBag.DirectorId = CacheManager.GetCachedSelectList("DirectorsSelectedList", DirectorService.ReadAll, nameof(DirectorDisplayViewModel.Id), nameof(DirectorDisplayViewModel.Name));
            ViewBag.LanguageId = CacheManager.GetCachedSelectList("LanguagesSelectedList", LanguageService.ReadAll, nameof(LanguageDisplayViewModel.Id), nameof(DirectorDisplayViewModel.Name));
            ViewBag.PegiId = CacheManager.GetCachedSelectList("PegisSelectedList", PegiService.ReadAll, nameof(PegiDisplayViewModel.Id), nameof(PegiDisplayViewModel.Type));
            ViewBag.GenreIds = CacheManager.GetCachedSelectList("GenresSelectedList", GenreService.ReadAll, nameof(GenreDisplayViewModel.Id), nameof(GenreDisplayViewModel.Title));
            ViewBag.TagIds = CacheManager.GetCachedSelectList("TagsSelectedList", TagService.ReadAll, nameof(TagDisplayViewModel.Id), nameof(TagDisplayViewModel.Text));
        }
    }
}
