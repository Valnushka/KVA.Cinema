using System;
using System.Collections.Generic;
using KVA.Cinema.Services;
using KVA.Cinema.ViewModels;
using KVA.Cinema.Utilities;
using KVA.Cinema.Entities;
using System.Linq;

namespace KVA.Cinema.Controllers
{
    public class GenresController : BaseController<Genre, GenreCreateViewModel, GenreDisplayViewModel, GenreEditViewModel, GenreService>
    {
        protected override string ModuleCaption { get { return "Genres"; } }

        protected override string CacheKeyCaption { get { return "GenresSelectedList"; } }

        public GenresController(GenreService genreService, CacheManager cacheManager) : base(genreService, cacheManager) { }
       
        protected override GenreEditViewModel MapToEditViewModel(GenreDisplayViewModel displayViewModel)
        {
            return new GenreEditViewModel()
            {
                Id = displayViewModel.Id,
                Title = displayViewModel.Title
            };
        }

        protected override IEnumerable<GenreDisplayViewModel> GetFilterResult(IEnumerable<GenreDisplayViewModel> genres, string query)
        {
            query = query.ToLower();

            return genres.Where(x => x.Title.ToLower().Contains(query));
        }

        protected override IEnumerable<GenreDisplayViewModel> Sort(IEnumerable<GenreDisplayViewModel> genres, string sortColumn, bool isSortDescending)
        {
            switch (sortColumn)
            {
                default:
                    return isSortDescending ? genres.OrderByDescending(x => x.Title) : genres.OrderBy(x => x.Title);
            }
        }
    }
}
