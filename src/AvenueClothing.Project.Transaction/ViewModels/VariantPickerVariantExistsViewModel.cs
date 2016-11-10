using System.Collections.Generic;

namespace AvenueClothing.Project.Transaction.ViewModels
{
    public class VariantPickerVariantExistsViewModel
    {
        public VariantPickerVariantExistsViewModel()
        {
            VariantNameValueDictionary = new Dictionary<string, string>();
        }

        public string ProductSku { get; set; }
        public Dictionary<string, string> VariantNameValueDictionary { get; set; }
    }
}