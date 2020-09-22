using Sitecore.Caching;
using Sitecore.Configuration;
using Ucommerce.Pipelines;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
    public class ClearSitecoreCacheTask : IPipelineTask<InstallationPipelineArgs>
    {
        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
        {
            CacheManager.ClearAllCaches();
            Factory.Reset();

            return PipelineExecutionResult.Success;
        }
    }
}