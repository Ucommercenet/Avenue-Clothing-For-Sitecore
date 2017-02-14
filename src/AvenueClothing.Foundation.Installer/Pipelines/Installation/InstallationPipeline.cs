using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.Infrastructure.Logging;
using UCommerce.Pipelines;

namespace AvenueClothing.Foundation.Installer.Pipelines.Installation
{
    public class InstallationPipelineArgs
    {
        
    }

    public class InstallationPipeline : Pipeline<InstallationPipelineArgs>
    {
        public InstallationPipeline(IPipelineTask<InstallationPipelineArgs>[] tasks, ILoggingService loggingService) : base(tasks, loggingService)
        {
        }
    }
}
