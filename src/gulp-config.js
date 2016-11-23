module.exports = function () {
	var instanceRoot = "C:\\inetpub\\sc8\\Website";
	var config = {
		websiteRoot: instanceRoot,
		websiteUrl: "http://sc8",
		sitecoreLibraries: instanceRoot + "\\bin",
		licensePath: instanceRoot + "\\App_Data\\license.xml",
		solutionName: "AvenueClothing",
		buildConfiguration: "Debug",
		runCleanBuilds: true
	};
	return config;
}