using System;
using Binding;
using Scripts.Notifications.Hosts;
using Scripts.Prefs;
using Scripts.RemoteConfig;
using Scripts.Util;
using UnityEngine;
using Util;

#if ANDROID_SIMPLE_NOTIFICATIONS
using Assets.SimpleAndroidNotifications.Enums;
#endif

namespace Scripts.Notifications
{
    [CreateAssetMenu(menuName = "Notifications/Retention Settings")]
    public class RetentionNotifications : Singleton<RetentionNotifications>, INotificationHost
    {
        [Header("Config")]
        public float timeOfDay = 8;

        [Header("Android")] 
        #if ANDROID_SIMPLE_NOTIFICATIONS
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
            get => StencilPrefs.Default.GetBool("retention_push_configured_v2");
            set => StencilPrefs.Default.SetBool("retention_push_configured_v2", value).Save();
        }

        private bool Enabled => debugMode || _remoteEnabled || StencilRemote.IsDeveloper();
        private bool _init;
        
        public void Init()
        {
            if (_init) return;
            _init = true;
            
            this.BindRemoteConfig();
            _remoteEnabled |= StencilRemote.IsDeveloper();
            StencilRemote.OnRemoteConfig += (sender, args) => Init();
            _host = _host ?? ConfigureHost();
            
            ClearBadges();
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
                Debug.Log($"Retention: Schedule note {i++} for {next}");
                Schedule(day, note, next); 
                day++;
            }

            Debug.Log("Retention: Check debug mode");
            if (debugMode)
            {
                Debug.Log($"Retention: Schedule debug note");
                ScheduleDebug(weekOfNotifications[0]);
            }
        }

        private INotificationHost ConfigureHost()
        {
            #if UNITY_IOS
               return new IosNotificationHost();
            #elif UNITY_ANDROID 
                #if ANDROID_UNITY_NOTIFICATIONS
                    return new AndroidUnityNotificationHost();
                #elif ANDROID_SIMPLE_NOTIFICATIONS
                    return new AndroidSimpleNotificationHost(icon);
                #endif
            #endif
            return null;
        }

        public void Schedule(DayOfWeek day, RetentionNotification note, DateTime date)
        {
            _host?.Schedule(day, note, date);
        }

        public void ScheduleDebug(RetentionNotification note)
        {
            _host?.ScheduleDebug(note);
        }

        public void CancelAll()
        {
            _host?.CancelAll();
        }

        public void ClearBadges()
        {
            _host?.ClearBadges();
        }
    }
}