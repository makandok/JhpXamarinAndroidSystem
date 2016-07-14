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
    public class VmmcRegAndProcControl : BaseWorkflowController
    {
        public VmmcRegAndProcControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.vmmc_regandproc1,typeof(VmmcRegAndProc1) },
            { Resource.Layout.vmmc_regandproc2,typeof(VmmcRegAndProc2) },
            { Resource.Layout.vmmc_regandproc3,typeof(VmmcRegAndProc3) },
            { Resource.Layout.vmmc_regandproc4,typeof(VmmcRegAndProc4) },
            { Resource.Layout.vmmc_regandproc5,typeof(VmmcRegAndProc5) },
            { Resource.Layout.DataEntryEnd,typeof(VmmcRegAndProcEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.vmmc_regandproc1,
                Resource.Layout.vmmc_regandproc2,
                Resource.Layout.vmmc_regandproc3,
                Resource.Layout.vmmc_regandproc4,
                Resource.Layout.vmmc_regandproc5,
                Resource.Layout.DataEntryEnd
            };
        }
    }
}