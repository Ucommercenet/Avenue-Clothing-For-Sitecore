using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Catalog.ViewModels;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace AvenueClothing.Project.Catalog.Controllers
{
    public class ProductTitleController : BaseController
    {
        public ActionResult Rendering()
        {
            var productPrimaryTitleViewModel = new ProductTitleViewModel();

            productPrimaryTitleViewModel.ProductTitle = new HtmlString(FieldRenderer.Render(RenderingContext.Current.Rendering.Item, "Display name"));

            return View(productPrimaryTitleViewModel);
        }
    
    }
}