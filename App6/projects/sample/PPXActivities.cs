using System;
using Android.App;
using Android.OS;
using System.Collections.Generic;
using JhpDataSystem.model;

namespace JhpDataSystem.projects.sample
{
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