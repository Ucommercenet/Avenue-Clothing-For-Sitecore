﻿using System;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Publishing;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Pipelines;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
    public class PublishMasterDatabaseTask : IPipelineTask<InstallationPipelineArgs>
    {

        private void PublishEverything()
        {
            var loggingService = ObjectFactory.Instance.Resolve<ILoggingService>();

            var masterDatabase = Factory.GetDatabase("master");

            var item = masterDatabase.GetItem(ID.Parse("11111111-1111-1111-1111-111111111111"));

            try
            {
                PublishItem(item, masterDatabase, loggingService);
            }
            catch (Exception ex)
            {
                loggingService.Log<PublishMasterDatabaseTask>(ex);
                throw;
            }
        }

        private static void PublishItem(Item item, Database masterDatabase, ILoggingService loggingService)
        {
            if (item == null)
            {
                loggingService.Log<PublishMasterDatabaseTask>("Could not publish to targetDatbase. Item is null");
                return;
            }

            loggingService.Log<PublishMasterDatabaseTask>("Publishing to web from demo store installer.");

            var publishOptions = new PublishOptions(masterDatabase,
                Database.GetDatabase("web"),
                Sitecore.Publishing.PublishMode.Full,
                item.Language,
                DateTime.Now);

            var publisher = new Publisher(publishOptions);
            publisher.Options.RootItem = item;
            publisher.Options.Deep = true;

            publisher.Publish();

            loggingService.Log<PublishMasterDatabaseTask>("Publishing done from demo store installer.");
        }

        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
        {
            PublishEverything();

            return PipelineExecutionResult.Success;
        }
    }
}