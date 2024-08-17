using System;
using System.Collections.Generic;
using System.Linq;
using KVA.Cinema.Services;
using KVA.Cinema.ViewModels;
using KVA.Cinema.Utilities;
using KVA.Cinema.Entities;

namespace KVA.Cinema.Controllers
{
    public class TagsController : BaseController<Tag, TagCreateViewModel, TagDisplayViewModel, TagEditViewModel, TagService>
    {
        protected override string ModuleCaption { get { return "Tags"; } }

        protected override string CacheKeyCaption { get { return "TagsSelectedList"; } }

        public TagsController(TagService tagService, CacheManager cacheManager) : base(tagService, cacheManager) { }

        protected override TagEditViewModel MapToEditViewModel(TagDisplayViewModel displayViewModel)
        {
            return new TagEditViewModel()
            {
                Id = displayViewModel.Id,
                Text = displayViewModel.Text,
                Color = displayViewModel.Color
            };
        }

        protected override IEnumerable<TagDisplayViewModel> GetFilterResult(IEnumerable<TagDisplayViewModel> tags, string query)
        {
            query = query.ToLower();

            return tags.Where(x => x.Text.ToLower().Contains(query));
        }

        protected override IEnumerable<TagDisplayViewModel> Sort(IEnumerable<TagDisplayViewModel> tags, string sortColumn, bool isSortDescending)
        {
            switch (sortColumn)
            {
                default:
                    return isSortDescending ? tags.OrderByDescending(x => x.Text) : tags.OrderBy(x => x.Text);
            }
        }
    }
}
