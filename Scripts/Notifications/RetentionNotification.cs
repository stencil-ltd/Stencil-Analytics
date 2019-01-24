using System;
using JetBrains.Annotations;

namespace Scripts.Notifications
{
    [Serializable]
    public class RetentionNotification
    {
        [CanBeNull] 
        public string icon;
        public string title;
        public string message;
    }
}