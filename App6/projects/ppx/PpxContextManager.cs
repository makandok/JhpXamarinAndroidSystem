using Android.App;
using Android.Content.Res;

namespace JhpDataSystem.projects
{
    public class PpxContextManager : BaseContextManager
    {
        public PpxContextManager(AssetManager assetManager, Activity mainContext)
            : base(Constants.FILE_PPX_FIELDS, assetManager, mainContext)
        {
            ProjectCtxt = ProjectContext.Ppx;
        }
    }
}