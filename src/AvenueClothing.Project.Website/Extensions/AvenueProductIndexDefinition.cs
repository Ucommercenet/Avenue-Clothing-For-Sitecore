using Ucommerce.Search.Definitions;
using Ucommerce.Search.Extensions;
using Ucommerce.Search.Facets;

namespace AvenueClothing.Project.Website.Extensions
{
    public class AvenueProductIndexDefinition : DefaultProductsIndexDefinition
    {
        public AvenueProductIndexDefinition() : base()
        {
            this.Field(p => p["ShowOnHomepage"], typeof(bool));
            this.Field(p => p["CollarSize"], typeof(string));
            this.Field(p => p["ShoeSize"], typeof(string));
            this.Field(p => p["Colour"], typeof(string));
            
            this.Field(p => p.Taxes);
            this.Field(p => p.PricesInclTax);
            this.Field(p => p.UnitPrices);
            this.Field(p => p.PricesInclTax["EUR 15 pct"]).Facet();
            
            this.Field("Colour").Facet();
            this.Field("CollarSize").Facet();
            this.Field("ShoeSize").Facet();
        }
    }
}