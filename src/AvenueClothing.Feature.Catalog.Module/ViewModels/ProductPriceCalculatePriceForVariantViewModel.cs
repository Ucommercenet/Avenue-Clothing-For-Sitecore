using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
    public class VariantPriceCalculationDetails
    {
        public string ProductSku { get; set; }
        public string ProductVariantSku { get; set; }
        public int CatalogId { get; set; }
        public Guid CategoryId { get; set; }
    }
}