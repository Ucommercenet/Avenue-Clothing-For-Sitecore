using AvenueClothing.Feature.General.Module.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Runtime;
using Sitecore.Mvc.Presentation;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;

namespace AvenueClothing.Project.Website.Controllers
{
    public class BreadcrumbController: Controller
    {
        public ActionResult Index()
        {
            IList<BreadcrumbViewModel> Breadcrumbs = new List<BreadcrumbViewModel>();
            IList<Item> items = GetBreadcrumbItems();

            foreach (Item item in items)
            {
                BreadcrumbViewModel crumb = new BreadcrumbViewModel(item);
                if (!string.IsNullOrEmpty(crumb.BreadcrumbName))
                {
                    Breadcrumbs.Add(crumb);
                }
            }
            Breadcrumbs.Add(new BreadcrumbViewModel(Sitecore.Context.Item));

            return View("Viewvs/BreadCrumb", Breadcrumbs);
        }

        private IList<Item> GetBreadcrumbItems()
        {
            string homePath = Sitecore.Context.Site.StartPath;
            Item homeItem = Sitecore.Context.Database.GetItem(homePath);

            List<Item> items = Sitecore.Context.Item.Axes.GetAncestors()
              .SkipWhile(item => item.ID != homeItem.ID)
              .ToList();
            //If the content tree is empty, it is a uCommerce products
            if (items.Count <= 0)
            {
                Item item = RenderingContext.Current.Rendering.Item;
                string url = Sitecore.Links.LinkManager.GetItemUrl(item);
                foreach (var crumb in item.Axes.GetAncestors())
                {
                    items.Add(crumb);
                }
            }
            return items;


            //    public ActionResult Index()
            //    {
            //        IList<BreadcrumbViewModel> breadcrumbs = new List<BreadcrumbViewModel>();
            //        Category lastCategory = null;
            //        Product product = SiteContext.Current.CatalogContext.CurrentProduct;

            //        foreach (var category in SiteContext.Current.CatalogContext.CurrentCategories)
            //        {
            //            var breadcrumb = new BreadcrumbViewModel()
            //            {
            //                BreadcrumbName = category.DisplayName(),
            //                BreadcrumbUrl = CatalogLibrary.GetNiceUrlForCategory(category)
            //            };
            //            lastCategory = category;
            //            breadcrumbs.Add(breadcrumb);
            //        }

            //        if (product != null)
            //        {
            //            var breadcrumb = new BreadcrumbsViewModel()
            //            {
            //                BreadcrumbName = product.DisplayName(),
            //                BreadcrumbUrl = UCommerce.Api.CatalogLibrary.GetNiceUrlForProduct(product, lastCategory)
            //            };
            //            breadcrumbs.Add(breadcrumb);
            //        }

            //        if (product == null && lastCategory == null)
            //        {
            //            var currentNode = UmbracoContext.Current.PublishedContentRequest.PublishedContent;
            //            foreach (var level in currentNode.Ancestors().Where("Visible"))
            //            {
            //                var breadcrumb = new BreadcrumbViewModel()
            //                {
            //                    BreadcrumbName = level.Name,
            //                    BreadcrumbUrl = level.Url
            //                };
            //                breadcrumbs.Add(breadcrumb);
            //            }
            //            var currentBreadcrumb = new BreadcrumbViewModel()
            //            {
            //                BreadcrumbName = currentNode.Name,
            //                BreadcrumbUrl = currentNode.Url
            //            };
            //            breadcrumbs.Add(currentBreadcrumb);
            //        }

            //        return View("/Views/PartialView/Breadcrumbs.cshtml", breadcrumbs);
            //    }
            //}
        }