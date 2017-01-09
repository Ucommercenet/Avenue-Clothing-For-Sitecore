using AvenueClothing.Installer.Services;
using UCommerce.Pipelines;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
	public class SynchronizeSitecoreItemsTask : IPipelineTask<InstallationPipelineArgs>
	{
		private readonly SynchronizeSitecoreService _synchronizeSitecoreService;

		public SynchronizeSitecoreItemsTask(SynchronizeSitecoreService synchronizeSitecoreService)
		{
			_synchronizeSitecoreService = synchronizeSitecoreService;
		}

		public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
		{
			_synchronizeSitecoreService.SynchronizeSitecoreItems();

			return PipelineExecutionResult.Success;
		}
	}
}
