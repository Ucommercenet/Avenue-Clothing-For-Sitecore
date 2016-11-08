using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
    public class CategoryRenderingViewModel
    {
        public List<Guid> ProductItemGuids { get; set; }
		public HtmlString DisplayName { get; set; }
    }
}