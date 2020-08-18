using Ucommerce.Pipelines;
using Ucommerce.Search.Indexers;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
    public class RunScratchIndexerTask : IPipelineTask<InstallationPipelineArgs>
    {
        private readonly IScratchIndexer _scratchIndexer;

        public RunScratchIndexerTask(IScratchIndexer scratchIndexer)
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