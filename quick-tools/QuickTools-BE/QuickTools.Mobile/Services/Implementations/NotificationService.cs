using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace QuickTools.Mobile.Services.Implementations
{
    public class NotificationService
    {
        public async Task<bool> RequestPermissionAsync()
        {
            return await LocalNotificationCenter.Current
                .RequestNotificationPermission();
        }

        public async Task ShowAsync(
            int id,
            string title,
            string message)
        {
            var permission =
                await RequestPermissionAsync();

            if (!permission)
                return;
            
            var request = new NotificationRequest
            {
                NotificationId = id,
                Title = title,
                Description = message,
                Android = new AndroidOptions()
                {
                   IconSmallName = new AndroidIcon("notification_icon"),
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }

        public async Task ScheduleAsync(
            int id,
            string title,
            string message,
            DateTime notifyTime)
        {
            var request = new NotificationRequest
            {
                NotificationId = id,

                Title = title,

                Description = message,

                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
    }
}