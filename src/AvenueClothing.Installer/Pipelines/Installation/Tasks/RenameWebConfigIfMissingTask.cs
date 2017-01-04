using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using UCommerce.Pipelines;

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

            new UCommerce.Installer.FileMover(
                new FileInfo(HostingEnvironment.MapPath("~/Views/Web.config.default")),
                new FileInfo(HostingEnvironment.MapPath("~/Views/Web.config"))).Move(false, (Exception ex) => { throw ex; });

            return PipelineExecutionResult.Success;
        }
    }
}
