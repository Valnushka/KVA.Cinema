using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using KVA.Cinema.Services;
using KVA.Cinema.Models.ViewModels.Video;

namespace KVA.Cinema.Controllers
{
    public class VideosController : Controller
    {
        private VideoService VideoService { get; }

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
                                TagService tagService)
        {
            VideoService = videoService;
            CountryService = countryService;
            DirectorService = directorService;
            LanguageService = languageService;
            PegiService = pegiService;
            GenreService = genreService;
            TagService = tagService;
        }

        // GET: Videos
        public IActionResult Index(VideoSort sortingField = VideoSort.Name, bool isSortDescending = false)
        {
            ViewBag.SortingField = sortingField;
            ViewBag.SortDescending = isSortDescending;

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
                case VideoSort.Genre:
                    videos = isSortDescending ? videos.OrderByDescending(s => s.GenreNames) : videos.OrderBy(s => s.GenreNames);
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

            var video = VideoService.ReadAll()
                .FirstOrDefault(m => m.Id == id);

            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        // GET: Videos/Create
        public IActionResult Create()
        {
            ViewBag.CountryId = new SelectList(CountryService.ReadAll(), "Id", "Name");
            ViewBag.DirectorId = new SelectList(DirectorService.ReadAll(), "Id", "Name");
            ViewBag.LanguageId = new SelectList(LanguageService.ReadAll(), "Id", "Name");
            ViewBag.PegiId = new SelectList(PegiService.ReadAll(), "Id", "Type");
            ViewBag.GenreIds = new SelectList(GenreService.ReadAll(), "Id", "Title");
            ViewBag.TagIds = new SelectList(TagService.ReadAll(), "Id", "Text");
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

            // TODO: При каждой ошибки валидации считывается 5 раз наборы сущностей
            ViewBag.CountryId = new SelectList(CountryService.ReadAll(), "Id", "Name");
            ViewBag.DirectorId = new SelectList(DirectorService.ReadAll(), "Id", "Name");
            ViewBag.LanguageId = new SelectList(LanguageService.ReadAll(), "Id", "Name");
            ViewBag.PegiId = new SelectList(PegiService.ReadAll(), "Id", "Type");
            ViewBag.GenreIds = new SelectList(GenreService.ReadAll(), "Id", "Title");
            ViewBag.TagIds = new SelectList(TagService.ReadAll(), "Id", "Text");

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

            ViewBag.CountryId = new SelectList(CountryService.ReadAll(), "Id", "Name");
            ViewBag.DirectorId = new SelectList(DirectorService.ReadAll(), "Id", "Name");
            ViewBag.LanguageId = new SelectList(LanguageService.ReadAll(), "Id", "Name");
            ViewBag.PegiId = new SelectList(PegiService.ReadAll(), "Id", "Type");
            ViewBag.GenreIds = new SelectList(GenreService.ReadAll(), "Id", "Title");
            ViewBag.TagIds = new SelectList(TagService.ReadAll(), "Id", "Text");

            var videoEditModel = new VideoEditViewModel()
            {
                Id = video.Id,
                Name = video.Name,
                Description = video.Description,
                Length = video.Length,
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

            ViewBag.CountryId = new SelectList(CountryService.ReadAll(), "Id", "Name");
            ViewBag.DirectorId = new SelectList(DirectorService.ReadAll(), "Id", "Name");
            ViewBag.LanguageId = new SelectList(LanguageService.ReadAll(), "Id", "Name");
            ViewBag.PegiId = new SelectList(PegiService.ReadAll(), "Id", "Type");
            ViewBag.GenreIds = new SelectList(GenreService.ReadAll(), "Id", "Title");
            ViewBag.TagIds = new SelectList(TagService.ReadAll(), "Id", "Text");

            return View(videoNewData);
        }

        // GET: Videos/Delete/5
        public IActionResult Delete(Guid? id)
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

            return View(video);
        }

        // POST: Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var video = VideoService.ReadAll()
                .FirstOrDefault(m => m.Id == id);
            VideoService.Delete(video.Id);

            return RedirectToAction(nameof(Index));
        }

        private bool VideoExists(Guid id)
        {
            return VideoService.IsEntityExist(id);
        }
    }
}
