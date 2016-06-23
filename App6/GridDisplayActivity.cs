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
using JhpDataSystem.model;
using JhpDataSystem.store;

namespace JhpDataSystem
{
    [Activity(Label = "GridDisplayActivity")]
    public class GridDisplayActivity : ListActivity
    {
        List<PrepexClientSummary> clients;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var all = new LocalDB3().DB
                .Table<PrepexClientSummary>()
                .OrderBy(t => t.PlacementDate)
                .ToList();
            clients = all;
            var adapter = new ClientSummaryAdapter(this);
            adapter._myList.AddRange(clients);
            //ListAdapter = new ArrayAdapter<PrepexClientSummary>(this, 
            //    Android.Resource.Layout.SimpleListItem1, all);
            ListView.FastScrollEnabled = true;
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var t = clients[position];
            Android.Widget.Toast.MakeText(this, t.Names, Android.Widget.ToastLength.Short).Show();
        }

        public class ClientSummaryAdapter : BaseAdapter<PrepexClientSummary>
        {
            ListActivity _context;
            public ClientSummaryAdapter(ListActivity context)
            {
                _context = context;
            }

            public 

            List<PrepexClientSummary> _myList = new List<PrepexClientSummary>();
            public override PrepexClientSummary this[int position]
            {
                get
                {
                    return _myList[position];
                }
            }

            public override int Count
            {
                get
                {
                    return _myList.Count;
                }
            }

            public override long GetItemId(int position)
            {
                return _myList[position].Id.Value.GetHashCode();
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                if (convertView == null)
                {
                    convertView = _context.LayoutInflater.Inflate(Resource.Layout.clientsummary, parent);
                }
                return convertView;
            }
        }

        //public override View GetView(int position, View convertView, ViewGroup parent)
        //{
        //    View view = convertView; // re-use an existing view, if one is available
        //    if (view == null) // otherwise create a new one
        //        view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
        //    view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position];
        //    return view;
        //}
    }
}