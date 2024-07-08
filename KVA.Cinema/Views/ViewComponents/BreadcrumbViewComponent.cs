using KVA.Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace KVA.Cinema.Views.ViewComponents
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<Breadcrumb> breadcrumbs)
        {
            return View(breadcrumbs);
        }
    }
}
