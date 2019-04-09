using System;

namespace Scripts.Notifications.Hosts
{
    public class DummyNoteHost : INotificationHost
    {
        public void Schedule(DayOfWeek day, RetentionNotification note, DateTime date)
        {
            throw new NotImplementedException();
        }

        public void ScheduleDebug(RetentionNotification note)
        {
            throw new NotImplementedException();
        }

        public void ClearBadges()
        {
            throw new NotImplementedException();
        }

        public void CancelAll()
        {
            throw new NotImplementedException();
        }

        public void Diagnostic()
        {
            throw new NotImplementedException();
        }

        public bool ConfirmScheduled()
        {
            throw new NotImplementedException();
        }
    }
}