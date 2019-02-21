#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using CalendarUnit = UnityEngine.iOS.CalendarUnit;
using NotificationServices = UnityEngine.iOS.NotificationServices;

namespace Scripts.Notifications.Hosts
{
    public class IosNotificationHost : INotificationHost
    {
        [DllImport ("__Internal")]
        private static extern void _clearNotificationBadge();
        
        public void Schedule(DayOfWeek day, RetentionNotification note, DateTime date)
        {
            var ln = new UnityEngine.iOS.LocalNotification
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
            var ln = new UnityEngine.iOS.LocalNotification
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

        public void Diagnostic()
        {
            var scheduled = NotificationServices.scheduledLocalNotifications;
            Debug.Log($"Retention: Found {scheduled.Length} scheduled notifications");
            foreach (var note in scheduled)
                Debug.Log($"Retention Scheduled: [{note.fireDate}, {note.repeatInterval}] - {note.alertTitle} ({note.alertBody})");
        }

        public bool ConfirmScheduled()
        {
            return NotificationServices.scheduledLocalNotifications.Length > 1;
        }
    }
}
#endif