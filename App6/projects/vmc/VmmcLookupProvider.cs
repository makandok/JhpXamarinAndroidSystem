using JhpDataSystem.model;

namespace JhpDataSystem.projects.vmc
{
    public class VmmcLookupProvider: ClientLookupProvider<VmmcClientSummary>
    {
        public VmmcLookupProvider():base(Constants.KIND_VMMC_CLIENTSUMMARY)
        {
        }
    }
}