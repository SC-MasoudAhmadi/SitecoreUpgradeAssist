using System;

namespace Wheelbarrowex.Models
{
    public class SitecoreVersionModel : IComparable
    {
        public string Name
        {
            get; set;
        }
        public string Id
        {
            get; set;
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }

        // support comparison so we can sort by properties of this type
        public int CompareTo(object obj)
        {
            return StringComparer.Ordinal.Compare(this.Name, ((SitecoreVersionModel)obj).Name);
        }
    }
}