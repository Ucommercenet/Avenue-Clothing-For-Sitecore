using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AvenueClothing.Feature.Catalog.Controllers
{
    public class FrontpageProductsController: Controller
    {
        public ActionResult FrontpageProducts()
        {

            return View("~/views/FrontpageProducts.cshtml");
        }
    }
}