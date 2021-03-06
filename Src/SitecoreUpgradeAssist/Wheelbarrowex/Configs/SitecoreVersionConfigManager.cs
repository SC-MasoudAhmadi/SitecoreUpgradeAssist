﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VHQLabs.TargetFrameworkMigrator;
using Wheelbarrowex.Models;

namespace Wheelbarrowex.Configs
{
    public static class SitecoreVersionConfigManager
    {
        private static string _pakageConfigFolderPath;
        private static string PackageConfigFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(_pakageConfigFolderPath))
                {
                    _pakageConfigFolderPath = TargetFrameworkMigratorPackage.GetExtensionInstallationDirectory() +
                                              @"\Wheelbarrowex\Configs\VersionConfigs";
                    
                }

                return _pakageConfigFolderPath;
            }
        }

        public static SitecoreConfigModel GetSitecoreConfigModel(string sitecoreVersion)
        {
            var result = new SitecoreConfigModel();
            var otherPackages = new List<PackageModel>();
            var config = new XmlDocument();
            try
            {
                config.Load(Path.Combine(PackageConfigFolderPath, $"{sitecoreVersion}.xml"));
                foreach (XmlNode node in config.DocumentElement.ChildNodes)
                {
                    if(node.Name == "SitecorePkgVersion")
                    {
                        result.SitecoreVersion = node.Attributes["Value"].Value;
                    }else if (node.Name == "Framework")
                    {
                        result.Framework = new FrameworkModel { Id = uint.Parse(node.Attributes["id"].Value), Name = node.Attributes["name"].Value };
                    }
                    else if (node.Name == "UnicornPackages")
                    {
                        result.UnicornPackages = ReadChildPackages(node);
                    }
                    else if (node.Name == "GlassPackages")
                    {
                        result.GlassVersion = node.Attributes["version"].Value;
                        result.GlassPackages = ReadChildPackages(node);
                    }
                    else if (node.Name == "MSPackages")
                    {
                        result.MSPackages = ReadChildPackages(node);
                    }
                    else
                    {
                        otherPackages.AddRange(ReadChildPackages(node));
                    }
                }
                result.OtherPackages = otherPackages;
                   
            }
            catch(Exception e)
            {

                result.Error = "Could not read configuration for file " +
                    Path.Combine(PackageConfigFolderPath, $"{sitecoreVersion}.xml") +
                    e.Message + Environment.NewLine + e.StackTrace;
            }
            return result;
        }

        private static IEnumerable<PackageModel> ReadChildPackages(XmlNode node)
        {
            var result = new List<PackageModel>();
            foreach (XmlNode child in node.ChildNodes)
                result.Add(new PackageModel { Id = child.Attributes["id"].Value, Version = child.Attributes["version"].Value });

            return result;
        }

        public static IEnumerable<SitecoreVersionModel> GetSupportedSitecoreVersions()
        {
            var result = new List<SitecoreVersionModel>();
            Debug.WriteLine(Path.Combine(PackageConfigFolderPath, "SitecoreVersions.xml"));
            var availableVersions = new XmlDocument();
            availableVersions.Load(Path.Combine(PackageConfigFolderPath, "SitecoreVersions.xml"));
            foreach (XmlNode node in availableVersions.DocumentElement.ChildNodes)
                result.Add(new SitecoreVersionModel { Id = node.Attributes["Id"].Value, Name = node.Attributes["Name"].Value });
            return result;
        }
    }
}
