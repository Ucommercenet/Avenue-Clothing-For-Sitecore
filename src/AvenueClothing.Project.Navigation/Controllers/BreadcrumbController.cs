using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Project.Navigation.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using Sitecore.Data.Items;
using Sitecore.Links;
using Ucommerce.Api;
using Ucommerce.Extensions;
using Ucommerce.Search.Slugs;

namespace AvenueClothing.Project.Navigation.Controllers
{
    public class BreadcrumbController : BaseController
    {
        private readonly ICatalogContext _catalogContext;
        private readonly IUrlService _urlService;

        public BreadcrumbController(ICatalogContext catalogContext, IUrlService urlService)
        {
            _catalogContext = catalogContext;
            _urlService = urlService;
        }

        public ActionResult Rendering()
        {
            BreadcrumbWrapper breadcrumbs = new BreadcrumbWrapper();
            IList<Item> items = GetBreadcrumbItems();
            foreach (Item item in items)
            {
                if (!IsTemplateBlacklisted(item.TemplateName))
                {
                    BreadcrumbViewModel crumb = new BreadcrumbViewModel(item);
                    crumb.BreadcrumbName = item["Name"];
                    crumb.IsActive = Sitecore.Context.Item.ID == item.ID;
                    crumb.BreadcrumbUrl = LinkManager.GetItemUrl(item);
                    breadcrumbs.SitecoreBreadcrumbs.Add(crumb);
                }
            }

            var product = _catalogContext.CurrentProduct;

            var lastCategory = _catalogContext.CurrentCategory;
            foreach (var category in _catalogContext.CurrentCategories)
            {
                BreadcrumbViewModelUcommerce crumb = new BreadcrumbViewModelUcommerce
                {
                    BreadcrumbNameUcommerce = category.DisplayName,
                    BreadcrumbUrlUcommerce = _urlService.GetUrl(_catalogContext.CurrentCatalog, new []{category}.Compact().ToArray())
                };
                lastCategory = category;
                breadcrumbs.UcommerceBreadcrumbs.Add(crumb);
            }

            if (product != null)
            {
                var breadcrumb = new BreadcrumbViewModelUcommerce
                {
                    BreadcrumbNameUcommerce = product.DisplayName,
                    BreadcrumbUrlUcommerce = _urlService.GetUrl(_catalogContext.CurrentCatalog, new []{ lastCategory }.Compact().ToArray(), product)
                };
                breadcrumbs.UcommerceBreadcrumbs.Add(breadcrumb);
            }

            if (IsTemplateWhitelisted(Sitecore.Context.Item.TemplateName))
            {
                BreadcrumbViewModelUcommerce currentCrumb = new BreadcrumbViewModelUcommerce();
                currentCrumb.BreadcrumbNameUcommerce = Sitecore.Context.Item.DisplayName;
                currentCrumb.BreadcrumbUrlUcommerce = LinkManager.GetItemUrl(Sitecore.Context.Item);
                breadcrumbs.UcommerceBreadcrumbs.Add(currentCrumb);
            }


            return View(breadcrumbs);
        }

        private bool IsTemplateWhitelisted(string templateName)
        {
            if(templateName.Equals("Content Page") ||
               templateName.Equals("Confirmation"))
            {
                return true;
            }
            return false;
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

            List<Item> items = Sitecore.Context.Item.Axes.GetAncestors()
              .SkipWhile(item => item.ID != homeItem.ID).Where(x => x.ID != homeItem.ID)
              .ToList();
            items.Add(homeItem);
            return items;
        }
    }
}