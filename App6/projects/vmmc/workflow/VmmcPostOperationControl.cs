using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace JhpDataSystem.projects.vmmc.workflow
{
    public class VmmcPostOperationControl : BaseWorkflowController
    {
        public VmmcPostOperationControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.vmmc_postop1,typeof(VmmcPostOp1) },
            { Resource.Layout.vmmc_postop2,typeof(VmmcPostOp2) },
            { Resource.Layout.DataEntryEnd,typeof(VmmcPostOpEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.vmmc_postop1,
                Resource.Layout.vmmc_postop2,
                Resource.Layout.DataEntryEnd
            };
        }
    }
}