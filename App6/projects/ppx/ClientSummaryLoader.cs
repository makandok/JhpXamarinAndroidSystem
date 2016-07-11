using System.Collections.Generic;
using System.Linq;
using JhpDataSystem.model;
using JhpDataSystem.store;

namespace JhpDataSystem.projects.ppx
{
    public class ClientSummaryLoader
    {
        public List<PPClientSummary> Get()
        {
            var all = new LocalDB3().DB
                .Table<PPClientSummary>()
                .OrderBy(t => t.PlacementDate)
                .ToList();
            all.ForEach(t => { t.Id = new KindKey(t.KindKey); t.EntityId = new KindKey(t.KindKey); });
            return all;
        }

        public int Update(List<PPClientSummary> clients)
        {
            var all = new LocalDB3().DB
                .UpdateAll(clients);
            return all;
        }
    }
}