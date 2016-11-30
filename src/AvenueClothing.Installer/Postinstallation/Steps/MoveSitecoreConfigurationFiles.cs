using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Sitecore.Install.Framework;

namespace AvenueClothing.Installer.Postinstallation.Steps
{
    public class MoveSitecoreConfigurationFiles : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            new UCommerce.Installer.FileMover(
                new FileInfo(HostingEnvironment.MapPath("~/sitecore modules/Shell/ucommerce/install/App_Config/config_include/AvenueClothing.Serialization.config")),
                new FileInfo(HostingEnvironment.MapPath("~/App_Config/include/AvenueClothing.Serialization.config"))).Move(true ,(Exception ex)=> { throw ex; });

            new UCommerce.Installer.FileMover(
                new FileInfo(HostingEnvironment.MapPath("~/sitecore modules/Shell/ucommerce/install/App_Config/config_include/AvenueClothing.Sites.config")),
                new FileInfo(HostingEnvironment.MapPath("~/App_Config/include/AvenueClothing.Sites.config"))).Move(true, (Exception ex) => { throw ex; });
        }
    }
}
