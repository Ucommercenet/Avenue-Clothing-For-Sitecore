﻿using AvenueClothing.Foundation.Installer.Services;
using UCommerce.Pipelines;

namespace AvenueClothing.Foundation.Installer.Pipelines.Installation.Tasks
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
