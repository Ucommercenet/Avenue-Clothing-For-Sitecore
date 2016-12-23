How To Install AvenueClothing For Sitecore
===========================================
Clone this repo to your favorite location on your computer.

1. Getting your deploy site up and running

	1.1 You need a clean Sitecore 8.x website with uCommerce installed.
	
2. Getting avenue-clothing installed

	2.1 Open up the visual studio solution with the 'AvenueClothing.sln' file found under the folder 'src'

	2.2 Modify the "$websiteRoot" paramter found in the top of the 'Deploy.To.Local.ps1' file found under the solution folder 'deploy'. This parameter needs to match the website root path of your running website with Sitecore and uCommerce.

	2.3 Build your solution and refresh your browser to see Avenue-Clothing in action!

3. Generating an installation package

	3.1 Open a powershell promt in windows and navigate the execution path to the following folder relative to your repo: 'tools\deploy'.
	3.1.1 Typing 'dir' should show you a list of files including 'Deploy.To.Package.ps1'
	3.2 Now you should just run the command: .\Deploy.To.Package -version x.x.x 
	3.2.1 You can also use the -configuration and specify either 'debug' or 'release' to build the solution in either ms build configuration.
	3.3 The package will be put into the folder 'c:\tmp' and you can now use the package to install it on a sitecore + uCommerce environement through the  Sitecore package pages in the Sitecore backend.


#If you experience any errors during the build command, try to redo the command but press your build-shortcut a little harder. This will most likely work! If that is not the case, you're more than welcome to contact us on our support forum http://eureka.ucommerce.net/#!/ and let us know we should engage a small army run by headless monkeys to fix your issue!