using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;

namespace AvenueClothing.Project.Catalog.Controllers
{
    public class ProductPageController: BaseController
    {
        public ActionResult Rendering()
        {
            return View();
        }

    }
}