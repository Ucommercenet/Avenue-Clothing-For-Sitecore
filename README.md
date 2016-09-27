How To Install AvenueClothing For Sitecore
===========================================
1. Getting your deploy site up and running
	1.1 You need a clean Sitecore 8.x website with uCommerce installed.
	
2. Getting your development environment up and running	
	2.1 You need to install NPM. You can download it here if you haven't installed it already: https://www.npmjs.com/
	
	2.2 Now open a command prompt (in admin mode.) Navigate to the solution so your command prompt is in the right directory next to where your package.json is located.
	
	2.2 Once npm is installed you need to run "npm install" against your package.json file, located in the "src" folder. This will restore all front-end dependencies for your development environment.

	2.3 You need gulp installed globally so you can access the "gulp" command line. To do so, run the following command: "npm install -g gulp"
	
	2.4 Now run the command "gulp". This will publish all items to the running website including all items for Sitecore.