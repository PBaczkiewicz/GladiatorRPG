using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Gms.Ads;
using Xamarin.Essentials;
using Android.Content;
using AndroidX.Core.App;
using System;
using Xamarin.Forms;
using GladiatorRPG;
using GladiatorRPG.Droid;
using Android.Media;
using Xamarin.Forms.Platform.Android;
using AndroidX.AppCompat.App;
using Android.Util;
using Android.Content.Res;

[assembly: Dependency(typeof(MainActivity))]
namespace GladiatorRPG.Droid
{

    [Activity(Label = "Gladiator RPG", Icon = "@mipmap/game_icon", Theme = "@style/MainTheme", MainLauncher = true,  ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Context baseContext;
        public static NotificationManager notificationManager;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //initFontScale();
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
            try
            {
                OrientationSensor.Stop();
            }
            catch
            {

            }
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CallPermissions();
            MobileAds.Initialize(ApplicationContext);
            LoadApplication(new App());

        }

        #region FONT SCALING (dont you dare touching this)
        protected override void AttachBaseContext(Context @base)
        {
            base.AttachBaseContext(OverrideFontScale(@base));
        }
        private Context OverrideFontScale(Context context)
        {
            Configuration configuration = new Configuration(context.Resources.Configuration);
            configuration.FontScale = 1.0f; // Set font scale to 1.0
            return context.CreateConfigurationContext(configuration);
        }
        #endregion

        async void CallPermissions()
        {
            //await Permissions.RequestAsync<Permissions.StorageWrite>();  // probably not neccesary
            //await Permissions.RequestAsync<Permissions.StorageRead>();  // probably not neccesary
            await Permissions.RequestAsync<Permissions.Reminders>();



        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var intent = new Intent(this, this.GetType()).AddFlags(ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.Mutable);
        }

    }

}