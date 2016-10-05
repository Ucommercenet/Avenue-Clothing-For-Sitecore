using System.Collections.Generic;
using UCommerce.Api;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class ProductViewModel
	{
		public ProductViewModel()
		{
			Variants = new List<ProductViewModel>();
		}

		public int Id { get; set; }
		public bool IsVariant { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }

		public string LongDescription { get; set; }

		public IList<ProductViewModel> Variants { get; set; }

		public string Sku { get; set; }

		public string VariantSku { get; set; }

		public PriceCalculation PriceCalculation { get; set; }
		public string ThumbnailImageUrl { get; set; }
	}
}