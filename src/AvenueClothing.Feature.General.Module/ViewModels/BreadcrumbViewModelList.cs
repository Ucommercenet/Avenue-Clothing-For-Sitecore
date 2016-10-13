using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;

namespace AvenueClothing.Feature.General.Module.ViewModels
{
    public class BreadcrumbViewModelList : Sitecore.Mvc.Presentation.RenderingModel
    {
        public List<BreadcrumbViewModel> Breadcrumbs { get; set; }
        public override void Initialize(Rendering rendering)
        {
            Breadcrumbs = new List<BreadcrumbViewModel>();
            List<Item> items = GetBreadcrumbItems();
            foreach (Item item in items)
            {
                Breadcrumbs.Add(new BreadcrumbViewModel(item));
            }
            Breadcrumbs.Add(new BreadcrumbViewModel(Sitecore.Context.Item));
        }
        private List<Item> GetBreadcrumbItems()
        {
            string homePath = Sitecore.Context.Site.StartPath;
            Item homeItem = Sitecore.Context.Database.GetItem(homePath);
            List<Item> items = Sitecore.Context.Item.Axes.GetAncestors()
              .SkipWhile(item => item.ID != homeItem.ID)
              .ToList();
            return items;
        }
    }
}