using AvenueClothing.Feature.General.Module.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCommerce.EntitiesV2;
using UCommerce.Runtime;

namespace AvenueClothing.Project.Website.Controllers
{
    public class BreadcrumbController: Controller
    {
        public ActionResult Index()
        {
            IList<BreadcrumbViewModel> breadcrumbs = new List<BreadcrumbViewModel>();
            Category lastCategory = null;
            Product product = SiteContext.Current.CatalogContext.CurrentProduct;

            foreach (var category in SiteContext.Current.CatalogContext.CurrentCategories)
            {
                var breadcrumb = new BreadcrumbViewModel()
                {
                    BreadcrumbName = category.DisplayName(),
                    BreadcrumbUrl = CatalogLibrary.GetNiceUrlForCategory(category)
                };
                lastCategory = category;
                breadcrumbs.Add(breadcrumb);
            }

            if (product != null)
            {
                var breadcrumb = new BreadcrumbsViewModel()
                {
                    BreadcrumbName = product.DisplayName(),
                    BreadcrumbUrl = CatalogLibrary.GetNiceUrlForProduct(product, lastCategory)
                };
                breadcrumbs.Add(breadcrumb);
            }

            if (product == null && lastCategory == null)
            {
                var currentNode = UmbracoContext.Current.PublishedContentRequest.PublishedContent;
                foreach (var level in currentNode.Ancestors().Where("Visible"))
                {
                    var breadcrumb = new BreadcrumbsViewModel()
                    {
                        BreadcrumbName = level.Name,
                        BreadcrumbUrl = level.Url
                    };
                    breadcrumbs.Add(breadcrumb);
                }
                var currentBreadcrumb = new BreadcrumbsViewModel()
                {
                    BreadcrumbName = currentNode.Name,
                    BreadcrumbUrl = currentNode.Url
                };
                breadcrumbs.Add(currentBreadcrumb);
            }

            return View("/Views/PartialView/Breadcrumbs.cshtml", breadcrumbs);
        }
    }
}