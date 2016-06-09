using JhpDataSystem.model;
using System.Linq;

namespace JhpDataSystem.store
{
    internal class KindRegistryQuery: TableStore
    {
        public KindRegistryQuery() : base("kindRegister")
        {

        }

        internal KindItem GetKindForKey(KindKey entityId)
        {
            return Get(entityId).FirstOrDefault();
        }        
    }
}