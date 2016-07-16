using JhpDataSystem.model;

namespace JhpDataSystem.projects.vmmc
{
    public class VmmcLookupProvider : ClientLookupProvider<VmmcClientSummary>
    {
        public VmmcLookupProvider() : base(Constants.KIND_VMMC_CLIENTSUMMARY)
        {
        }
    }
}