using System;

namespace AvenueClothing.Project.Catalog.ViewModels
{
    public class ProductPriceRenderingViewModel
    {
        public string CalculatePriceUrl { get; set; }
		public string CalculateVariantPriceUrl { get; set; }
        public string Sku { get; set; }
        public Guid CategoryGuid { get; set; }
        public int CatalogGuid { get; set; }
	    public int ProductId { get; set; }
    }
}