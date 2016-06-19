using System;
using Android.App;
using Android.OS;
using System.Collections.Generic;

namespace JhpDataSystem.modules
{
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

    [Activity(Label = "Client Evaluation - Pt2")]
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

    [Activity(Label = "Client Evaluation - Pt3")]
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

    [Activity(Label = "Client Evaluation - Pt4")]
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

    public class PP_ClientEvalControl: IPP_NavController
    {
        public static Dictionary<int, Type> MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.prepexreg1,typeof(PP_ClientEval1) },
            { Resource.Layout.prepexreg2,typeof(PP_ClientEval2) },
            { Resource.Layout.prepexreg3,typeof(PP_ClientEval3) },
            { Resource.Layout.prepexreg4,typeof(PP_ClientEval4) },
            { Resource.Layout.PrepexDataEntryEnd,typeof(PP_ClientEvalEnd) }
            };

        public static int[] MyLayouts = new[] {
                Resource.Layout.prepexreg1,
                Resource.Layout.prepexreg2,
                Resource.Layout.prepexreg3,
                Resource.Layout.prepexreg4,
                Resource.Layout.PrepexDataEntryEnd
            };

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
}