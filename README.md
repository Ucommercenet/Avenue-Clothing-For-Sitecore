# Avenue Clothing - Sitecore  #

## What do I get? ##
---

When installing Avenue Clothing you get:

+ The Avenue Clothing store, complete with a catalog, several categories and a variety of products.
+ Functional checkout process, including billing and shipping address forms, payment- and shipping options, basket overview and order confirmation.
+ Example product definitions(Shirt, Shoe, Accessory)
+ Example uCommerce data type(Color)
+ A simple and quick way to go from zero to hero, by building the solution on top of a clean Sitecore + uCommerce installation (more on this later..)
+ Frontend and backend validated user input
+ Faceted search
+ Full-text search
+ Product review system

## Compatibility ##
---
Ucommerce v9.5.0 and higher

Sitecore 10.1

## Overall System Architecture ##
---

The Avenue Clothing Sitecore accelerator was built with the [Helix](http://helix.sitecore.net) design principles and guidelines in mind.

The solution contains a separate project for each major area of interest. Each project contains modules(conceptual groupings of assets), that can be used interchangeably and independently.
 
Modules are built with an MVC structure, and represent controller renderings in the Sitecore backend, which can be attached to content items, layouts or templates.

A rendering can be added as a control by clicking PRESENTATION --> DETAILS on the desired item. Most renderings in the Avenue Clothing website are used on the Avenue Clothing item, and on the category and product definitions which can be found at:

    Templates --> User Defined --> uCommerce definitions

For example:
    
    Product definitions:
    Templates --> User Defined --> uCommerce definitions --> Shirt

    Category definitions:
    Templates --> User Defined --> uCommerce definitions --> Default Category Definition

The projects can also contain JavaScript modules in the /Scripts folders. These are client-side logical modules created, bundled and served using the [Require.js library](http://requirejs.org/).

These modules are not interrelated but can communicate using JavaScript events. Every Require.js module that starts with the prefix �js� and placed in the "/Scripts" folder will be automatically bundled.

## How To Install Avenue Clothing For Sitecore ##
---

Clone [this](https://bitbucket.org/uCommerce/avenue-clothing-for-sitecore) repository to your favorite location on your computer.

1. Getting your deploy site up and running 
    
    1.1 You need a clean Sitecore 10.1 website with Ucommerce 9.5 installed.

2. Getting avenue-clothing installed

    2.1 Open up the visual studio solution with the 'AvenueClothing.sln' file found under the folder 'src'

    2.2 Modify the "$websiteRoot" parameter found in the top of the 'Deploy.To.Local.ps1' file found under the solution folder 'deploy'. This parameter needs to match the website root path of your running website with Sitecore and uCommerce.

    2.3 Build your solution and refresh your browser to see Avenue-Clothing in action!

3. Generating an installation package (this is only needed if you want to deploy Avenue-Clothing on a computer where the solution is not present like on azure or similar.)

    3.1 Open a powershell prompt in windows and navigate the execution path to the following folder relative to your repo: 'tools\deploy'.

    **NOTE** Typing 'dir' should show you a list of files including 'Deploy.To.Package.ps1'

    3.2 Now you should just run the command: .\Deploy.To.Package -version x.x.x

    **NOTE** You can also use the -configuration and specify either 'debug' or 'release' to build the solution in either ms build configuration.

    3.3 The package will be put into the folder 'c:\tmp' and you can now use the package to install it on a sitecore + uCommerce environment through the  Sitecore package pages in the Sitecore backend.


## If you need assistance with an issue, you're more than welcome to contact us on our dedicated support email: support@ucommerce.net ##