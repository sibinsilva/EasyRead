using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasyRead
{
    public partial class App : Application
    {
        private static Label labelscreen;
        private static Timer timer;
        private static bool IsInternetAvailable;
        private static Page CurrentPage;
        public static bool NoInternetShowAlert;

        public App()
        {
            InitializeComponent(); 
            MainPage = new MainPage();
        }

        public static void CheckInternetConnectivity(Label label,Page page)
        {
            labelscreen = label;
            label.Text = Generic.NoInternet;
            label.IsVisible = false;
            IsInternetAvailable = true;
            CurrentPage = page;
            if(timer==null)
            {
                timer = new Timer((e) =>
                    {
                        CheckInternetOverTime();
                    }, null, 10, (int)TimeSpan.FromSeconds(3).TotalMilliseconds);
            }
        }

        private static void CheckInternetOverTime()
        {
            var NetworkConnection = DependencyService.Get<INetworkChecker>();
            NetworkConnection.CheckInternetConnection();
            if(!NetworkConnection.IsConnected)
            {
                Device.BeginInvokeOnMainThread(async() =>
                {
                    if (IsInternetAvailable)
                    {
                        if (!NoInternetShowAlert)
                        {
                            IsInternetAvailable = false;
                            labelscreen.IsVisible = true;
                            await ShowDisplayAlert();
                        }
                    }
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsInternetAvailable = true;
                    labelscreen.IsVisible = false;
                });
            }
        }

        private static async Task ShowDisplayAlert()
        {
            NoInternetShowAlert = false;
            await CurrentPage.DisplayAlert("Internet", "Internet Connection Lost. Please Reconnect", "Ok");
            NoInternetShowAlert = false;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
