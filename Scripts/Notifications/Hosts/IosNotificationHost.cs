//#if UNITY_IOS
using System;
using UnityEngine.iOS;

namespace Scripts.Notifications.Hosts
{
    public class IosNotificationHost : INotificationHost
    {
        public void Schedule(RetentionNotification note, DateTime date)
        {
            var ln = new LocalNotification
            {
                alertTitle = note.title, 
                alertBody = note.message, 
                fireDate = date,
                applicationIconBadgeNumber = 1,
                repeatInterval = CalendarUnit.Week
            };
            NotificationServices.ScheduleLocalNotification(ln);
        }

        public void ScheduleDebug(RetentionNotification note)
        {
            var ln = new LocalNotification
            {
                alertTitle = note.title, 
                alertBody = note.message, 
                fireDate = DateTime.Now.AddSeconds(15),
                applicationIconBadgeNumber = 1,
                repeatInterval = CalendarUnit.Minute
            };
            NotificationServices.ScheduleLocalNotification(ln);
        }

        public void ClearBadges()
        {
            #if !UNITY_EDITOR
            _clearNotificationBadge();
            #endif
        }

        public void CancelAll()
        {
            NotificationServices.CancelAllLocalNotifications();
        }
    }
}
//#endif