using System.Collections.Generic;
using System.Linq;
using JhpDataSystem.model;
using JhpDataSystem.store;

namespace JhpDataSystem.projects.vmmc
{
    public class ClientSummaryLoader
    {
        public List<VmmcClientSummary> Get()
        {
            var all = new LocalDB3().DB
                .Table<VmmcClientSummary>()
                .OrderBy(t => t.MCDate)
                .ToList();
            all.ForEach(t => { t.Id = new KindKey(t.KindKey); t.EntityId = new KindKey(t.KindKey); });
            return all;
        }

        public int Update(List<VmmcClientSummary> clients)
        {
            var all = new LocalDB3().DB
                .UpdateAll(clients);
            return all;
        }
    }
}