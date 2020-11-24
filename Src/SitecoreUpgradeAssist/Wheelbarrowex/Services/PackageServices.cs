using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.PlatformUI;
using Wheelbarrowex.Models;

namespace Wheelbarrowex.Services
{
    public class PackageServices
    {

        private IVsPackageInstallerServices _PkgService;
        private IVsPackageInstaller2 _PkgInstaller;
        private IVsPackageSourceProvider _PkgRepos;
        private IVsPackageUninstaller _PkgUninstaller;
        private IVsPackageRestorer _PkgRestorer;
        private DTE2 DTE;
        public PackageServices()
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            
            _PkgInstaller = componentModel.GetService<IVsPackageInstaller2>();
            _PkgService = componentModel.GetService<IVsPackageInstallerServices>();
            _PkgRepos = componentModel.GetService<IVsPackageSourceProvider>();
            _PkgUninstaller = componentModel.GetService<IVsPackageUninstaller>();
            _PkgRestorer = componentModel.GetService<IVsPackageRestorer>();


        }
        public IEnumerable<PackageModel> GetInstalledNugetPackages(Project project)
        {
            return _PkgService.GetInstalledPackages(project).Select(x => new PackageModel() { Id = x.Id, Version = x.VersionString });
        }
        private void UpdatePackage(Project project, PackageModel newPkg,bool includeDependencies, string pkgSrc)
        {
            _PkgInstaller.InstallPackage(pkgSrc, project, newPkg.Id, newPkg.Version, !includeDependencies);
        }
        public void UpdatePackage(Project project, PackageModel newPkg, bool includeDependencies)
        {
            var pkgSrc = _PkgRepos.GetSources(true, false)?.FirstOrDefault(x => x.Key.Contains("nuget.org"));
            UpdatePackage(project, newPkg, includeDependencies, pkgSrc.Value.Value);
        }

        public void UpdateScPackage(Project project, PackageModel newPkg, bool includeDependencies)
        {
            
            var pkgSrcs = _PkgRepos.GetSources(true, false);
            var pkgSrc = pkgSrcs?.FirstOrDefault(x => x.Key.Contains("Sitecore V2")).Value ?? pkgSrcs?.FirstOrDefault(x => x.Key.Contains("Sitecore")).Value;
            UpdatePackage(project, newPkg, includeDependencies, pkgSrc);
        }

        public void UninstallPackage(Project project, PackageModel Pkg, bool includeDependencies)
        {
            _PkgUninstaller.UninstallPackage(project, Pkg.Id, !includeDependencies);
        }

        public void RestorePackages(Project project)
        {
            _PkgRestorer.RestorePackages(project);
        }

        public string StartProjectMigration(DTE2 dte,string solutionName, string projectFilePath)
        {
            var slnFileName = Path.GetFileNameWithoutExtension(solutionName);
            var slnRootDir = Path.GetDirectoryName(solutionName);
            var prjPath = projectFilePath.Replace(slnRootDir + "\\src\\", string.Empty);
            var projectPath = Path.Combine(slnFileName, prjPath);

            var projectItem = dte.ToolWindows.SolutionExplorer.GetItem(@"");
            projectItem.Select(EnvDTE.vsUISelectionType.vsUISelectionTypeSelect);
            var chilItems = projectItem.UIHierarchyItems.GetEnumerator();

            while (chilItems.MoveNext())
            {
                var current = chilItems as UIHierarchyItem;
                if (current?.Name == "packages.config")
                {
                    current.Select(EnvDTE.vsUISelectionType.vsUISelectionTypeSelect);
                    break;
                }
            }
            dte.ExecuteCommand("ClassViewContextMenus.ClassViewProject.Migratepackages.configtoPackageReference");
            return projectPath;
        }
    }
}
