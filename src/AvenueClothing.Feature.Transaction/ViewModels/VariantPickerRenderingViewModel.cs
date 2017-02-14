using System.Collections.Generic;

namespace AvenueClothing.Project.Transaction.ViewModels
{
    public class VariantPickerRenderingViewModel
    {
        public VariantPickerRenderingViewModel()
        {
            Variants = new List<Variant>();
        }
        public IList<Variant> Variants { get; set; }
        public string ProductSku { get; set; }
        public string VariantExistsUrl { get; set; }
        public string GetAvailableCombinationsUrl { get; set; }

        public class Variant
        {
            public Variant()
            {
                VaraintItems = new List<VaraintValue>();
            }

            public string Name { get; set; }
            public string DisplayName { get; set; }
            public IList<VaraintValue> VaraintItems { get; set; }

            public class VaraintValue
            {
                public string Name { get; set; }
                public string DisplayName { get; set; }
            }
        }
    }
}