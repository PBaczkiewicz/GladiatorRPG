using Android.App;
using Android.Content;

namespace GladiatorRPG.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = false, Label = "Reboot complete receiver")]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == "android.intent.action.BOOT_COMPLETED")
            {
                // Recreate alarms
            }
        }
    }
}

