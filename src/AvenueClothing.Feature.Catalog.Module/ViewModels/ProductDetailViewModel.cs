using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class ProductDetailViewModel
	{
		public HtmlString LongDescription { get; set; }
		public string ReviewListRendering { get; set; }
		public string ReviewFormRendering { get; set; }
	}
}