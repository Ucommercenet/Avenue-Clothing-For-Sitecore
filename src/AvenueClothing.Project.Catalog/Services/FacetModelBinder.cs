using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using UCommerce.Search.Facets;
using DefaultModelBinder = System.Web.ModelBinding.DefaultModelBinder;

namespace AvenueClothing.Project.Catalog.Services
{
    public class FacetModelBinder : DefaultModelBinder, IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var parameters = new Dictionary<string, string>();
             
            foreach (var queryString in HttpContext.Current.Request.QueryString.AllKeys)
            { 
                parameters[queryString] = HttpContext.Current.Request.QueryString[queryString];
            }
            if (parameters.ContainsKey("umbDebugShowTrace"))
            {
                parameters.Remove("umbDebugShowTrace");
            }
            if (parameters.ContainsKey("product"))
            {
                parameters.Remove("product");
            }
            if (parameters.ContainsKey("category"))
            {
                parameters.Remove("category");
            }
            if (parameters.ContainsKey("catalog"))
            {
                parameters.Remove("catalog");
            }
            var facetsForQuerying = new List<Facet>();

            foreach (var parameter in parameters)
            {
                var facet = new Facet();
                facet.FacetValues = new List<FacetValue>();
                facet.Name = parameter.Key;
                foreach (var value in parameter.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    facet.FacetValues.Add(new FacetValue() { Value = value });
                }
                facetsForQuerying.Add(facet);
            }

            return facetsForQuerying;
        }

       
    }
}