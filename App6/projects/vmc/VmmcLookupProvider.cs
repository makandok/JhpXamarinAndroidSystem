namespace JhpDataSystem.projects.vmc
{
    public class VmmcLookupProvider: ClientLookupProvider<VmmcClientSummary>
    {
        public VmmcLookupProvider():base(Constants.KIND_DERIVED_VMMC_CLIENTSUMMARY)
        {
        }
    }
}