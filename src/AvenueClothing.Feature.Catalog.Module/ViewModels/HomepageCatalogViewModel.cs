using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
    public class HomepageCatalogViewModel
    {
        public HomepageCatalogViewModel()
        {
            Products = new List<ProductViewModel>();
        }

        public IList<ProductViewModel> Products;
    }
}