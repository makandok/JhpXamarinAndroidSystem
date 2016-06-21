using System;
using Android.App;
using Android.OS;
using System.Collections.Generic;

namespace JhpDataSystem.modules
{
    [Activity(Label = "A.4: Post-Removal Visit Assessment - End")]
    public class PP_PostRemovalVisitEnd : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.PrepexDataEntryEnd;
            myNavController = new PP_PostRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.4: Post-Removal Visit Assessment - 2")]
    public class PP_PostRemovalVisit2 : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexpostremoval2;
            myNavController = new PP_PostRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.4: Post-Removal Visit Assessment - Start")]
    public class PP_PostRemovalVisit1 : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexpostremoval1;
            myNavController = new PP_PostRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.3: Device Removal Visit or Follow-up Assessment - End")]
    public class PP_DeviceRemovalEnd : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.PrepexDataEntryEnd;
            myNavController = new PP_DeviceRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.3: Device Removal Visit or Follow-up Assessment - 2")]
    public class PP_DeviceRemoval2 : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexdevremoval2;
            myNavController = new PP_DeviceRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.3: Device Removal Visit or Follow-up Assessment - Start")]
    public class PP_DeviceRemoval1 : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexdevremoval1;
            myNavController = new PP_DeviceRemovalControl();
            ShowMyView();
        }
    }







    [Activity(Label = "A.2: Unscheduled or Follow-Up Prepex Assessment - 2")]
    public class PP_Unscheduled2 : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexunscheduled2;
            myNavController = new PP_UnscheduledVisitControl() { };
            ShowMyView();
        }
    }

    [Activity(Label = "A.2: Unscheduled or Follow-Up Prepex Assessment - Start")]
    public class PP_Unscheduled1 : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexunscheduled1;
            myNavController = new PP_UnscheduledVisitControl() {  };
            ShowMyView();
        }
    }

    [Activity(Label = "A.2: Unscheduled or Follow-Up Prepex Assessment - End")]
    public class PP_UnscheduledEnd : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.PrepexDataEntryEnd;
            myNavController = new PP_UnscheduledVisitControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - Start")]
    public class PP_ClientEval1 : PrepexFormsBase
    {        
        protected override void OnCreate(Bundle savedInstanceState)
        {            
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexreg1;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - 2")]
    public class PP_ClientEval2 : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexreg2;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - 3")]
    public class PP_ClientEval3 : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexreg3;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - 4")]
    public class PP_ClientEval4 : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexreg4;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - 5")]
    public class PP_ClientEval5 : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.prepexreg5;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - End")]
    public class PP_ClientEvalEnd : PrepexFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            myView = Resource.Layout.PrepexDataEntryEnd;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    //PP_PostRemovalControl
    public class PP_PostRemovalControl : PP_BaseWorkflowController
    {
        public PP_PostRemovalControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.prepexpostremoval1,typeof(PP_PostRemovalVisit1) },
            { Resource.Layout.prepexpostremoval2,typeof(PP_PostRemovalVisit2) },
            { Resource.Layout.PrepexDataEntryEnd,typeof(PP_PostRemovalVisitEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.prepexpostremoval1,
                Resource.Layout.prepexpostremoval2,
                Resource.Layout.PrepexDataEntryEnd
            };
        }
    }

    public class PP_DeviceRemovalControl : PP_BaseWorkflowController
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

    public class PP_UnscheduledVisitControl : PP_BaseWorkflowController
    {
        public PP_UnscheduledVisitControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.prepexunscheduled1,typeof(PP_Unscheduled1) },
            { Resource.Layout.prepexunscheduled2,typeof(PP_Unscheduled2) },
            { Resource.Layout.PrepexDataEntryEnd,typeof(PP_UnscheduledEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.prepexunscheduled1,
                Resource.Layout.prepexunscheduled2,
                Resource.Layout.PrepexDataEntryEnd
            };
        }
    }

    public class PP_BaseWorkflowController : IPP_NavController
    {
        public Dictionary<int, Type> MyActivities;

        public int[] MyLayouts;

        public System.Type GetNextActivity(int currentLayout, bool moveForward)
        {
            var targetLayout = GetNextLayout(currentLayout, moveForward);
            return GetActivityForLayout(targetLayout);
        }

        public System.Type GetActivityForLayout(int targetLayout)
        {
            return MyActivities[targetLayout];
        }

        public int GetNextLayout(int currentLayout, bool moveForward)
        {
            var targetIndex = -1;
            var currIndx = Array.IndexOf(MyLayouts, currentLayout);
            var max = MyLayouts.Length;
            if (currIndx == -1)
            {
                return -1;
            }
            var next = moveForward ? currIndx + 1 : currIndx - 1;
            targetIndex = moveForward ? (next >= max ? currentLayout : next) :
                targetIndex = next < 0 ? 0 : next;
            var targetLayout = MyLayouts[targetIndex];
            return targetLayout;
        }
    }

    public class PP_ClientEvalControl: PP_BaseWorkflowController
    {
        public PP_ClientEvalControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.prepexreg1,typeof(PP_ClientEval1) },
            { Resource.Layout.prepexreg2,typeof(PP_ClientEval2) },
            { Resource.Layout.prepexreg3,typeof(PP_ClientEval3) },
            { Resource.Layout.prepexreg4,typeof(PP_ClientEval4) },
            { Resource.Layout.prepexreg5,typeof(PP_ClientEval5) },
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