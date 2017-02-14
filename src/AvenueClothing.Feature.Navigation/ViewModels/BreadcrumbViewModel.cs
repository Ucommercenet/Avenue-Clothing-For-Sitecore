using Sitecore.Data.Items;

namespace AvenueClothing.Project.Navigation.ViewModels
{
    public class BreadcrumbViewModel : CustomItem   
    {
		public BreadcrumbViewModel(Item innerItem) : base(innerItem){ }
        public string BreadcrumbName{ get; set; }
        public bool IsActive{ get; set; }
        public string BreadcrumbUrl{ get; set; }
        public string UcommerceBreadcrumbUrl { get; set; }
    }
}

