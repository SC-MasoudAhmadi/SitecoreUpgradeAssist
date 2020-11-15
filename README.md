# SitecoreUpgradeAssist
=======================

[![Visual Studio Marketplace Version](https://vsmarketplacebadge.apphb.com/version/PavelSamokha.TargetFrameworkMigrator.svg)](https://marketplace.visualstudio.com/items?itemName=PavelSamokha.TargetFrameworkMigrator)

[![Build status](https://ci.appveyor.com/api/projects/status/asrqt7urcujs06lp/branch/master?svg=true)](https://ci.appveyor.com/project/304NotModified/targetframeworkmigrator/branch/master)


This tool is aiming to assist you with all the project refactoring required to upgrade your Sitecore solution from any version.

Features:
* Upgrade all projects .NetFramework to the version required by the target Sitecore Version (special credit to [TargetFrameworkMigrator](https://github.com/TargetFrameworkMigrator/TargetFrameworkMigrator))
* Reinstall all the dependent packages to match the new .NetFramework and Sitecore Version.
* Upgrade all sitecore packages.
* Upgrade GlassMapper.
* Upgrade SXA.
* Upgrade Unicorn.
* Assist with Glassmaper 4 to 5 Migration (code refactoring) (coming soon).
* Convert Unit Test solutions to Package referencing (coming soon).


## How to use

Tools -> Sitecore Upgrade Assist

![image](https://raw.githubusercontent.com/SC-MasoudAhmadi/SitecoreUpgradeAssist/develop/Src/SitecoreUpgradeAssistVSIX/tool-menu-btn.png)

Select projects and follow the instruction in the form

![image](https://github.com/SC-MasoudAhmadi/SitecoreUpgradeAssist/blob/develop/Src/SitecoreUpgradeAssistVSIX/nupreview.png?raw=true)


Development
===================

Use Visual Studio 2019.

Frameworks list
-------------------

To add support for new Sitecore version upgrade, add new [sitecoreVersion].xml in Src/SitecoreUpgradeAssist/Wheelbarrowex/Configs/VersionConfigs/
The new config can be a copy of the 9.3.0.xml and update the required package versions and dotnet framework version.
Update SitecoreVersions.xml with a new tag to add the version to the Sitecore Version select combobox.
Where to get Id for new framework? I get it via runtime (change one project's framework in visual studio project properties and get it's Id in debug mode).

How to debug visual studio extension
------------------------------------

Set "Run external program" in Debug to Visual Studio devenv.exe (e.g. C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\\devenv.exe) and command line arguments to `/rootsuffix Exp`

![image](https://user-images.githubusercontent.com/5808377/71218359-81352e00-22c1-11ea-8843-4661c57f3442.png)

