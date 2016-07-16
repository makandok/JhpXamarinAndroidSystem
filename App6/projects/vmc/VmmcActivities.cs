using Android.App;
using Android.OS;
using JhpDataSystem.model;
using JhpDataSystem.projects.vmc.workflow;

namespace JhpDataSystem.projects.vmc
{
    [Activity(Label = "Post Operation - End")]
    public class VmmcPostOpEnd : VmmcFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _kindName = new KindName(Constants.KIND_VMMC_POSTOP);
            myView = Resource.Layout.DataEntryEnd;
            myNavController = new VmmcPostOperationControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Post Operation - 2")]
    public class VmmcPostOp2 : VmmcFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _kindName = new KindName(Constants.KIND_VMMC_POSTOP);
            myView = Resource.Layout.vmmc_postop2;
            myNavController = new VmmcPostOperationControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Post Operation - Start")]
    public class VmmcPostOp1 : VmmcFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            IsFirstPage = true;
            _kindName = new KindName(Constants.KIND_VMMC_POSTOP);
            myView = Resource.Layout.vmmc_postop1;
            myNavController = new VmmcPostOperationControl();
            ShowMyView();
        }
    }
    
    [Activity(Label = "Registration and Procedure - Start")]
    public class VmmcRegAndProc1 : VmmcFormsBase
    {        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            IsFirstPage = true;
            _kindName = new KindName(Constants.KIND_VMMC_REGANDPROCEDURE);
            myView = Resource.Layout.vmmc_regandproc1;
            myNavController = new VmmcRegAndProcControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Registration and Procedure - 2")]
    public class VmmcRegAndProc2 : VmmcFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _kindName = new KindName(Constants.KIND_VMMC_REGANDPROCEDURE);
            myView = Resource.Layout.vmmc_regandproc2;
            myNavController = new VmmcRegAndProcControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Registration and Procedure - 3")]
    public class VmmcRegAndProc3 : VmmcFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _kindName = new KindName(Constants.KIND_VMMC_REGANDPROCEDURE);
            myView = Resource.Layout.vmmc_regandproc3;
            myNavController = new VmmcRegAndProcControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Registration and Procedure - 4")]
    public class VmmcRegAndProc4 : VmmcFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _kindName = new KindName(Constants.KIND_VMMC_REGANDPROCEDURE);
            myView = Resource.Layout.vmmc_regandproc4;
            myNavController = new VmmcRegAndProcControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Registration and Procedure - 5")]
    public class VmmcRegAndProc5 : VmmcFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _kindName = new KindName(Constants.KIND_VMMC_REGANDPROCEDURE);
            myView = Resource.Layout.vmmc_regandproc5;
            myNavController = new VmmcRegAndProcControl();
            ShowMyView();
        }
    }

    [Activity(Label = "Registration and Procedure - End")]
    public class VmmcRegAndProcEnd : VmmcFormsBase
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _kindName = new KindName(Constants.KIND_VMMC_REGANDPROCEDURE);
            myView = Resource.Layout.DataEntryEnd;
            myNavController = new VmmcRegAndProcControl();
            ShowMyView();
        }
    }
}