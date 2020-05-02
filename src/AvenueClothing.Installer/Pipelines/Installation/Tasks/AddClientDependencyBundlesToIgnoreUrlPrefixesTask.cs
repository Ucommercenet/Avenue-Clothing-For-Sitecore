using System.Web.Hosting;
using System.Xml;
using Ucommerce.Pipelines;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
    public class AddClientDependencyBundlesToIgnoreUrlPrefixesTask : IPipelineTask<InstallationPipelineArgs>
    {
        /// <summary>
        /// Adds ClientDependency bundle urls to ignore list of Sitecore,
        /// to ensure that sitecore doesn't resolve the request.
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
        {
            XmlDocument sitecoreConfig = new XmlDocument();
            // Load Sitecore.config which contains the value we are appending to.
            var sitecoreConfigPath = HostingEnvironment.MapPath("~/App_Config/Sitecore.config");

            if (sitecoreConfigPath != null)
            {
                sitecoreConfig.Load(sitecoreConfigPath);

                var ignoreUrlPrefixesNode =
                    sitecoreConfig.SelectSingleNode("sitecore/settings//setting[@name='IgnoreUrlPrefixes']");

                if (ignoreUrlPrefixesNode != null && ignoreUrlPrefixesNode.Attributes != null)
                {
                    var ignoreUrlPrefixesValue = ignoreUrlPrefixesNode.Attributes["value"].Value;
                    if (!ignoreUrlPrefixesValue.Contains("|/bundles|/content/css"))
                    {
                        ignoreUrlPrefixesNode.Attributes["value"].Value += "|/bundles|/content/css";
                    }
                }

                sitecoreConfig.Save(sitecoreConfigPath);
            }

            return PipelineExecutionResult.Success;
        }
    }
}
