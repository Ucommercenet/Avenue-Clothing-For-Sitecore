﻿using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;

namespace AvenueClothing.Feature.Catalog.Controllers
{
    public class ProductPageController: BaseController
    {
        public ActionResult Rendering()
        {
            return View();
        }

    }
}