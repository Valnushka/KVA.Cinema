using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KVA.Cinema.ViewModels
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }

        public int PagesTotal { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int itemsOnPage)
        {
            PageIndex = pageIndex;
            PagesTotal = (int)Math.Ceiling(count / (double)itemsOnPage);

            this.AddRange(items);
        }

        public bool HasPreviousPage() => (PageIndex > 1);

        public bool HasNextPage() => (PageIndex < PagesTotal);

        public static PaginatedList<T> CreateAsync(IEnumerable<T> source, int pageIndex, int itemsOnPage)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * itemsOnPage).Take(itemsOnPage).ToList();
            return new PaginatedList<T>(items, count, pageIndex, itemsOnPage);
        }
    }
}
