using System.Collections.Generic;

namespace AvenueClothing.Project.Navigation.ViewModels
{
    public class BreadcrumbWrapper
    {
        public BreadcrumbWrapper() {
            SitecoreBreadcrumbs = new List<BreadcrumbViewModel>();
            UcommerceBreadcrumbs = new List<BreadcrumbViewModelUcommerce>();
        }
        public IList<BreadcrumbViewModel> SitecoreBreadcrumbs { get; set; }
        public IList<BreadcrumbViewModelUcommerce> UcommerceBreadcrumbs { get; set; }
    }
}