using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHQLabs.TargetFrameworkMigrator;

namespace Wheelbarrowex.Models
{
    public class SitecoreConfigModel
    {
        public SitecoreConfigModel()
        {
            otherPackages = new Dictionary<string, IEnumerable<PackageModel>>();
        }
        private Dictionary<string, IEnumerable<PackageModel>> otherPackages;
        public string Error { get; set; }
        public string SitecoreVersion { get; set; }
        public string SXAVersion { get; set; }
        public FrameworkModel Framework { get; set; }
        public IEnumerable<PackageModel> GlassPackages { get; set; }
        public IEnumerable<PackageModel> UnicornPackages { get; set; }
        public Dictionary<string, IEnumerable<PackageModel>> OtherPackages { get => otherPackages; set => otherPackages = value; }
    }
}
