using System;

namespace Scripts.Notifications
{
    public interface INotificationHost
    {
        void Schedule(DayOfWeek day, RetentionNotification note, DateTime date);
        void ScheduleDebug(RetentionNotification note);
        void ClearBadges();
        void CancelAll();
        void Diagnostic();
    }
}