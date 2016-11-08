module.exports = function () {
	var instanceRoot = "C:\\inetpub\\SC8Accelerator\\Website";
	var config = {
		websiteRoot: instanceRoot,
		websiteUrl: "http://sc8accelerator",
		sitecoreLibraries: instanceRoot + "\\bin",
		licensePath: instanceRoot + "\\App_Data\\license.xml",
		solutionName: "AvenueClothing",
		buildConfiguration: "Debug",
		runCleanBuilds: true
	};
	return config;
}