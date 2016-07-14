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

namespace JhpDataSystem.projects.sample.workflow
{
    public class PP_DeviceRemovalControl : BaseWorkflowController
    {
        public PP_DeviceRemovalControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.prepexdevremoval1,typeof(PP_DeviceRemoval1) },
            { Resource.Layout.prepexdevremoval2,typeof(PP_DeviceRemoval2) },
            { Resource.Layout.PrepexDataEntryEnd,typeof(PP_DeviceRemovalEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.prepexdevremoval1,
                Resource.Layout.prepexdevremoval2,
                Resource.Layout.PrepexDataEntryEnd
            };
        }
    }

    public class PP_ClientEvalControl : BaseWorkflowController
    {
        public PP_ClientEvalControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.prepexreg1,typeof(PP_ClientEval1) },
            { Resource.Layout.prepexreg2,typeof(PP_ClientEval2) },
            { Resource.Layout.PrepexDataEntryEnd,typeof(PP_ClientEvalEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.prepexreg1,
                Resource.Layout.prepexreg2,
                Resource.Layout.prepexreg3,
                Resource.Layout.prepexreg4,
                Resource.Layout.prepexreg5,
                Resource.Layout.PrepexDataEntryEnd
            };
        }
    }

}