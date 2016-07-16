using JhpDataSystem.model;

namespace JhpDataSystem.projects.ppx
{
    public class PpxLookupProvider: ClientLookupProvider<PPClientSummary>
    {
        public PpxLookupProvider():base(Constants.KIND_PPX_CLIENTSUMMARY)
        {
        }
    }
}