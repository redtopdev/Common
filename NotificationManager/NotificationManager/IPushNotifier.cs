namespace RedTop.Common.NotificationManager
{
    using System.Collections.Generic;
    public interface IPushNotifier
    {
        void PushNotification(ICollection<string> registrationIds, dynamic notificationData);
    }
}
