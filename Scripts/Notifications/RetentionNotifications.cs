using System;
using Binding;
using Scripts.Notifications.Hosts;
using Scripts.Prefs;
using Scripts.RemoteConfig;
using Scripts.Util;
using UnityEngine;
using Util;
#if UNITY_IOS
using System.Runtime.InteropServices;
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
        #if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void _clearNotificationBadge();
        #endif
        
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

        private INotificationHost _host;

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
            if (_host == null)
            {
                #if UNITY_IOS
                _host = new IosNotificationHost();
                #elif UNITY_ANDROID
                _host = new AndroidSimpleNotificationHost(icon);
                #endif
            }
            
            _host.ClearBadges();
            Debug.Log($"Check Retention Notifications");
            if (!Enabled)
            {
            
                Debug.Log($"Retention Notifications not enabled");
                Configured = false;
                _host.CancelAll();
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
            _host.CancelAll();

            var i = 0;
            var day = DayOfWeek.Sunday;
            foreach (var note in weekOfNotifications)
            {
                var next = day.GetNext().AddHours(timeOfDay);
                Debug.Log($"Schedule note {i++} for {next}");
                _host.Schedule(note, next); 
                day++;
            }

            if (debugMode)
            {
                Debug.Log($"Schedule debug note");
                _host.ScheduleDebug(weekOfNotifications[0]);
            }
        }

        public void ClearBadges()
        {
            _host.ClearBadges();
        }
    }
}