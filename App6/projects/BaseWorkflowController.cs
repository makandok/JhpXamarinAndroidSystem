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

namespace JhpDataSystem.projects
{
    public interface IPP_NavController
    {
        System.Type GetNextActivity(int currentLayout, bool moveForward);
        int GetNextLayout(int currentLayout, bool moveForward);
        System.Type GetActivityForLayout(int targetLayout);
    }

    public class BaseWorkflowController : IPP_NavController
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
}