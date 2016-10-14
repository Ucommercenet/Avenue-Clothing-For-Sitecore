using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;

namespace AvenueClothing.Feature.General.Module.ViewModels
{
    public class BreadcrumbViewModelList : RenderingModel
    {
        public List<BreadcrumbViewModel> Breadcrumbs { get; set; }
        public List<Template> BlacklistedTemplates { get; set; }
        public override void Initialize(Rendering rendering)
        {
            Breadcrumbs = new List<BreadcrumbViewModel>();
        
        //controller material
        List<Item> items = GetBreadcrumbItems();
            foreach (Item item in items)
            {
                BreadcrumbViewModel crumb = new BreadcrumbViewModel(item);
                if (!string.IsNullOrEmpty(crumb.BreadcrumbName))
                {
                    Breadcrumbs.Add(crumb);
                }
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
//private List<Item> FilterBlackListedTemplates(List<Item> items)
//{
//    foreach (var item in items)
//    {
//        var template = TemplateManager.GetTemplate(item);
//        //t.GetBaseTemplates();
//        if (BlacklistedTemplates.Contains(template))
//        {
//            items.Remove(item);
//        }
//    }
//    return items;
//}
    

}}