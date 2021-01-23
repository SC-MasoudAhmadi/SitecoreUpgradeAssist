namespace WheelbarrowEx.SitecoreUgradeAssist.GlassAbstraction
{
    using Sitecore.ContentSearch.Linq.Extensions;
    using System.Collections.Generic;
    using System.Reflection;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using System;
    using Glass.Mapper.Sc;
    using Glass.Mapper.Sc.Web;
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;

    public class ItemRepository : IItemRepository
    {
        public ItemRepository(IRequestContext request)
        {
            this.RequestContext = request;
        }
        
        private IRequestContext RequestContext { get; }


        public T GetItem<T>(Guid guid)
            where T : class
        {
            var service = this.RequestContext.SitecoreService.Database == null? new SitecoreService(this.Context.Database) : this.RequestContext.SitecoreService;
            
            return service.GetItem<T>(guid);
        }

        public T GetItem<T>(Item item, bool versionCount)
            where T : class
            => this.RequestContext.SitecoreService.GetItem<T>(new GetItemByItemOptions(item) {VersionCount = versionCount} );

        public T GetItem<T>(Item item)
            where T : class => GetItem<T>(item,false);

        public T GetContextItem<T>()
            where T : class => this.GetContextItem<T>(false);

        public T GetContextItem<T>(bool versionCount)
            where T : class => this.RequestContext.GetContextItem<T>(new GetKnownOptions(){VersionCount = versionCount});

        public T CastItem<T>(Item item)
            where T : class
            => this.RequestContext.SitecoreService.GetItem<T>(new GetItemByItemOptions(item)
            { VersionCount = false });

    }
}
