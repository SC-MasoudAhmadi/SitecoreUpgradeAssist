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

                //projectsUpdateList.State = "Waiting all projects are loaded...";

                if (applicationObject.Solution != null)
                {
                    if (isSolutionLoaded)
                        ReloadProjects();
                }

                projectsUpdateList.StartPosition = FormStartPosition.CenterParent;
                projectsUpdateList.ShowDialog();

            }
        }


        void Update(Action<int,object> progressReport)
        {
            progressReport(-1,"Reading config file...");
            var selectedProjects = projectsUpdateList.Projects.Where(p => p.IsSelected); 
            var sitecoreConfigModel = SitecoreVersionConfigManager.GetSitecoreConfigModel(projectsUpdateList.SelectedSitecoreVersion.Id);

            if (sitecoreConfigModel.Error != null)
            {
                progressReport(100,sitecoreConfigModel.Error);
                return;
            }

            progressReport(-1,"Updating dotnet Framework...");

            UpdateFrameworks(selectedProjects, sitecoreConfigModel.Framework,progressReport);

            //projectsUpdateList.EnableNextStep();
            //projectsUpdateList.Projects = LoadProjects();

            progressReport(100,"Dotnet Framework has been updated... please verify and then continue");
        }
        private void UpdateFrameworks(IEnumerable<ProjectModel> selectedProjects, FrameworkModel frameworkModel,
            Action<int, object> progressReport)
        {
            var count = selectedProjects.Count();
            var i = 1;
            foreach (var projectModel in selectedProjects)
            {

                try
                {
                    projectModel.DteProject.Properties.Item("TargetFrameworkMoniker").Value = frameworkModel.Name;
                    progressReport(GetPercentage(i,count), $"Updating... {projectModel.Name} done");
                }
                catch (COMException e) //possible "project unavailable" for unknown reasons
                {
                    progressReport(-1,"COMException on " + projectModel.Name + e);
                }

                i++;
            }
        }


        void UpdateMSSCPkgFired(Action<int,object> progressReport)
        {
            progressReport(0,"Started SC + MS Update...");
            var sitecoreConfigModel = SitecoreVersionConfigManager.GetSitecoreConfigModel(projectsUpdateList.SelectedSitecoreVersion.Id);

            if (sitecoreConfigModel.Error != null)
            {
                progressReport(-1, sitecoreConfigModel.Error);
                return;
            }
            

            var selectedProjects = projectsUpdateList.Projects.Where(p => p.IsSelected);
            var count = selectedProjects.Count();
            var i = 1;
            foreach (var prj in selectedProjects)
            {
                progressReport(GetPercentage(i, count), $"Starting project {prj.Name}");
                var prjPkgs = pkgMnger.GetInstalledNugetPackages(prj.DteProject);
                //restoring packages first
                pkgMnger.RestorePackages(prj.DteProject);
                //MSPackages first
                var pkgToUpdate = prjPkgs.Where(x => x.Id.StartsWith("Microsoft."));
                if (pkgToUpdate.Any())
                {
                    UpdateMsPackages(sitecoreConfigModel, prj, pkgToUpdate, prjPkgs,progressReport);
                }

                //Sitecore package second
                pkgToUpdate = prjPkgs.Where(x => x.Id.StartsWith("Sitecore."));
                if (pkgToUpdate.Any())
                {
                    UpdateSCPackages(sitecoreConfigModel, prj, pkgToUpdate, prjPkgs,progressReport);
                }

                //other packages
                pkgToUpdate = prjPkgs.Where(x => !x.Id.StartsWith("Sitecore.") && !x.Id.StartsWith("Microsoft.") && !x.Id.StartsWith("Glass."));
                if (pkgToUpdate.Any())
                {
                    UpdateOtherPackages(sitecoreConfigModel, prj, pkgToUpdate, prjPkgs,progressReport);
                }

                //now build the project so it get saved and references get updated.
                BuildProject(prj);
                i++;
            }

            progressReport(100, "MS + Sitecore has been updated. Please verify and continue");
        }

        private void
            UpdateMsPackages(SitecoreConfigModel sitecoreConfigModel, ProjectModel prj,
                IEnumerable<PackageModel> pkgToUpdate, IEnumerable<PackageModel> prjPkgs,
                Action<int, object> progressReport)
        {
            progressReport(-1,$"Updating MS packages for {prj.Name}");
            foreach (var oldPkg in pkgToUpdate)
            {
                try
                {
                    var newPkg = sitecoreConfigModel.MSPackages.FirstOrDefault(pkg => pkg.Id == oldPkg.Id);
                    if(newPkg == null)
                    {
                        progressReport(-1,$"Sitecore {sitecoreConfigModel.SitecoreVersion} config does not have an equivalent for {oldPkg.Id}. Reinstalling the same version");
                        //await pkgMnger.UninstallPackage(prj.DteProject, oldPkg, false);
                        newPkg = oldPkg;
                    }
                    // this will be a pain if the user mistakenly upgrade packages first
                    //else if(newPkg.Version == oldPkg.Version)
                    //{
                    //    projectsUpdateList.State = $"Package {oldPkg.Id} is already up to date with version {oldPkg.Version} ";
                    //    continue;
                    //}
                
                    pkgMnger.UpdatePackage(prj.DteProject, newPkg, false);
                    progressReport(-1, $"Package {oldPkg.Id} updated to version {newPkg.Version} ");
                }
                catch (Exception e)
                {
                    progressReport(-1,"Could not install a package " + e.Message);
                }
            }
        
            progressReport(-1,$"done with MsPcakges for {prj.Name}");
        }
        private void
            UpdateSCPackages(SitecoreConfigModel sitecoreConfigModel, ProjectModel prj,
                IEnumerable<PackageModel> pkgToUpdate, IEnumerable<PackageModel> prjPkgs,
                Action<int, object> progressReport)
        {
            progressReport(-1,$"Updating Sitecore packages for {prj.Name}");
            
            foreach (var oldPkg in pkgToUpdate)
            {
                try 
                {
                    var tempPkg = oldPkg;
                    if (tempPkg.Id.EndsWith(".NoReferences"))
                    {
                        progressReport(-1,"uninstalling " + tempPkg.Id);
                        pkgMnger.UninstallPackage(prj.DteProject, tempPkg, false);

                        tempPkg.Id = oldPkg.Id.Replace(".NoReferences", string.Empty);
                    }
                    tempPkg.Version = sitecoreConfigModel.SitecoreVersion;
                    progressReport(-1,"installing " + tempPkg.Id + " with version " + tempPkg.Version);
                    pkgMnger.UpdateScPackage(prj.DteProject, tempPkg, false);
                }catch (Exception e)
                {
                    progressReport(-1, "Could not install a package " + e.Message);
                }
            }
            progressReport(-1, $"done with Sitecore for {prj.Name}");
        }


        private void UpdateOtherPackages(SitecoreConfigModel sitecoreConfigModel, ProjectModel prj,
            IEnumerable<PackageModel> pkgToUpdate, IEnumerable<PackageModel> prjPkgs,
            Action<int, object> progressReport)
        {
            progressReport(-1,$"Updating other packages for {prj.Name}");
            foreach (var oldPkg in pkgToUpdate)
            {
                var newPkg = sitecoreConfigModel.OtherPackages.FirstOrDefault(pkg => pkg.Id == oldPkg.Id);
                if (newPkg == null)
                {
                    progressReport(-1, $"Sitecore {sitecoreConfigModel.SitecoreVersion} config does not have an equivelant for {oldPkg.Id}. Reinstalling the same version");
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
                    pkgMnger.UpdatePackage(prj.DteProject, newPkg, false);
                    progressReport(-1, $"Package {oldPkg.Id} updated to version {newPkg.Version} ");
                }
                catch (Exception e)
                {
                    progressReport(-1, "Could not install a package " + e.Message);
                }
            }

            progressReport(100,$"done with othe packages for {prj.Name}");
        }

        void UpdateGlassPkgFired(Action<int,object> progressReport)
        {
            progressReport(-1, "Started Glass Upgrade...");
            progressReport(-1, "Loading Config...");
            var sitecoreConfigModel = SitecoreVersionConfigManager.GetSitecoreConfigModel(projectsUpdateList.SelectedSitecoreVersion.Id);

            if (sitecoreConfigModel.Error != null)
            {
                progressReport(-1, sitecoreConfigModel.Error);
                return;
            }


            var selectedProjects = projectsUpdateList.Projects.Where(p => p.IsSelected);
            var count = selectedProjects.Count();
            var i = 1;
            foreach (var prj in selectedProjects)
            {
                progressReport(GetPercentage(i,count), "Starting project " + prj.Name);
                var prjPkgs = pkgMnger.GetInstalledNugetPackages(prj.DteProject);
                //restoring packages first
                pkgMnger.RestorePackages(prj.DteProject);
                
                var pkgToUpdate = prjPkgs.Where(x => x.Id.StartsWith("Glass."));
                if (pkgToUpdate.Any())
                {
                    UpdateGlassPackages(sitecoreConfigModel, prj, pkgToUpdate, prjPkgs, progressReport);
                }
                //now build the project so it get saved and references get updated.
                BuildProject(prj);
                i++;
            }

            progressReport(-1, "Glass has been updated. Please verify and continue");
        }

        private void UpdateGlassPackages(SitecoreConfigModel sitecoreConfigModel, ProjectModel prj,
            IEnumerable<PackageModel> pkgToUpdate, IEnumerable<PackageModel> prjPkgs,
            Action<int, object> progressReport)
        {
            var glassVersionText = "." + sitecoreConfigModel.GlassVersion;
            foreach (var oldPkg in pkgToUpdate)
            {
                var glsPkgNameWithoutVersion = string.IsNullOrEmpty(projectsUpdateList.CurrentGlassVersion) ? oldPkg.Id : oldPkg.Id.Replace(projectsUpdateList.CurrentGlassVersion, string.Empty);
                var newPkg = sitecoreConfigModel.GlassPackages.FirstOrDefault(pkg => pkg.Id.Replace(glassVersionText, string.Empty).Equals(glsPkgNameWithoutVersion,StringComparison.OrdinalIgnoreCase));
                if (newPkg == null)
                {
                    progressReport(-1, $"Sitecore {sitecoreConfigModel.SitecoreVersion} config does not have an equivelant for {oldPkg.Id}. Reinstalling the same version");
                    newPkg = oldPkg;
                }
                else
                {
                    progressReport(-1,"uninstalling " + oldPkg.Id);
                    pkgMnger.UninstallPackage(prj.DteProject, oldPkg, false);
                }

                
                // this will be a pain if the user mistakenly upgrade packages first
                //else if(newPkg.Version == oldPkg.Version)
                //{
                //    projectsUpdateList.State = $"Package {oldPkg.Id} is already up to date with version {oldPkg.Version} ";
                //    continue;
                //}
                try
                {
                    pkgMnger.UpdatePackage(prj.DteProject, newPkg, false);
                    progressReport(-1,$"Package {oldPkg.Id} updated to version {newPkg.Version} ");
                }
                catch (Exception e)
                {
                    progressReport(-1,"Could not install a package " + e.Message);
                }
            }

            progressReport(-1,$"done with glass upgrade for {prj.Name}");
        }



        private void MigrateToPackageReferencing(Action<int,object> progressReport)
        {
            progressReport(-1,"Started package migration...");
            var sitecoreConfigModel = SitecoreVersionConfigManager.GetSitecoreConfigModel(projectsUpdateList.SelectedSitecoreVersion.Id);

            if (sitecoreConfigModel.Error != null)
            {
                progressReport(-1,sitecoreConfigModel.Error);
                return;
            }


            var selectedProjects = projectsUpdateList.Projects.Where(p => p.IsSelected);
            var dte2 = (DTE2)applicationObject;
            foreach (var prj in selectedProjects)
            {
                pkgMnger.StartProjectMigration(dte2,applicationObject.Solution.FileName, prj.DteProject.FullName);
            }
        }

        private void BuildProject(ProjectModel prj)
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

        private int GetPercentage(int i, int count) => (i * 100 / count);
    }

}
