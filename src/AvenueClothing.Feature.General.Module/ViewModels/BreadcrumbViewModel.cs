using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace AvenueClothing.Feature.General.Module.ViewModels
{
    public class BreadcrumbViewModel : CustomItem
    {
        public BreadcrumbViewModel(Item innerItem) : base(innerItem)
        {
        }

        //public string BreadcrumbName { get; set; }
        //public string BreadcrumbUrl { get; set; }
        public string BreadcrumbName
        { get { return InnerItem["Title"]; } }

        public bool IsActive
        { get { return Sitecore.Context.Item.ID == InnerItem.ID; } }

        public string BreadcrumbUrl
        { get { return LinkManager.GetItemUrl(InnerItem); } }
    }
}

