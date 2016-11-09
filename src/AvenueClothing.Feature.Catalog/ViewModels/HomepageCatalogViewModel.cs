using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.ViewModels
{
    public class HomepageCatalogViewModel
    {
        public HomepageCatalogViewModel()
        {
            Products = new List<ProductCardRenderingViewModel>();
        }

        public IList<ProductCardRenderingViewModel> Products;
    }
}