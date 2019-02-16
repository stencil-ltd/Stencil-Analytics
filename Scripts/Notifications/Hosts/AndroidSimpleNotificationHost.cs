#if UNITY_ANDROID && ANDROID_SIMPLE_NOTIFICATIONS

using System;
using Assets.SimpleAndroidNotifications;
using Assets.SimpleAndroidNotifications.Data;
using Assets.SimpleAndroidNotifications.Enums;
using Assets.SimpleAndroidNotifications.Helpers;
using UnityEditor;

namespace Scripts.Notifications.Hosts
{
    public class AndroidSimpleNotificationHost : INotificationHost
    {
        public readonly NotificationIcon icon;

        private static int GetId(int day) => 
            $"retention_day_{day}".GetHashCode();

        public AndroidSimpleNotificationHost(NotificationIcon icon)
        {
            this.icon = icon;
        }

        public void Schedule(DayOfWeek day, RetentionNotification note, DateTime date)
        {
            var delay = date - DateTime.Now;
            var notificationParams = new NotificationParams
            {
                Id = GetId((int) day),
                Delay = delay,
                ExecuteMode = NotificationExecuteMode.Inexact,
                RepeatInterval = TimeSpan.FromDays(7)
            };
            Process(notificationParams, note);
            NotificationManager.SendCustom(notificationParams);
        }

        public void ScheduleDebug(RetentionNotification note)
        {
            var notificationParams = new NotificationParams
            {
                Id = GetId(100),
                Delay = TimeSpan.FromSeconds(10),
                ExecuteMode = NotificationExecuteMode.ExactAndAllowWhileIdle,
                RepeatInterval = TimeSpan.FromSeconds(30)
            };
            Process(notificationParams, note);
            NotificationManager.SendCustom(notificationParams);
        }

        private void Process(NotificationParams p, RetentionNotification note)
        {
            p.Title = note.title;
            p.Message = note.message;
            p.SmallIcon = icon;
            p.LargeIcon = note.icon;
            p.Vibrate = false;
            p.Vibration = new int[]{};
            p.Multiline = true;
            p.Repeat = true;
        }

        public void ClearBadges()
        {
            
        }

        public void CancelAll()
        {
            NotificationManager.CancelAll();
            NotificationManager.Cancel(GetId(100)); // debug notes.
            for (var i = 0; i < 7; i++) 
                NotificationManager.Cancel(GetId(i));
        }
    }
}

#endif