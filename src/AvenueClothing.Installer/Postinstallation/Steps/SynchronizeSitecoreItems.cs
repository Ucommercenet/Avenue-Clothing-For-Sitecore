using System;
using System.Collections.Specialized;
using System.IO;
using Sitecore.Data.Serialization;
using Sitecore.Data.Serialization.ObjectModel;
using Sitecore.Install.Framework;
using Sitecore.IO;

namespace AvenueClothing.Installer.Postinstallation.Steps
{
	public class SynchronizeSitecoreItems : IPostStep
	{
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			var itemsDicetory = GetItemsDirectory();

			Syncronize(itemsDicetory);

			foreach (var info in itemsDicetory.GetDirectories())
			{
				Syncronize(info);
			}
		}

		private static void Syncronize(DirectoryInfo itemsDicetory)
		{
			foreach (var fileInfo in itemsDicetory.GetFiles("*.yml"))
			{
				var streamReader = new StreamReader(fileInfo.FullName);

				var syncItem = SyncItem.ReadItem(new Tokenizer(streamReader), true);

				var options = new LoadOptions {DisableEvents = true, ForceUpdate = true, UseNewID = false};

				ItemSynchronization.PasteSyncItem(syncItem, options, true);
			}
		}

		private DirectoryInfo GetItemsDirectory()
		{
			var rootPath = GetSafeAppRoot();

			var itemsDirectory = new DirectoryInfo(Path.Combine(rootPath, @"\App_Data\tmp\accelerator\AvenueClothing\serialization"));

		    if (!itemsDirectory.Exists)
		    {
		        throw new DirectoryNotFoundException(string.Format("Sitecore items wasn't found in '{0}'. Please make sure that the configured items path is correct. Rootpath was: '{1}'", itemsDirectory, rootPath));
		    }

			return itemsDirectory;
		}

		private static string GetSafeAppRoot()
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
	}
}
