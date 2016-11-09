using Sitecore.Data.Items;
using Sitecore.Links;

namespace AvenueClothing.Project.Navigation.ViewModels
{
    public class BreadcrumbViewModel : CustomItem   
    {

        public BreadcrumbViewModel(Item innerItem) : base(innerItem)
        {

        }

        //public string BreadcrumbName { get; set; }
        //public string BreadcrumbUrl { get; set; }
        public string BreadcrumbName
        { get { return InnerItem["Name"]; } }

        public bool IsActive
        { get { return Sitecore.Context.Item.ID == InnerItem.ID; } }

        public string BreadcrumbUrl
        {  get { return LinkManager.GetItemUrl(InnerItem); } }

        public string UcommerceBreadcrumbUrl { get; set; }
    }
}

