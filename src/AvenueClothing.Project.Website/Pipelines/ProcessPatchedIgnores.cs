using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Pipelines.HttpRequest;

namespace AvenueClothing.Project.Website.Pipelines
{
    /// <summary>
    /// Since sitecore 9.2 at least we need to make sure that their routing does not screw around with the filebundling e.g
    /// /styles/css.
    /// We could use their own ignoreprefix but it is hard to programatically patch it using config only so this processor will just be added after to abort the request pipeline for our bundle routes.
    /// </summary>
    public class ProcessPatchedIgnores : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            string filePath = args.Url.FilePath;
            var paths = new List<string>()
            {
                "/content/css",
                "/bundles/require"
            };

            foreach (string path in paths)
            {
                if (filePath.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                {
                    args.AbortPipeline();
                    return;
                }
            }
        }
    }

}