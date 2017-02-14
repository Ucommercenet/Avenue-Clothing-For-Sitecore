using System;
using System.Collections.Specialized;
using System.IO;
using System.Web.Hosting;
using Sitecore.Install.Framework;
using UCommerce.Pipelines;

namespace AvenueClothing.Foundation.Installer.Pipelines.Installation.Tasks
{
    public class MoveSitecoreConfigurationFilesTask : IPipelineTask<InstallationPipelineArgs>
    {
        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
        {
            new UCommerce.Installer.FileMover(
                new FileInfo(HostingEnvironment.MapPath("~/sitecore modules/Shell/ucommerce/install/App_Config/config_include/AvenueClothing.Serialization.config")),
                new FileInfo(HostingEnvironment.MapPath("~/App_Config/include/AvenueClothing.Serialization.config"))).Move(true, (Exception ex) => { throw ex; });

            new UCommerce.Installer.FileMover(
                new FileInfo(HostingEnvironment.MapPath("~/sitecore modules/Shell/ucommerce/install/App_Config/config_include/AvenueClothing.Sites.config")),
                new FileInfo(HostingEnvironment.MapPath("~/App_Config/include/AvenueClothing.Sites.config"))).Move(true, (Exception ex) => { throw ex; });

            return PipelineExecutionResult.Success;
        }
    }
}
