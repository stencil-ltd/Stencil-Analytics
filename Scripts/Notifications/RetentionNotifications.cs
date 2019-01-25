using System;
using Binding;
using Scripts.Prefs;
using Scripts.RemoteConfig;
using Scripts.Util;
using UnityEngine;
using Util;
#if UNITY_IOS
using CalendarUnit = UnityEngine.iOS.CalendarUnit;
using LocalNotification = UnityEngine.iOS.LocalNotification;
using NotificationServices = UnityEngine.iOS.NotificationServices;
#endif

#if !EXCLUDE_SIMPLE_NOTIFICATIONS
using Assets.SimpleAndroidNotifications;
using Assets.SimpleAndroidNotifications.Data;
using Assets.SimpleAndroidNotifications.Enums;
using Assets.SimpleAndroidNotifications.Helpers;
#endif

namespace Scripts.Notifications
{
    [CreateAssetMenu(menuName = "Notifications/Retention Settings")]
    public class RetentionNotifications : Singleton<RetentionNotifications>
    {
        [Header("Config")]
        public float timeOfDay = 8;

        [Header("Android")] 
        #if !EXCLUDE_SIMPLE_NOTIFICATIONS
        public NotificationIcon icon = NotificationIcon.Wrench;
        #endif
        
        [Header("Debug")] 
        public bool debugMode;
        
        [Header("Content")]
        [Tooltip("Start with Sunday, seven total")]
        public RetentionNotification[] weekOfNotifications;


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
            
            Debug.Log($"Check Retention Notifications");
            if (!Enabled)
            {
            
                Debug.Log($"Retention Notifications not enabled");
                Configured = false;
                CancelAll();
                return;
            }

            if (Configured && !debugMode)
            {
                Debug.Log($"Retention Notifications already configured");
                return;
            }
            Configured = true;
            
            if (weekOfNotifications.Length > 7)
                throw new Exception("Must specify at most 7 notifications");
            
            Debug.Log($"Configuring Retention Notifications");
            CancelAll();

            var i = 0;
            var day = DayOfWeek.Sunday;
            foreach (var note in weekOfNotifications)
            {
                var next = day.GetNext().AddHours(timeOfDay);
                Debug.Log($"Schedule note {i++} for {next}");
                Schedule(note, next); 
                day++;
            }

            if (debugMode)
            {
                Debug.Log($"Schedule debug note");
                ScheduleDebug();
            }
        }

        public void Clear()
        {
            #if UNITY_IOS
            var setCountNotif = new LocalNotification
            {
                applicationIconBadgeNumber = -1, 
                hasAction = false
            };
            NotificationServices.PresentLocalNotificationNow(setCountNotif);
            #endif
        }

        #if UNITY_IOS
        private void CancelAll()
        {
            NotificationServices.CancelAllLocalNotifications();
        }

        private void ScheduleDebug()
        {
            var note = weekOfNotifications[0];
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
        
        private void Schedule(RetentionNotification note, DateTime date)
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
                SmallIcon = icon,
                LargeIcon = note.icon,
                ExecuteMode = NotificationExecuteMode.Inexact,
                Multiline = true,
                Repeat = true,
                RepeatInterval = TimeSpan.FromDays(7)
            };
            NotificationManager.SendCustom(notificationParams);
        }
        
        private void ScheduleDebug()
        {
            var note = weekOfNotifications[0];
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