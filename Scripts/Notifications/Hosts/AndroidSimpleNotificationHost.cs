#if UNITY_ANDROID && ANDROID_SIMPLE_NOTIFICATIONS

using System;
using Assets.SimpleAndroidNotifications;
using Assets.SimpleAndroidNotifications.Data;
using Assets.SimpleAndroidNotifications.Enums;
using Assets.SimpleAndroidNotifications.Helpers;

namespace Scripts.Notifications.Hosts
{
    public class AndroidSimpleNotificationHost : INotificationHost
    {
        public readonly NotificationIcon icon;

        public AndroidSimpleNotificationHost(NotificationIcon icon)
        {
            this.icon = icon;
        }

        public void Schedule(RetentionNotification note, DateTime date)
        {
            var delay = date - DateTime.Now;
            var notificationParams = new NotificationParams
            {
                Id = NotificationIdHandler.GetNotificationId(),
                Delay = delay,
                Title = note.title,
                Message = note.message,
                SmallIcon = icon,
                LargeIcon = note.icon,
                ExecuteMode = NotificationExecuteMode.Inexact,
                Multiline = true,
                Repeat = true,
                RepeatInterval = TimeSpan.FromDays(7)
            };
            NotificationManager.SendCustom(notificationParams);
        }

        public void ScheduleDebug(RetentionNotification note)
        {
            var notificationParams = new NotificationParams
            {
                Id = NotificationIdHandler.GetNotificationId(),
                Delay = TimeSpan.FromSeconds(30),
                Title = note.title,
                Message = note.message,
                SmallIcon = icon,
                LargeIcon = note.icon,
                Multiline = true,
                ExecuteMode = NotificationExecuteMode.ExactAndAllowWhileIdle,
                Repeat = true,
                RepeatInterval = TimeSpan.FromMinutes(1)
            };
            NotificationManager.SendCustom(notificationParams);
        }

        public void ClearBadges()
        {
            
        }

        public void CancelAll()
        {
            NotificationManager.CancelAll();
        }
    }
}

#endif