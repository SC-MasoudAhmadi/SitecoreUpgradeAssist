using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using VHQLabs.TargetFrameworkMigrator;
using Wheelbarrowex.Models;
using Wheelbarrowex.Forms;
using System.Windows.Forms;
using Wheelbarrowex.Configs;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.ComponentModelHost;
using NuGet.VisualStudio;
using Task = System.Threading.Tasks.Task;
using System.Data.OleDb;
using EnvDTE80;

namespace Wheelbarrowex.Services
{
    public class SitecoreMigrator : Migrator
    {
        private readonly IEnumerable<SitecoreVersionModel> sitecoreVersionList;
        private SynchronizationContext synchronizationContext;
        private bool isSolutionLoaded = true;
        private PackageServices pkgMnger;

        public SitecoreMigrator(DTE applicationObject):base(applicationObject)
        {
            sitecoreVersionList = SitecoreVersionConfigManager.GetSupportedSitecoreVersions();
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pkgMnger = new PackageServices();
        }
        public override void Show()
        {
            lock (syncRoot)
            {
                synchronizationContext = SynchronizationContext.Current;

                projectsUpdateList = new SitecoreUpdator();

                projectsUpdateList.UpdateFired += Update;
                projectsUpdateList.ReloadFired += ReloadProjects;
                projectsUpdateList.UpdateMSSCPkgFired += UpdateMSSCPkgFired;
                projectsUpdateList.UpdateGlassPkgFired += UpdateGlassPkgFired;
                projectsUpdateList.MigrateToPackageReferencing += MigrateToPackageReferencing;

                projectsUpdateList.AvailableVersions = sitecoreVersionList;

                projectsUpdateList.State = "Waiting all projects are loaded...";

                if (applicationObject.Solution == null)
                {
                    projectsUpdateList.State = "No solution";
                }
                else
                {
                    if (isSolutionLoaded)
                        ReloadProjects();
                }

                projectsUpdateList.StartPosition = FormStartPosition.CenterScreen;
                projectsUpdateList.TopMost = true;
                projectsUpdateList.ShowDialog();

            }
        }


        async void Update()
        {
            projectsUpdateList.State = "Reading config file...";
            var selectedProjects = projectsUpdateList.Projects.Where(p => p.IsSelected); 
            var sitecoreConfigModel = SitecoreVersionConfigManager.GetSitecoreConfigModel(projectsUpdateList.SelectedSitecoreVersion.Id);

            if (sitecoreConfigModel.Error != null)
            {
                projectsUpdateList.State = sitecoreConfigModel.Error;
                return;
            }

            projectsUpdateList.State = "Updating dotnet Framework...";

            await UpdateFrameworks(selectedProjects, sitecoreConfigModel.Framework);

            //projectsUpdateList.EnableNextStep();
            projectsUpdateList.Projects = LoadProjects();

            projectsUpdateList.State = "Dotnet Framework has been updated... please verify and then continue";
        }
        private Task UpdateFrameworks(IEnumerable<ProjectModel> selectedProjects, FrameworkModel frameworkModel)
        {
            return Task.Run(() =>
            {

                foreach (var projectModel in selectedProjects)
                {
                    try
                    {
                        projectModel.DteProject.Properties.Item("TargetFrameworkMoniker").Value = frameworkModel.Name;
                        //projectModel.DteProject.Save();
                        synchronizationContext.Post(o =>
                        {
                            var pm = (ProjectModel)o;
                            projectsUpdateList.State = string.Format("Updating... {0} done", pm.Name);
                        }, projectModel);
                    }
                    catch (COMException e) //possible "project unavailable" for unknown reasons
                    {
                        projectsUpdateList.State = ("COMException on " + projectModel.Name + e);
                    }
                }
            });
        }


        async void UpdateMSSCPkgFired()
        {
            projectsUpdateList.State = "Started SC + MS Update...";
            var sitecoreConfigModel = SitecoreVersionConfigManager.GetSitecoreConfigModel(projectsUpdateList.SelectedSitecoreVersion.Id);

            if (sitecoreConfigModel.Error != null)
            {
                projectsUpdateList.State = sitecoreConfigModel.Error;
                return;
            }
            

            var selectedProjects = projectsUpdateList.Projects.Where(p => p.IsSelected);

            foreach (var prj in selectedProjects)
            {
                var prjPkgs = pkgMnger.GetInstalledNugetPackages(prj.DteProject);
                //restoring packages first
                await pkgMnger.RestorePackages(prj.DteProject);
                //MSPackages first
                var pkgToUpdate = prjPkgs.Where(x => x.Id.StartsWith("Microsoft."));
                if (pkgToUpdate.Any())
                {
                    await UpdateMsPackages(sitecoreConfigModel, prj, pkgToUpdate, prjPkgs);
                }

                //Sitecore package second
                pkgToUpdate = prjPkgs.Where(x => x.Id.StartsWith("Sitecore."));
                if (pkgToUpdate.Any())
                {
                        await UpdateSCPackages(sitecoreConfigModel, prj, pkgToUpdate, prjPkgs);
                }

                //other packages
                pkgToUpdate = prjPkgs.Where(x => !x.Id.StartsWith("Sitecore.") && !x.Id.StartsWith("Microsoft.") && !x.Id.StartsWith("Glass."));
                if (pkgToUpdate.Any())
                {
                    await UpdateOtherPackages(sitecoreConfigModel, prj, pkgToUpdate, prjPkgs);
                }

                //now build the project so it get saved and references get updated.
                await BuildProject(prj);
            }
            projectsUpdateList.Projects = LoadProjects();

            projectsUpdateList.State = "MS + Sitecore has been updated. Please verify and continue";
        }

        private async Task
        UpdateMsPackages(SitecoreConfigModel sitecoreConfigModel, ProjectModel prj, IEnumerable<PackageModel> pkgToUpdate, IEnumerable<PackageModel> prjPkgs)
        {
            projectsUpdateList.State = $"Updating MS packages for {prj.Name}";
            foreach (var oldPkg in pkgToUpdate)
            {
                try
                {
                    var newPkg = sitecoreConfigModel.MSPackages.FirstOrDefault(pkg => pkg.Id == oldPkg.Id);
                    if(newPkg == null)
                    {
                        projectsUpdateList.State = $"Sitecore {sitecoreConfigModel.SitecoreVersion} config does not have an equivalent for {oldPkg.Id}. Reinstalling the same version";
                        //await pkgMnger.UninstallPackage(prj.DteProject, oldPkg, false);
                        newPkg = oldPkg;
                    }
                    // this will be a pain if the user mistakenly upgrade packages first
                    //else if(newPkg.Version == oldPkg.Version)
                    //{
                    //    projectsUpdateList.State = $"Package {oldPkg.Id} is already up to date with version {oldPkg.Version} ";
                    //    continue;
                    //}
                
                    await pkgMnger.UpdatePackage(prj.DteProject, newPkg, false);
                    projectsUpdateList.State = $"Package {oldPkg.Id} updated to version {newPkg.Version} ";
                }
                catch (Exception e)
                {
                    projectsUpdateList.State = "Could not install a package " + e.Message;
                }
            }
        
            projectsUpdateList.State = $"done with MsPcakges for {prj.Name}";
        }
        private async Task
        UpdateSCPackages(SitecoreConfigModel sitecoreConfigModel, ProjectModel prj, IEnumerable<PackageModel> pkgToUpdate, IEnumerable<PackageModel> prjPkgs)
        {
            projectsUpdateList.State = $"Updating Sitecore packages for {prj.Name}";
            
            foreach (var oldPkg in pkgToUpdate)
            {
                try 
                {
                    var tempPkg = oldPkg;
                    if (tempPkg.Id.EndsWith(".NoReferences"))
                    {
                        projectsUpdateList.State = "uninstalling " + tempPkg.Id;
                        await pkgMnger.UninstallPackage(prj.DteProject, tempPkg, false);

                        tempPkg.Id = oldPkg.Id.Replace(".NoReferences", string.Empty);
                    }
                    tempPkg.Version = sitecoreConfigModel.SitecoreVersion;
                    projectsUpdateList.State = "installing " + tempPkg.Id + " with version " + tempPkg.Version;
                    await pkgMnger.UpdateScPackage(prj.DteProject, tempPkg, false);
                }catch (Exception e)
                {
                    projectsUpdateList.State = "Could not install a package " + e.Message;
                }
            }
            projectsUpdateList.State = $"done with Sitecore for {prj.Name}";
        }


        private async Task UpdateOtherPackages(SitecoreConfigModel sitecoreConfigModel, ProjectModel prj, IEnumerable<PackageModel> pkgToUpdate, IEnumerable<PackageModel> prjPkgs)
        {
            projectsUpdateList.State = $"Updating other packages for {prj.Name}";
            foreach (var oldPkg in pkgToUpdate)
            {
                var newPkg = sitecoreConfigModel.OtherPackages.FirstOrDefault(pkg => pkg.Id == oldPkg.Id);
                if (newPkg == null)
                {
                    projectsUpdateList.State = $"Sitecore {sitecoreConfigModel.SitecoreVersion} config does not have an equivelant for {oldPkg.Id}. Reinstalling the same version";
                    newPkg = oldPkg;
                }
                // this will be a pain if the user mistakenly upgrade packages first
                //else if(newPkg.Version == oldPkg.Version)
                //{
                //    projectsUpdateList.State = $"Package {oldPkg.Id} is already up to date with version {oldPkg.Version} ";
                //    continue;
                //}
                try
                {
                    await pkgMnger.UpdatePackage(prj.DteProject, newPkg, false);
                    projectsUpdateList.State = $"Package {oldPkg.Id} updated to version {newPkg.Version} ";
                }
                catch (Exception e)
                {
                    projectsUpdateList.State = "Could not install a package " + e.Message;
                }
            }

            projectsUpdateList.State = $"done with othe packages for {prj.Name}";
        }

        async void UpdateGlassPkgFired()
        {
            projectsUpdateList.State = "Started Glass Upgrade...";
            projectsUpdateList.State = "Loading Config...";
            var sitecoreConfigModel = SitecoreVersionConfigManager.GetSitecoreConfigModel(projectsUpdateList.SelectedSitecoreVersion.Id);

            if (sitecoreConfigModel.Error != null)
            {
                projectsUpdateList.State = sitecoreConfigModel.Error;
                return;
            }


            var selectedProjects = projectsUpdateList.Projects.Where(p => p.IsSelected);

            foreach (var prj in selectedProjects)
            {
                projectsUpdateList.State = "Starting project " + prj.Name;
                var prjPkgs = pkgMnger.GetInstalledNugetPackages(prj.DteProject);
                //restoring packages first
                await pkgMnger.RestorePackages(prj.DteProject);
                
                var pkgToUpdate = prjPkgs.Where(x => x.Id.StartsWith("Glass."));
                if (pkgToUpdate.Any())
                {
                    await UpdateGlassPackages(sitecoreConfigModel, prj, pkgToUpdate, prjPkgs);
                }
                //now build the project so it get saved and references get updated.
                await BuildProject(prj);
            }
            projectsUpdateList.Projects = LoadProjects();

            projectsUpdateList.State = "Glass has been updated. Please verify and continue";
        }

        private async Task UpdateGlassPackages(SitecoreConfigModel sitecoreConfigModel, ProjectModel prj, IEnumerable<PackageModel> pkgToUpdate, IEnumerable<PackageModel> prjPkgs)
        {
            var glassVersionText = "." + sitecoreConfigModel.GlassVersion;
            foreach (var oldPkg in pkgToUpdate)
            {
                var glsPkgNameWithoutVersion = string.IsNullOrEmpty(projectsUpdateList.CurrentGlassVersion) ? oldPkg.Id : oldPkg.Id.Replace(projectsUpdateList.CurrentGlassVersion, string.Empty);
                var newPkg = sitecoreConfigModel.GlassPackages.FirstOrDefault(pkg => pkg.Id.Replace(glassVersionText, string.Empty).Equals(glsPkgNameWithoutVersion,StringComparison.OrdinalIgnoreCase));
                if (newPkg == null)
                {
                    projectsUpdateList.State = $"Sitecore {sitecoreConfigModel.SitecoreVersion} config does not have an equivelant for {oldPkg.Id}. Reinstalling the same version";
                    newPkg = oldPkg;
                }
                else
                {
                    projectsUpdateList.State = "uninstalling " + oldPkg.Id;
                    await pkgMnger.UninstallPackage(prj.DteProject, oldPkg, false);
                }

                
                // this will be a pain if the user mistakenly upgrade packages first
                //else if(newPkg.Version == oldPkg.Version)
                //{
                //    projectsUpdateList.State = $"Package {oldPkg.Id} is already up to date with version {oldPkg.Version} ";
                //    continue;
                //}
                try
                {
                    await pkgMnger.UpdatePackage(prj.DteProject, newPkg, false);
                    projectsUpdateList.State = $"Package {oldPkg.Id} updated to version {newPkg.Version} ";
                }
                catch (Exception e)
                {
                    projectsUpdateList.State = "Could not install a package " + e.Message;
                }
            }

            projectsUpdateList.State = $"done with glass upgrade for {prj.Name}";
        }



        private async void MigrateToPackageReferencing()
        {
            projectsUpdateList.State = "Started package migration...";
            var sitecoreConfigModel = SitecoreVersionConfigManager.GetSitecoreConfigModel(projectsUpdateList.SelectedSitecoreVersion.Id);

            if (sitecoreConfigModel.Error != null)
            {
                projectsUpdateList.State = sitecoreConfigModel.Error;
                return;
            }


            var selectedProjects = projectsUpdateList.Projects.Where(p => p.IsSelected);
            var dte2 = (DTE2)applicationObject;
            foreach (var prj in selectedProjects)
            {
                pkgMnger.StartProjectMigration(dte2,applicationObject.Solution.FileName, prj.DteProject.FullName);
            }
        }

        private async Task BuildProject(ProjectModel prj)
        {
            //try
            //{
            //    prj.DteProject.DTE

            //    synchronizationContext.Post(o =>
            //    {
            //        var pm = (ProjectModel)o;
            //        projectsUpdateList.State = string.Format("Updating... {0} done", pm.Name);
            //    }, projectModel);
            //}
            //catch (COMException e) //possible "project unavailable" for unknown reasons
            //{
            //    projectsUpdateList.State = ("COMException on " + projectModel.Name + e);
            //}
        }
    }

}
