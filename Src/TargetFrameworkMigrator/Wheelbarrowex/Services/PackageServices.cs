using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private async System.Threading.Tasks.Task UpdatePackage(Project project, PackageModel newPkg,bool includeDependencies, string pkgSrc)
        {
            _PkgInstaller.InstallPackage(pkgSrc, project, newPkg.Id, newPkg.Version, !includeDependencies);
        }
        public async System.Threading.Tasks.Task UpdatePackage(Project project, PackageModel newPkg, bool includeDependencies)
        {
            var pkgSrc = _PkgRepos.GetSources(true, false)?.FirstOrDefault(x => x.Key.Contains("nuget.org"));
            await UpdatePackage(project, newPkg, includeDependencies, pkgSrc.Value.Value);
        }

        public async System.Threading.Tasks.Task UpdateScPackage(Project project, PackageModel newPkg, bool includeDependencies)
        {
            
            var pkgSrcs = _PkgRepos.GetSources(true, false);
            var pkgSrc = pkgSrcs?.FirstOrDefault(x => x.Key.Contains("Sitecore V2")).Value ?? pkgSrcs?.FirstOrDefault(x => x.Key.Contains("Sitecore")).Value;
            await UpdatePackage(project, newPkg, includeDependencies, pkgSrc);
        }

        public async System.Threading.Tasks.Task UninstallPackage(Project project, PackageModel Pkg, bool includeDependencies)
        {
            _PkgUninstaller.UninstallPackage(project, Pkg.Id, includeDependencies);
        }

        public async System.Threading.Tasks.Task RestorePackages(Project project)
        {
            _PkgRestorer.RestorePackages(project);
        }
    }
}
