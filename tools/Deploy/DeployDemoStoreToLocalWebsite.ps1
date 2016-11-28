task DeployDemoStoreToLocalWebsite -depends SetPathVars,CopyDemoStoreFiles {

}

task SetPathVars -description "Sets the local paths." {
	# Website root folder (website is deployed here)
    $properties["website_root_folder"] = "C:\inetpub\sc8\Website"

    # Website data folder (items are deployed here)
    # Default assumes the website and data folder sits on the same level
    # /MySite
    #	/Website
    #	/Data
    $properties["website_data_folder"] =  "c:\inetpub\SCTest\sc8\App_Data\data"
}

task CopyDemoStoreFiles -description "Copy all the Demo Store files to the local website" {
    
    # Exclude files and folders from deploy, usually these are
    # source code files, proj files from Visual Studio, and other 
    # files not required at runtime
    $options = @("/E", "/S", "/xf", "*.cs", "/xf", "*.??proj", "/xf", "*.user", "/xf", "*.old", "/xf", "*.vspscc", "/xf", "xsltExtensions.config", "/xf", "uCommerce.key", "/xf", "*.tmp", "/xd", "_Resharper*", "/xd", ".svn", "/xd", "_svn")    
    
    $website_root = $properties["website_root_folder"]
    $website_data = $properties["website_data_folder"]
	$src = $properties["src"]

    # Deploy Unicorn for syncing Sitecore items
    # Need a couple of DLLs in /bin
    & new-item "$website_root\App_Code\Unicorn\sync-database.aspx.cs" -Force -ItemType file
    & Copy-Item "$src\..\tools\unicorn\sync-database.aspx.cs" "$website_root\App_Code\Unicorn\sync-database.aspx.cs" -Recurse -Force

    & robocopy "$src\..\tools\Unicorn" "$website_root\bin" *.dll $options
    # Need aspx for accessing sync in Sitecore/Sitecore Modules/Shell
    & robocopy "$src\..\tools\Unicorn" "$website_root\sitecore modules\shell\Unicorn" *.aspx $options
    #Finally the serialization include files

    # Sync item files two-way between website data folder and project folder
    # Assume items are modified in Sitecore primarily so:
    # First copy new/changed items from Sitecore back to project
    # Next copy new/changed items from project to Sitecore
    & robocopy "$website_data\master" "$src\uCommerce.DemoStore.SitecoreItems\master" /E /XO
    & robocopy "$src\uCommerce.DemoStore.SitecoreItems\master" "$website_data\master" /E /XO

    # Copy all site specific files into the website
    & robocopy "$src\uCommerce.DemoStore\Controls" "$website_root\Controls" $options
    & robocopy "$src\uCommerce.DemoStore\Css" "$website_root\Css" $options
    & robocopy "$src\uCommerce.DemoStore\img" "$website_root\img" $options
    & robocopy "$src\uCommerce.DemoStore\Layouts" "$website_root\Layouts" $options
    & robocopy "$src\uCommerce.DemoStore\Scripts" "$website_root\Scripts" $options
    & robocopy "$src\uCommerce.DemoStore\bin" "$website_root\bin" UCommerce.DemoStore* $options /xf Sitecore.*.dll /xf Kamsar.WebConsole.dll /xf unicorn.dll /xf ServiceStack.* /xf UCommerce.dll /xf UCommerce.Infrastructure.dll /xf UCommerce.Pipelines.dll /xf UCommerce.Presentation.dll /xf UCommerce.Transactions.Payments.dll /xf UCommerce.Web.Shell* /xf UCommerce.Sitecore*.dll /xf UCommerce.Web*.dll
	& robocopy "$src\uCommerce.DemoStore\App_Config" "$website_root\App_Config" $options

}