namespace RedTop.Common.NotificationManager
{
    using System.Threading;
    public class PushNotificationTaskQueue : TaskQueue
    {
        IPushNotifier _notifier;
        public PushNotificationTaskQueue(IPushNotifier notifier)           
        {
            this._notifier = notifier;
        }
        protected override void PerformTask(dynamic userData)
        {
            Thread.Sleep(1000);
            this._notifier.PushNotification(userData.RegistrationIds, userData.NotificationData);
        }    
    }
}
