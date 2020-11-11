# SitecoreUpgradeAssist
=======================

[![Visual Studio Marketplace Version](https://vsmarketplacebadge.apphb.com/version/PavelSamokha.TargetFrameworkMigrator.svg)](https://marketplace.visualstudio.com/items?itemName=PavelSamokha.TargetFrameworkMigrator)

[![Build status](https://ci.appveyor.com/api/projects/status/asrqt7urcujs06lp/branch/master?svg=true)](https://ci.appveyor.com/project/304NotModified/targetframeworkmigrator/branch/master)


This tool is aiming to assist you with all the project refactoring required to update your Sitecore solution from any version.

Features:
* Upgrade all projects .NetFramework to the version required by target Sitecore Version (special credit to [TargetFrameworkMigrator](https://github.com/TargetFrameworkMigrator/TargetFrameworkMigrator))
* Reinstall all the dependent assebly packages to match the new .NetFramework
* Upgrade all sitecore packages.
* Upgrade GlassMaper
* Upgrade SXA
* Upgrade Unicorn
* Assist with Glassmaper 4 to 5 Migration (code refactoring)
* Convert Unit Test solutions to Package referencing
* Support .Net Frameworks 2.0-4.8
* Support solution folders 


## How to use

Tools -> Target Framework Migrator

![image](https://user-images.githubusercontent.com/5808377/71218148-bdb45a00-22c0-11ea-9347-13d37c299b7d.png)

Select projects and press "Migrate"

![image](https://user-images.githubusercontent.com/5808377/71218330-5ea31500-22c1-11ea-8aa8-de62af5ca6c4.png)


Development
===================

Use Visual Studio 2017 or 2019. The integration tests are currently broken.

Frameworks list
-------------------

Edit Frameworks.xml in main project to add new framework.
Where to get Id for new framework? I get it via runtime (change one project's framework in visual studio project properties and get it's Id in debug mode).

How to debug visual studio extension
------------------------------------

Set "Run external program" in Debug to Visual Studio devenv.exe (e.g. C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\\devenv.exe) and command line arguments to `/rootsuffix Exp`

![image](https://user-images.githubusercontent.com/5808377/71218359-81352e00-22c1-11ea-8843-4661c57f3442.png)

