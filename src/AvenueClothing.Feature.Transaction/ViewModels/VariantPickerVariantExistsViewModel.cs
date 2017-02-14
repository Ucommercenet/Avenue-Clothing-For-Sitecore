﻿using System.Collections.Generic;

namespace AvenueClothing.Feature.Transaction.ViewModels
{
    public class VariantPickerVariantExistsViewModel
    {
        public VariantPickerVariantExistsViewModel()
        {
            VariantNameValueDictionary = new Dictionary<string, string>();
        }

        public string ProductSku { get; set; }
        public IDictionary<string, string> VariantNameValueDictionary { get; set; }
    }
}