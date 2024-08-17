using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KVA.Cinema.Services;
using KVA.Cinema.ViewModels;
using KVA.Cinema.Utilities;
using KVA.Cinema.Entities;
using System.Collections.Generic;

namespace KVA.Cinema.Controllers
{
    public class VideosController : BaseController<Video, VideoCreateViewModel, VideoDisplayViewModel, VideoEditViewModel, VideoService>
    {
        protected override string ModuleCaption { get { return "Videos"; } }

        private CountryService CountryService { get; }

        private DirectorService DirectorService { get; }

        private LanguageService LanguageService { get; }

        private PegiService PegiService { get; }

        private GenreService GenreService { get; }

        private TagService TagService { get; }

        public VideosController(VideoService videoService,
                                CountryService countryService,
                                DirectorService directorService,
                                LanguageService languageService,
                                PegiService pegiService,
                                GenreService genreService,
                                TagService tagService,
                                CacheManager memoryCache) : base(videoService, memoryCache)
        {
            CountryService = countryService;
            DirectorService = directorService;
            LanguageService = languageService;
            PegiService = pegiService;
            GenreService = genreService;
            TagService = tagService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override IActionResult Create(VideoCreateViewModel videoData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    EntityService.Create(videoData);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    ModelState.AddModelError(string.Empty, "Please upload preview again"); //TODO: get read of this issue to use only base method
                }
            }

            GetCachedEntities();

            AddCreateCrumbs();

            return View(videoData);
        }

        protected override void GetCachedEntities()
        {
            ViewBag.CountryId = CacheManager.GetCachedSelectList("CountriesSelectedList", CountryService.ReadAll, nameof(CountryDisplayViewModel.Id), nameof(CountryDisplayViewModel.Name));
            ViewBag.DirectorId = CacheManager.GetCachedSelectList("DirectorsSelectedList", DirectorService.ReadAll, nameof(DirectorDisplayViewModel.Id), nameof(DirectorDisplayViewModel.Name));
            ViewBag.LanguageId = CacheManager.GetCachedSelectList("LanguagesSelectedList", LanguageService.ReadAll, nameof(LanguageDisplayViewModel.Id), nameof(DirectorDisplayViewModel.Name));
            ViewBag.PegiId = CacheManager.GetCachedSelectList("PegisSelectedList", PegiService.ReadAll, nameof(PegiDisplayViewModel.Id), nameof(PegiDisplayViewModel.Type));
            ViewBag.GenreIds = CacheManager.GetCachedSelectList("GenresSelectedList", GenreService.ReadAll, nameof(GenreDisplayViewModel.Id), nameof(GenreDisplayViewModel.Title));
            ViewBag.TagIds = CacheManager.GetCachedSelectList("TagsSelectedList", TagService.ReadAll, nameof(TagDisplayViewModel.Id), nameof(TagDisplayViewModel.Text));
        }

        protected override VideoEditViewModel MapToEditViewModel(VideoDisplayViewModel displayViewModel)
        {
            return new VideoEditViewModel()
            {
                Id = displayViewModel.Id,
                Name = displayViewModel.Name,
                Description = displayViewModel.Description,
                Length = 1,
                CountryId = displayViewModel.CountryId,
                ReleasedIn = displayViewModel.ReleasedIn,
                Views = displayViewModel.Views,
                Preview = displayViewModel.Preview,
                PreviewFileName = displayViewModel.PreviewFileName,
                PegiId = displayViewModel.PegiId,
                LanguageId = displayViewModel.LanguageId,
                DirectorId = displayViewModel.DirectorId,
                GenreIds = displayViewModel.Genres.Select(x => x.Id),
                TagIds = displayViewModel.Tags.Select(x => x.Id)
            };
        }

        protected override IEnumerable<VideoDisplayViewModel> GetFilterResult(IEnumerable<VideoDisplayViewModel> videos, string query)
        {
            query = query.ToLower();

            return videos.Where(x => x.Name.Contains(query));
        }

        protected override IEnumerable<VideoDisplayViewModel> Sort(IEnumerable<VideoDisplayViewModel> videos, string sortColumn, bool isSortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortColumn)
                || !Enum.TryParse(sortColumn, out VideoSort parsedSortColumn))
            {
                parsedSortColumn = VideoSort.Name;
            }

            switch (parsedSortColumn)
            {
                case VideoSort.Name:
                    return isSortDescending ? videos.OrderByDescending(s => s.Name) : videos.OrderBy(s => s.Name);
                case VideoSort.ReleasedIn:
                    return isSortDescending ? videos.OrderByDescending(s => s.ReleasedIn) : videos.OrderBy(s => s.ReleasedIn);
                case VideoSort.Language:
                    return isSortDescending ? videos.OrderByDescending(s => s.LanguageName) : videos.OrderBy(s => s.LanguageName);
                default:
                    return videos.OrderBy(s => s.Name);
            }
        }
    }
}
