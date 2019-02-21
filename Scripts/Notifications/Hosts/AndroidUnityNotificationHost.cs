#if UNITY_ANDROID && ANDROID_UNITY_NOTIFICATIONS

using System;
using Unity.Notifications.Android;

namespace Scripts.Notifications.Hosts
{
    public class AndroidUnityNotificationHost : INotificationHost
    {
        private static string channelId = "merge_city";

        private AndroidNotificationChannel _channel;
        
        public AndroidUnityNotificationHost()
        {
            _channel = new AndroidNotificationChannel(
                channelId, 
                "Merge City Notifications", 
                "Motor Empire",
                Importance.Default);
            AndroidNotificationCenter.RegisterNotificationChannel(_channel);
        }

        public void Schedule(DayOfWeek day, RetentionNotification note, DateTime date)
        {
            var n = new AndroidNotification
            {
                FireTime = date, 
                RepeatInterval = TimeSpan.FromDays(7)
            };
            Process(n, note);
            AndroidNotificationCenter.SendNotification(n, channelId);
        }

        public void ScheduleDebug(RetentionNotification note)
        {
            var n = new AndroidNotification
            {
                FireTime = DateTime.Now.AddSeconds(10), 
                RepeatInterval = TimeSpan.FromMinutes(1)
            };
            Process(n, note);
            AndroidNotificationCenter.SendNotification(n, channelId);
        }

        private void Process(AndroidNotification n1, RetentionNotification n2)
        {
            n1.Title = n2.title;
            n1.Text = n2.message;
            n1.SmallIcon = "ic_wrench";
            n1.LargeIcon = n2.icon;
            n1.Style = NotificationStyle.BigTextStyle;
        }

        public void ClearBadges()
        {
            
        }

        public void CancelAll()
        {
            AndroidNotificationCenter.CancelAllNotifications();
        }

        public void Diagnostic()
        {
            
        }
    }
}

#endif