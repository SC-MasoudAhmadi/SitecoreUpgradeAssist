using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE80;

namespace WheelbarrowEx.SitecoreUpgradeAssist.Wheelbarrowex.Services
{
    public class GlassRefactorService
    {
        public string ErrorMessage;
        public bool AddAbstractionClass(EnvDTE.Project hostingProject, string folderName)
        {
            EnvDTE.ProjectItem folder;
            string folderFullName;
            string[] fileFullNames;

            try
            {
                folder = hostingProject.ProjectItems.AddFolder(folderName);

                folderFullName = folder.FileNames[0];

                CreateFiles(folderFullName);

                fileFullNames = System.IO.Directory.GetFiles(folderFullName);

                foreach (string fileFullName in fileFullNames)
                {
                    folder.ProjectItems.AddFromFile(fileFullName);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        private void CreateFiles(string folderFullName)
        {
            File.Copy(
                "D:\\Personal\\proj\\SitecoreUpgradeAssist\\Src\\SitecoreUpgradeAssist\\Resources\\IItemRepository.cs.resource",
                Path.Combine(folderFullName, "IItemRepository.cs"));
            File.Copy(
                "D:\\Personal\\proj\\SitecoreUpgradeAssist\\Src\\SitecoreUpgradeAssist\\Resources\\ItemRepository.cs.resource",
                Path.Combine(folderFullName, "ItemRepository.cs"));

            //AppendAllText(System.IO.Path.Combine(folderFullName, "IItemRepository.cs"),);
            //File.AppendAllText(System.IO.Path.Combine(folderFullName, "dummy2.cs"), "using System.Windows.Forms;");
        }
    }
}
