using AvenueClothing.Installer.Services;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace AvenueClothing.Installer.Pipelines.Initialize
{
	public class SynchronizeSitecoreItemsTask : IPipelineTask<InitializeArgs>
	{
		private readonly SynchronizeSitecoreService _synchronizeSitecoreService;

		public SynchronizeSitecoreItemsTask(SynchronizeSitecoreService synchronizeSitecoreService)
		{
			_synchronizeSitecoreService = synchronizeSitecoreService;
		}

		public PipelineExecutionResult Execute(InitializeArgs subject)
		{
			_synchronizeSitecoreService.SynchronizeSitecoreItems();

			return PipelineExecutionResult.Success;
		}
	}
}
