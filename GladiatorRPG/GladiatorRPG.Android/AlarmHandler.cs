using Android.Content;
using Java.Interop;

namespace GladiatorRPG.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = false ,Label = "Local Notifications Broadcast Receiver")]
    public class AlarmHandler : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(AndroidNotificationManager.TitleKey);
                string message = intent.GetStringExtra(AndroidNotificationManager.MessageKey);
                int id = int.Parse(intent.GetStringExtra(AndroidNotificationManager.ChannelId));
                AndroidNotificationManager manager = AndroidNotificationManager.Instance ?? new AndroidNotificationManager();
                manager.Show(title, message, id);
            }
        }
    }
}
