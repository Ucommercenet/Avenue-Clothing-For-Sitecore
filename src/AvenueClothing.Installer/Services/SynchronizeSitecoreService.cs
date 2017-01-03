using System;
using System.Linq;
using Unicorn;
using Unicorn.Configuration;
using Unicorn.Logging;
using Unicorn.Predicates;

namespace AvenueClothing.Installer.Services
{
	public class SynchronizeSitecoreService
	{
		public virtual void SynchronizeSitecoreItems()
		{
			var configurations = UnicornConfigurationManager.Configurations;

			if (configurations == null) throw new InvalidOperationException("Could not determine configurations for Unicorn.");

			var configuration = configurations.FirstOrDefault(c => c.Name.Equals("Project.AvenueClothing.Website.Installer", StringComparison.OrdinalIgnoreCase));

			if (configuration == null) configuration = configurations.FirstOrDefault(c => c.Name.Equals("Project.AvenueClothing.Website", StringComparison.OrdinalIgnoreCase));
			if (configuration == null) throw new InvalidOperationException("No configuration for unicorn was found under sitecore include folder. Looked for: 'Project.AvenueClothing.Website.Installer' and 'Project.AvenueClothing.Website'");

			SynchroniseTargetDataStore(configuration);
		}

		public virtual void SynchroniseTargetDataStore(IConfiguration configuration)
		{
			var logger = configuration.Resolve<ILogger>();
			var helper = configuration.Resolve<SerializationHelper>();

			try
			{
				logger.Info(string.Empty);
				logger.Info("Unicorn.Bootstrap is syncing " + configuration.Name);

				var pathResolver = configuration.Resolve<PredicateRootPathResolver>();

				var roots = pathResolver.GetRootSerializedItems();

				helper.SyncTree(configuration, null, roots);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				throw;
			}
		}
	}
}
