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
        private Dictionary<string, IEnumerable<PackageModel>> otherPackages;
        public string Error { get; set; }
        public string SitecoreVersion { get; set; }
        public string SXAVersion { get; set; }
        public string GlassVersion { get; set; }
        public FrameworkModel Framework { get; set; }
        public IEnumerable<PackageModel> GlassPackages { get; set; }
        
        public IEnumerable<PackageModel> UnicornPackages { get; set; }
        public IEnumerable<PackageModel> MSPackages { get; set; }
        public IEnumerable<PackageModel> OtherPackages { get; set; }
    }
}
