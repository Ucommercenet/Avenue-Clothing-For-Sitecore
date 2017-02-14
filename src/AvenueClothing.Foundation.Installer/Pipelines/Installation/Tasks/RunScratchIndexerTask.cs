using System.Collections.Specialized;
using Sitecore.Install.Framework;
using UCommerce.Infrastructure;
using UCommerce.Pipelines;
using UCommerce.Search.Indexers;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
    public class RunScratchIndexerTask : IPipelineTask<InstallationPipelineArgs>
    {
        private readonly ScratchIndexer _scratchIndexer;

        public RunScratchIndexerTask(ScratchIndexer scratchIndexer)
        {
            _scratchIndexer = scratchIndexer;
        }

        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
        {
            _scratchIndexer.Index();

            return PipelineExecutionResult.Success;
        }
    }
}