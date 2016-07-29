using JhpDataSystem.model;

namespace JhpDataSystem.projects.session
{
    public class SessionLookupProvider : ClientLookupProvider<SiteSession>
    {

        public SessionLookupProvider() : base(Constants.KIND_SITESESSION)
        {
            //todo add logic for everything to dbsaveentity and tablestore
            int y = 6;
        }
    }
}