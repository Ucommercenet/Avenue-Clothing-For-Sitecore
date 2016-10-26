module.exports = function () {
	var instanceRoot = "C:\\inetpub\\sc8dev\\Website";
	var config = {
		websiteRoot: instanceRoot,
		sitecoreLibraries: instanceRoot + "\\bin",
		licensePath: instanceRoot + "\\App_Data\\license.xml",
		solutionName: "AvenueClothing",
		buildConfiguration: "Debug",
		runCleanBuilds: true
	};
	return config;
}