using JhpDataSystem.model;

namespace JhpDataSystem.projects.vmc
{
    public class VmmcLookupProvider: ClientLookupProvider<PPClientSummary>
    {
        public VmmcLookupProvider():base(Constants.KIND_PPX_CLIENTSUMMARY)
        {
        }
    }
}