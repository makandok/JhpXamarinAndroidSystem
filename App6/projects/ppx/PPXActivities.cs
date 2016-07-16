using System;
using Android.App;
using Android.OS;
using System.Collections.Generic;
using JhpDataSystem.model;
using JhpDataSystem.projects.ppx.wfcontrollers;

namespace JhpDataSystem.projects.ppx
{
    [Activity(Label = "A.4: Post-Removal Visit Assessment - End")]
    public class PP_PostRemovalVisitEnd : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_POSTREMOVAL);
            myView = Resource.Layout.PrepexDataEntryEnd;
            myNavController = new PP_PostRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.4: Post-Removal Visit Assessment - 2")]
    public class PP_PostRemovalVisit2 : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_POSTREMOVAL);
            myView = Resource.Layout.prepexpostremoval2;
            myNavController = new PP_PostRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.4: Post-Removal Visit Assessment - Start")]
    public class PP_PostRemovalVisit1 : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            IsFirstPage = true;
            _kindName = new KindName(Constants.KIND_PPX_POSTREMOVAL);
            myView = Resource.Layout.prepexpostremoval1;
            myNavController = new PP_PostRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.3: Device Removal Visit or Follow-up Assessment - End")]
    public class PP_DeviceRemovalEnd : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_DEVICEREMOVAL);
            myView = Resource.Layout.PrepexDataEntryEnd;
            myNavController = new PP_DeviceRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.3: Device Removal Visit or Follow-up Assessment - 2")]
    public class PP_DeviceRemoval2 : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_DEVICEREMOVAL);
            myView = Resource.Layout.prepexdevremoval2;
            myNavController = new PP_DeviceRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.3: Device Removal Visit or Follow-up Assessment - Start")]
    public class PP_DeviceRemoval1 : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            IsFirstPage = true;
            _kindName = new KindName(Constants.KIND_PPX_DEVICEREMOVAL);
            myView = Resource.Layout.prepexdevremoval1;
            myNavController = new PP_DeviceRemovalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "A.2: Unscheduled or Follow-Up Prepex Assessment - 2")]
    public class PP_Unscheduled2 : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_UNSCHEDULEDVISIT);
            myView = Resource.Layout.prepexunscheduled2;
            myNavController = new PP_UnscheduledVisitControl() { };
            ShowMyView();
        }
    }

    [Activity(Label = "A.2: Unscheduled or Follow-Up Prepex Assessment - Start")]
    public class PP_Unscheduled1 : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            IsFirstPage = true;
            _kindName = new KindName(Constants.KIND_PPX_UNSCHEDULEDVISIT);
            myView = Resource.Layout.prepexunscheduled1;
            myNavController = new PP_UnscheduledVisitControl() {  };
            ShowMyView();
        }
    }

    [Activity(Label = "A.2: Unscheduled or Follow-Up Prepex Assessment - End")]
    public class PP_UnscheduledEnd : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_UNSCHEDULEDVISIT);
            myView = Resource.Layout.PrepexDataEntryEnd;
            myNavController = new PP_UnscheduledVisitControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - Start")]
    public class PP_ClientEval1 : PPXFormsBase
    {        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            IsFirstPage = true;
            _kindName = new KindName(Constants.KIND_PPX_CLIENTEVAL);
            myView = Resource.Layout.prepexreg1;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - 2")]
    public class PP_ClientEval2 : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_CLIENTEVAL);
            myView = Resource.Layout.prepexreg2;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - 3")]
    public class PP_ClientEval3 : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_CLIENTEVAL);
            myView = Resource.Layout.prepexreg3;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - 4")]
    public class PP_ClientEval4 : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_CLIENTEVAL);
            myView = Resource.Layout.prepexreg4;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - 5")]
    public class PP_ClientEval5 : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_CLIENTEVAL);
            myView = Resource.Layout.prepexreg5;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Client Evaluation - End")]
    public class PP_ClientEvalEnd : PPXFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            _kindName = new KindName(Constants.KIND_PPX_CLIENTEVAL);
            myView = Resource.Layout.PrepexDataEntryEnd;
            myNavController = new PP_ClientEvalControl();
            ShowMyView();
        }
    }
}