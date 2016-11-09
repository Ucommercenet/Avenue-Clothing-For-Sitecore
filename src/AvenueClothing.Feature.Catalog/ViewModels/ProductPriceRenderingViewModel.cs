using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.ViewModels
{
    public class ProductPriceRenderingViewModel
    {
        public string CalculatePriceUrl { get; set; }

        public string CalculateVariantPriceUrl { get; set; }

        public string SKU { get; set; }

        public Guid CategoryGuid { get; set; }

        public int CatalogGuid { get; set; }
    }
}