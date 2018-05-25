
namespace RedTop.Common.NotificationManager.GCM
{
    using System;
    using System.Collections.Generic;

    using PushSharp.Google;
    using PushSharp.Common;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using System.Linq;
    public class GCMNotifier : IPushNotifier
    {

        private string _gcmSenderId;
        private string _authToken;

        public GCMNotifier(string gcmSenderId, string authToken)
        {
            this._gcmSenderId = gcmSenderId;
            this._authToken = authToken;
        }

        #region private methods
        public void PushNotification(ICollection<string> registrationIds, dynamic notificationData)
        {
            var config = new GcmConfiguration(this._gcmSenderId, this._authToken, null);

            //Create our push services broker
            var gcmBroker = new GcmServiceBroker(config);

            gcmBroker.OnNotificationSucceeded += GcmBroker_OnNotificationSucceeded;

            gcmBroker.OnNotificationFailed += GcmBroker_OnNotificationFailed;

            // Start the broker
            gcmBroker.Start();

            this.QueueNotofication(registrationIds, notificationData, gcmBroker);

            gcmBroker.Stop();

        }

        private void GcmBroker_OnNotificationSucceeded(GcmNotification notification)
        {
            //
        }

        private void GcmBroker_OnNotificationFailed(GcmNotification notification, AggregateException aggregateEx)
        {
            aggregateEx.Handle(ex =>
            {

                // See what kind of exception it was to further diagnose
                if (ex is GcmNotificationException)
                {
                    var notificationException = (GcmNotificationException)ex;

                    // Deal with the failed notification
                    var gcmNotification = notificationException.Notification;
                    var description = notificationException.Description;

                    //Log.info($"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}");
                }
                else if (ex is GcmMulticastResultException)
                {
                    var multicastException = (GcmMulticastResultException)ex;

                    foreach (var succeededNotification in multicastException.Succeeded)
                    {
                        Console.WriteLine($"GCM Notification Succeeded: ID={succeededNotification.MessageId}");
                    }

                    foreach (var failedKvp in multicastException.Failed)
                    {
                        var n = failedKvp.Key;
                        var e = failedKvp.Value;

                        //Log.info($"GCM Notification Failed: ID={n.MessageId}, Desc={e.Data}");
                    }

                }
                else if (ex is DeviceSubscriptionExpiredException)
                {
                    var expiredException = (DeviceSubscriptionExpiredException)ex;

                    var oldId = expiredException.OldSubscriptionId;
                    var newId = expiredException.NewSubscriptionId;

                    //Log.info($"Device RegistrationId Expired: {oldId}");

                    if (!string.IsNullOrWhiteSpace(newId))
                    {
                        // If this value isn't null, our subscription changed and we should update our database
                        //Log.info($"Device RegistrationId Changed To: {newId}");
                    }
                }
                else if (ex is RetryAfterException)
                {
                    var retryException = (RetryAfterException)ex;
                    // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                    //Log.info($"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}");
                }
                else
                {
                    //Log.info("GCM Notification Failed for some unknown reason");
                }

                // Mark it as handled
                return true;
            });
        }

        private void QueueNotofication(ICollection<string> registrationIds, dynamic notificationData, GcmServiceBroker gcmBroker)
        {
            // Queue a notification to send
            gcmBroker.QueueNotification(new GcmNotification
            {
                RegistrationIds = registrationIds.ToList(),
                Data = JObject.Parse(JsonConvert.SerializeObject(notificationData))
            });
        }

        #endregion private methods
    }
}
