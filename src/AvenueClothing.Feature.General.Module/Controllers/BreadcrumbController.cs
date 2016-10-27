using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Feature.General.Module.ViewModels;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Runtime;

namespace AvenueClothing.Feature.General.Module.Controllers
{
    public class BreadcrumbController : SitecoreController
    {
        public ActionResult Rendering()
        {
            BreadcrumbWrapper breadcrumbs = new BreadcrumbWrapper();
            IList<Item> items = GetBreadcrumbItems();
            foreach (Item item in items)
            {
                if (!IsTemplateBlacklisted(item.TemplateName))
                {
                    BreadcrumbViewModel crumb = new BreadcrumbViewModel(item);
                    breadcrumbs.SitecoreBreadcrumbs.Add(crumb);
                }
            }

            Product product = SiteContext.Current.CatalogContext.CurrentProduct;

            Category lastCategory = SiteContext.Current.CatalogContext.CurrentCategory;
            foreach (var category in SiteContext.Current.CatalogContext.CurrentCategories)
            {
                BreadcrumbViewModelUcommerce crumb = new BreadcrumbViewModelUcommerce()
                {
                    BreadcrumbNameUcommerce = category.DisplayName(),
                    BreadcrumbUrlUcommerce = CatalogLibrary.GetNiceUrlForCategory(category)
                };
                lastCategory = category;
                breadcrumbs.UcommerceBreadcrumbs.Add(crumb);
            }

            if (product != null)
            {
                var breadcrumb = new BreadcrumbViewModelUcommerce()
                {
                    BreadcrumbNameUcommerce = product.DisplayName(),
                    BreadcrumbUrlUcommerce = CatalogLibrary.GetNiceUrlForProduct(product, lastCategory)
                };
                breadcrumbs.UcommerceBreadcrumbs.Add(breadcrumb);
            }
            return View(breadcrumbs);
        }

        private bool IsTemplateBlacklisted(string templateName)
        {
            if (templateName.Equals("ProductCatalogTemplate") ||
                templateName.Equals("ProductCatalogGroupBaseTemplate") ||
                templateName.Equals("uCommerce stores Template") ||
                templateName.Equals("Root") ||
                templateName.Equals("uCommerceTemplate")
                )
            {
                return true;
            }
            return false;
        }

        private IList<Item> GetBreadcrumbItems()
        {
            string homePath = Sitecore.Context.Site.StartPath;
            Item homeItem = Sitecore.Context.Database.GetItem(homePath);
            //but what if we have a mixture of these?
            List<Item> items = Sitecore.Context.Item.Axes.GetAncestors()
              .SkipWhile(item => item.ID != homeItem.ID).Where(x => x.ID != homeItem.ID)
              .ToList();
            items.Add(homeItem);
            return items;
        }
    }
}