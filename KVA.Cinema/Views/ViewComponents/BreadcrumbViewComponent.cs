namespace KVA.Cinema.Views.ViewComponents
{
    using KVA.Cinema.Models.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class BreadcrumbViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<Breadcrumb> breadcrumbs)
        {
            return View(breadcrumbs);
        }
    }
}
