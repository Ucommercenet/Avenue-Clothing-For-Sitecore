using System.Web;
using AvenueClothing.Project.Website;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: PreApplicationStartMethod(typeof(AvenueClothingStartup), "PreStart")]
namespace AvenueClothing.Project.Website
{
    public static class AvenueClothingStartup
    {
        public static void PreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof(AvenueClothingHttpModule));
        }
    }
}
