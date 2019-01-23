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

        [Header("Debug")] 
        public bool debugMode;

        [RemoteField("enable_retention_notifications")]
        private bool _remoteEnabled;

        private bool Configured
        {
            get => StencilPrefs.Default.GetBool("retention_push_configured");
            set => StencilPrefs.Default.SetBool("retention_push_configured", value).Save();
        }

        private bool Enabled => debugMode || _remoteEnabled || StencilRemote.IsDeveloper();

        public void Init()
        {
            this.BindRemoteConfig();
            _remoteEnabled |= StencilRemote.IsDeveloper();
            StencilRemote.OnRemoteConfig += (sender, args) => Init();
            
            if (!Enabled)
            {
                CancelAll();
                return;
            }
            
            if (Configured && !debugMode) return;
            Configured = true;
            
            if (notifications.Length > 7)
                throw new Exception("Must specify at most 7 notifications");
            
            CancelAll();

            var i = 0;
            var next = DateTime.Today.AddDays(1).AddHours(timeOfDay);
            foreach (var note in notifications)
            {
                Debug.Log($"Schedule note {i++} for {next}");
                Schedule(note, next);
                next = next.AddDays(1);
            }

            if (debugMode)
            {
                Debug.Log($"Schedule debug note");
                ScheduleDebug();
            }
        }

        #if UNITY_IOS
        private void CancelAll()
        {
            Configured = false;
            NotificationServices.CancelAllLocalNotifications();
        }

        private void ScheduleDebug()
        {
            var note = notifications[0];
            var ln = new LocalNotification
            {
                alertTitle = note.title, 
                alertBody = note.message, 
                fireDate = DateTime.Now.AddSeconds(30),
                repeatInterval = CalendarUnit.Minute
            };
            NotificationServices.ScheduleLocalNotification(ln);
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
                SmallIcon = NotificationIcon.Wrench,
                ExecuteMode = NotificationExecuteMode.Inexact,
                Multiline = true,
                Repeat = true,
                RepeatInterval = TimeSpan.FromDays(7)
            };
            NotificationManager.SendCustom(notificationParams);
        }
        
        private void ScheduleDebug()
        {
            var note = notifications[0];
            var notificationParams = new NotificationParams
            {
                Id = NotificationIdHandler.GetNotificationId(),
                Delay = TimeSpan.FromSeconds(30),
                Title = note.title,
                Message = note.message,
                SmallIcon = NotificationIcon.Wrench,
                Multiline = true,
                ExecuteMode = NotificationExecuteMode.ExactAndAllowWhileIdle,
                Repeat = true,
                RepeatInterval = TimeSpan.FromMinutes(1)
            };
            NotificationManager.SendCustom(notificationParams);
        }
        #else
        private void CancelAll()
        {}
        
        private void Schedule(RetentionNotification note, DateTime date)
        {}
        
        private void ScheduleDebug()
        {}
        #endif
    }
}