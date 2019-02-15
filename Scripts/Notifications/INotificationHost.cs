using System;

namespace Scripts.Notifications
{
    public interface INotificationHost
    {
        void Schedule(RetentionNotification note, DateTime date);
        void ScheduleDebug(RetentionNotification note);
        void ClearBadges();
        void CancelAll();
    }
}