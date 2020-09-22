using Ucommerce.Infrastructure.Logging;
using Ucommerce.Pipelines;

namespace AvenueClothing.Installer.Pipelines.Installation
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
