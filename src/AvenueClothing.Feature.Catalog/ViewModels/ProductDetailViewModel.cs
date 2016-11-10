using System.Web;

namespace AvenueClothing.Feature.Catalog.ViewModels
{
	public class ProductDetailViewModel
	{
		public HtmlString LongDescription { get; set; }
		public string ReviewListRendering { get; set; }
		public string ReviewFormRendering { get; set; }
	}
}