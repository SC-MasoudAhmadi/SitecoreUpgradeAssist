using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.PlatformUI;
using VHQLabs.TargetFrameworkMigrator;
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

        public string StartProjectMigration(DTE2 dte,string solutionName, List<string> selectedProjects)
        {
            List<UIHierarchyItem> result = new List<UIHierarchyItem>();
            FindHierarchyItems(dte.ToolWindows.SolutionExplorer.UIHierarchyItems, selectedProjects, result,dte);
            return null;
        }

        private void FindHierarchyItems([NotNull]UIHierarchyItems items, [NotNull]List<string> selectedProjects, [NotNull]List<UIHierarchyItem> result, DTE2 dte)
        {
            if (items.Count == 0 || !selectedProjects.Any())
            {
                return;
            }

            foreach (UIHierarchyItem root in items)
            {
                var itemName = root.Name;
                if (itemName == "Configuration")
                {
                    continue;
                }
                ExpandItems(root.UIHierarchyItems);
                if (!IsProjectNode(root))
                {
                    FindHierarchyItems(root.UIHierarchyItems,selectedProjects,result,dte);
                }

                if (selectedProjects.Contains(root.Name))
                {
                    
                    foreach (UIHierarchyItem projectSub in root.UIHierarchyItems)
                    {
                        
                        var name = projectSub.Name;
                        if (projectSub?.Name == "packages.config")
                        {
                            try
                            {
                                projectSub.Select(EnvDTE.vsUISelectionType.vsUISelectionTypeSelect);
                                dte.ExecuteCommand(
                                    "ClassViewContextMenus.ClassViewProject.Migratepackages.configtoPackageReference");
                            }
                            catch
                            {

                            }
                            break;
                        }
                    }                    
                }
            }

        }

        private void ExpandItems(UIHierarchyItems items)
        {
            if (!items.Expanded)
                items.Expanded = true;
            if (!items.Expanded)
            {
                //bug: expand dont always work... 
                UIHierarchyItem parent = ((UIHierarchyItem)items.Parent);
                parent.Select(vsUISelectionType.vsUISelectionTypeSelect);

                UIHierarchy tree = items.DTE.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object as UIHierarchy;
                tree.DoDefaultAction();
                //_DTE.ToolWindows.SolutionExplorer.DoDefaultAction();
            }
        }

        private bool IsProjectNode(UIHierarchyItem item)
        {
            return IsDirectProjectNode(item) || IsProjectNodeInSolutionFolder(item);
        }

        private bool IsDirectProjectNode(UIHierarchyItem item)
        {
            return ((item.Object is Project) &&
                    ((item.Object as Project).Kind != ProjectKinds.vsProjectKindSolutionFolder));
        }

        private bool IsProjectNodeInSolutionFolder(UIHierarchyItem item)
        {
            return (item.Object is ProjectItem && ((ProjectItem) item.Object).Object is Project &&
                    ((Project) ((ProjectItem) item.Object).Object).Kind != ProjectKinds.vsProjectKindSolutionFolder);
        }

    }
}
