using System;
using System.Collections.Generic;
using System.Web;

namespace AvenueClothing.Feature.Catalog.ViewModels
{
    public class CategoryRenderingViewModel
    {
        public IList<Guid> ProductItemGuids { get; set; }
		public HtmlString DisplayName { get; set; }
	    public string ProductCardRendering { get; set; }
    }
}