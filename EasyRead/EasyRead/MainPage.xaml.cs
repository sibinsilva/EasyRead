using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EasyRead
{
    public partial class MainPage : ContentPage
    {
        public bool CloseApp = true;
        public MainPage()
        {
            InitializeComponent();
            App.CheckInternetConnectivity(this.lbl_NoInternet, this);
        }

        private void Button_ClickedAsync(object sender, EventArgs e)
        {
            App.Current.MainPage = new AppPage();
        }

        private async Task<bool> CheckInternetConnection()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                return true;
            }
             else
            {
                await DisplayAlert("No Internet Connection Detected", "Application Cannot Start without Internet Access", "Retry");
                return false;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (!CloseApp)
                return false;

            // don't exit the app when back button is pressed
            Device.BeginInvokeOnMainThread(async () =>
            {
                CloseApp = await DisplayAlert("EasyRead", "Do you want to exit the application?", "No", "Yes");
                if (!CloseApp)
                {
                    Process.GetCurrentProcess().CloseMainWindow();
                    Process.GetCurrentProcess().Close();
                }
                else
                {
                    base.OnBackButtonPressed();
                }
            });
            return CloseApp;
        }
    }
}
