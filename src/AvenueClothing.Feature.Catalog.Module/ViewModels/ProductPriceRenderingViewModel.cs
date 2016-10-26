using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
    public class ProductPriceRenderingViewModel
    {
        public String CalculatePriceUrl { get; set; }

        public string SKU { get; set; }

        public Guid CategoryGuid { get; set; }

        public int CatalogGuid { get; set; }
    }
}