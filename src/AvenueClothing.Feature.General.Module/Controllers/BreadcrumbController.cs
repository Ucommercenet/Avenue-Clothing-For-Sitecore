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
using UCommerce.Api;

namespace AvenueClothing.Project.Website.Controllers
{
    public class BreadcrumbController : Controller
    {
        public ActionResult Index()
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
            var categories = SiteContext.Current.CatalogContext.CurrentCategories;
            IList<Item> uCommerceItems = GetUcommerceBreadcrumbItems();

            foreach (var category in SiteContext.Current.CatalogContext.CurrentCategories)
            {
                BreadcrumbViewModelUcommerce crumb = new BreadcrumbViewModelUcommerce()
                {
                    BreadcrumbNameUcommerce = category.DisplayName(),
                    BreadcrumbUrlUcommerce = CatalogLibrary.GetNiceUrlForCategory(category)
                };
               
                breadcrumbs.UcommerceBreadcrumbs.Add(crumb);
            }

            //breadcrumbs.SitecoreBreadcrumbs.Add(new BreadcrumbViewModel(Sitecore.Context.Item));

            return View("/Views/Breadcrumb.cshtml", breadcrumbs);
        }

        private bool IsTemplateBlacklisted(string templateName) {
            if (templateName.Equals("ProductCatalogTemplate") || 
                templateName.Equals("ProductCatalogGroupBaseTemplate") ||
                templateName.Equals("uCommerce stores Template") ||
                templateName.Equals("Root") ||
                templateName.Equals("uCommerceTemplate")
                ) {
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
              .SkipWhile(item => item.ID != homeItem.ID)
              .ToList();
            items.Add(homeItem);
            return items;
        }
        private IList<Item> GetUcommerceBreadcrumbItems()
        {
            IList<Item> uCommerceItems = new List<Item>();
                Item item = RenderingContext.Current.Rendering.Item;
                string url = Sitecore.Links.LinkManager.GetItemUrl(item);
                foreach (var crumb in item.Axes.GetAncestors())
                {
                uCommerceItems.Add(crumb);
                }
            return uCommerceItems;
        }

        private void GetUcommerceUrlForItem(BreadcrumbViewModel crumb, Category category) {
            crumb.UcommerceBreadcrumbUrl = CatalogLibrary.GetNiceUrlForCategory(category);
        }
    } }