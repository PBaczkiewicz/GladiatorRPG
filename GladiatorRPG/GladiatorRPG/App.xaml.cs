
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GladiatorRPG
{
    public partial class App : Application
    {
        public static bool AppActive { get; set; }
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainTabPage());
            NavigationPage.SetHasNavigationBar(this, false);
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
