using System.Collections.Generic;

namespace AvenueClothing.Feature.Catalog.ViewModels
{
    public class VariantPickerRenderingViewModel
    {
        public VariantPickerRenderingViewModel()
        {
            Variants = new List<Variant>();
        }
        public List<Variant> Variants { get; set; }
        public string ProductSku { get; set; }
        public string VariantExistsUrl { get; set; }

        public class Variant
        {
            public Variant()
            {
                VaraintItems = new List<VaraintValue>();
            }

            public string Name { get; set; }
            public string DisplayName { get; set; }
            public List<VaraintValue> VaraintItems { get; set; }

            public class VaraintValue
            {
                public string Name { get; set; }
                public string DisplayName { get; set; }
            }
        }
    }
}