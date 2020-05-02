using System;
using System.IO;
using System.Web.Hosting;
using Ucommerce.Pipelines;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
    public class RenameWebConfigIfMissingTask : IPipelineTask<InstallationPipelineArgs>
    {
        public RenameWebConfigIfMissingTask()
        {

        }

        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
        {
            var webConfig = new FileInfo(HostingEnvironment.MapPath("~/Views/Web.config"));

            if (webConfig.Exists)
            {
                return PipelineExecutionResult.Success;
            }

            new Ucommerce.Installer.FileMover(
                new FileInfo(HostingEnvironment.MapPath("~/Views/Web.config.default")),
                new FileInfo(HostingEnvironment.MapPath("~/Views/Web.config"))).Move(false, (Exception ex) => { throw ex; });

            return PipelineExecutionResult.Success;
        }
    }
}
