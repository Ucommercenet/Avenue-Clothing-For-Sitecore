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
            IList<BreadcrumbViewModel> Breadcrumbs = new List<BreadcrumbViewModel>();
            Product product = SiteContext.Current.CatalogContext.CurrentProduct;
            var categories = SiteContext.Current.CatalogContext.CurrentCategories;

            IList<Item> items = GetBreadcrumbItems();
            //foreach (var category in SiteContext.Current.CatalogContext.CurrentCategories)
            //{
                foreach (Item item in items)
                {
                    if (!IsTemplateBlacklisted(item.TemplateName))
                    {                         
                        BreadcrumbViewModel crumb = new BreadcrumbViewModel(item);
                        if (!string.IsNullOrEmpty(crumb.BreadcrumbName))
                        {
                            //GetUcommerceUrlForItem(crumb, category);
                            Breadcrumbs.Add(crumb);
                        }
                    }
                }
            
            Breadcrumbs.Add(new BreadcrumbViewModel(Sitecore.Context.Item));

            return View("/Views/Breadcrumb.cshtml", Breadcrumbs);
        }

        private bool IsTemplateBlacklisted(string templateName) {
            if (templateName.Equals("ProductCatalogTemplate") || 
                templateName.Equals("ProductCatalogGroupBaseTemplate")) {
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
        }
        private IList<Item> GetUcommerceBreadcrumbItems()
        {
            string homePath = Sitecore.Context.Site.StartPath;
            Item homeItem = Sitecore.Context.Database.GetItem(homePath);
            List<Item> items = new List<Item>();

            Item item = RenderingContext.Current.Rendering.Item;
            string url = Sitecore.Links.LinkManager.GetItemUrl(item);
            foreach (var crumb in item.Axes.GetAncestors())
            {
                items.Add(crumb);
            }

            return items;
        }

        private void GetUcommerceUrlForItem(BreadcrumbViewModel crumb, Category category) {
            crumb.UcommerceBreadcrumbUrl = CatalogLibrary.GetNiceUrlForCategory(category);
        }
    } }