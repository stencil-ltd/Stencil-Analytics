using System;
using Binding;
using Scripts.Prefs;
using Scripts.RemoteConfig;
using UnityEngine;
using Util;

#if UNITY_IOS
using CalendarUnit = UnityEngine.iOS.CalendarUnit;
using LocalNotification = UnityEngine.iOS.LocalNotification;
using NotificationServices = UnityEngine.iOS.NotificationServices;
#endif

#if UNITY_ANDROID && !EXCLUDE_SIMPLE_NOTIFICATIONS
using Assets.SimpleAndroidNotifications;
using Assets.SimpleAndroidNotifications.Data;
using Assets.SimpleAndroidNotifications.Enums;
using Assets.SimpleAndroidNotifications.Helpers;
using UnityEngine;
#endif

namespace Scripts.Notifications
{
    [CreateAssetMenu(menuName = "Notifications/Retention Settings")]
    public class RetentionNotifications : Singleton<RetentionNotifications>
    {
        public float timeOfDay = 8;
        public RetentionNotification[] notifications;

        [RemoteField("enable_retention_notifications")]
        private bool _remoteEnabled;

        private bool Configured
        {
            get => StencilPrefs.Default.GetBool("retention_push_configured");
            set => StencilPrefs.Default.SetBool("retention_push_configured", value).Save();
        }

        public void Init()
        {
            this.BindRemoteConfig();
            _remoteEnabled |= StencilRemote.IsDeveloper();
            StencilRemote.OnRemoteConfig += (sender, args) => Init();
            
            if (!_remoteEnabled)
            {
                CancelAll();
                return;
            }
            
            if (Configured) return;
            Configured = true;
            
            if (notifications.Length > 7)
                throw new Exception("Must specify at most 7 notifications");
                
            var next = DateTime.Today.AddDays(1).AddHours(timeOfDay);
            foreach (var note in notifications)
            {
                Schedule(note, next);
                next = next.AddDays(1);
            }
        }

        #if UNITY_IOS
        private void CancelAll()
        {
            Configured = false;
            NotificationServices.CancelAllLocalNotifications();
        }

        private void Schedule(RetentionNotification note, DateTime date)
        {
            var ln = new LocalNotification
            {
                alertTitle = note.title, 
                alertBody = note.message, 
                fireDate = date,
                repeatInterval = CalendarUnit.Week
            };
            NotificationServices.ScheduleLocalNotification(ln);
        }
        #elif UNITY_ANDROID && !EXCLUDE_SIMPLE_NOTIFICATIONS
        private void CancelAll()
        {
            NotificationManager.CancelAll();
        }

        private void Schedule(RetentionNotification note, DateTime date)
        {
            var delay = date - DateTime.Now;
            var notificationParams = new NotificationParams
            {
                Id = NotificationIdHandler.GetNotificationId(),
                Delay = delay,
                Title = note.title,
                Message = note.message,
                LargeIcon = "app_icon",
                ExecuteMode = NotificationExecuteMode.Inexact,
                Repeat = true,
                RepeatInterval = TimeSpan.FromDays(7)
            };
            NotificationManager.SendCustom(notificationParams);
        }
        #else
        private void CancelAll()
        {}
        
        private void Schedule(RetentionNotification note, DateTime date)
        {}
        #endif
    }
}