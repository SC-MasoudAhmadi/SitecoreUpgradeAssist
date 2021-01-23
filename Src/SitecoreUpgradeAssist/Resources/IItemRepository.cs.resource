
namespace WheelbarrowEx.SitecoreUgradeAssist.GlassAbstraction
{
    using System;
    using System.Collections.Generic;
    using Sitecore.Data.Items;

    public interface IItemRepository
    {
        /// <summary>
        /// Returns the descendants of the parent item which inherit from the given glass type
        /// </summary>
        /// <typeparam name="T">The base template glass type</typeparam>
        /// <param name="parentItem">the parent item</param>
        /// <returns>list of parent item's descendents that inherit from <typeparam name="T"></typeparam> name="T"></returns>
        IEnumerable<Item> GetDescendents<T>(Item parentItem);

        Item GetItem(string path);

        T GetItem<T>(Guid guid)
            where T : class;
        T GetItem<T>(Item item, bool versionCount)
            where T : class;

        T GetItem<T>(Item item)
            where T : class;

        T GetContextItem<T>()
            where T : class;

        T GetContextItem<T>(bool versionCount)
            where T : class;

        T CastItem<T>(Item item)
            where T : class;
    }
}
