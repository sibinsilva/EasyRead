using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EasyRead.Droid;

[assembly : Xamarin.Forms.Dependency(typeof(NetworkConnection))]

namespace EasyRead.Droid
{
    public class NetworkConnection : INetworkChecker
    {
        public bool IsConnected { get; set; }

        [Obsolete]
        public void CheckInternetConnection()
        {
            var Connection = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
            var NetworkInfo = Connection.ActiveNetworkInfo;
            if (NetworkInfo != null && NetworkInfo.IsConnectedOrConnecting)
            {
                IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }
        }
    }
}