using System.Collections.Generic;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
    public class VariantExistsViewModel
    {
        public VariantExistsViewModel()
        {
            VariantNameValueDictionary = new Dictionary<string, string>();
        }

        public string ProductSku { get; set; }
        public Dictionary<string, string> VariantNameValueDictionary { get; set; }
    }
}