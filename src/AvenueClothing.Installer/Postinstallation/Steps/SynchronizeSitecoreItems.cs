using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Rainbow.Storage.Yaml;
using Sitecore.Data.Serialization;
using Sitecore.Data.Serialization.ObjectModel;
using Sitecore.Install.Framework;
using Sitecore.IO;
using UCommerce.Extensions;
using Unicorn;
using Unicorn.Configuration;
using Unicorn.Data;
using Unicorn.Loader;
using Unicorn.Logging;
using Unicorn.Predicates;

namespace AvenueClothing.Installer.Postinstallation.Steps
{
	public class SynchronizeSitecoreItems : IPostStep
	{
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
            var itemsDicetory = GetItemsDirectory();
            ProcessDirectory(itemsDicetory);
        }

        public virtual void ProcessDirectory(DirectoryInfo directory)
        {
            if (directory == null) throw new InvalidOperationException("DirectoryInfo is null");

            if (!directory.Exists) throw new InvalidOperationException("Directory does not exists under the website.");

            var directoryShortName = directory.Name;
            var configurations = UnicornConfigurationManager.Configurations;

            if (configurations == null) throw new InvalidOperationException("Could not determine configurations for Unicorn.");

            var configuration = configurations.First(c => c.Name.Equals(directoryShortName, StringComparison.OrdinalIgnoreCase));
            
            if (configuration == null) throw new InvalidOperationException("Could not determine configuration for installation serialization with name: {0}".FormatWith(directoryShortName));

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

        public virtual string GetTargetDataStorePathFromIConfiguration(IConfiguration configuration)
        {
            var targetDataStore = configuration.Resolve<ITargetDataStore>();
            if (targetDataStore == null)
                throw new Exception($"targetDatastore undefined in configuration '{configuration.Name}'");

            return targetDataStore.GetConfigurationDetails().First(kvp => kvp.Key.Equals("Physical root path")).Value;
        }

        private string GetSafeAppRoot()
        {
            try
            {
                return FileUtil.MapPath("/");
            }
            catch (Exception exception)
            {

            }
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private DirectoryInfo GetItemsDirectory()
        {
            var rootPath = GetSafeAppRoot();

            var combinedPath = Path.Combine(rootPath, @"App_Data\tmp\accelerator\AvenueClothing\serialization");

            var itemsDirectory = new DirectoryInfo(combinedPath);

            if (!itemsDirectory.Exists)
            {
                throw new DirectoryNotFoundException(string.Format("Sitecore items wasn't found in '{0}'. Please make sure that the configured items path is correct. Rootpath was: '{1}'", combinedPath, rootPath));
            }

            return itemsDirectory;
        }
    }
}
