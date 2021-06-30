using System.Threading.Tasks;
using Foundation;
using NControl.iOS;
using Prism.Events;
using Lottie.Forms.Platforms.Ios;
using Plugin.Permissions;
using Prism.Ioc;
using Xamarin.Forms;
using UIKit;
using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using JimmyDore.Services.DialogAlert;
using JimmyDore.Enums;
using UserNotifications;

namespace JimmyDore.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        App _app;

        IEventAggregator _eventAggregator;

        public override bool FinishedLaunching(UIApplication application, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            Plugin.InputKit.Platforms.iOS.Config.Init();

            NControlViewRenderer.Init();

            Plugin.Segmented.Control.iOS.SegmentedControlRenderer.Initialize();

            Rg.Plugins.Popup.Popup.Init();

            LoadApplication(_app = new App(new iOSInitializer()));

            if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.DEVICE)
            {
                Device.StartTimer(TimeSpan.FromSeconds(10), () =>
                {
                    var settings = UIApplication.SharedApplication.CurrentUserNotificationSettings.Types;
                    if (settings == UIUserNotificationType.None)
                    {
                        var lastSetting = Xamarin.Essentials.Preferences.Get(PreferenceKeys.NotificationWarning, false);

                        if (!lastSetting)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                var result = await _app.Container.Resolve<IJimmyDoreDialogService>().UserAcceptedDisplayAlert(
                                    "Notifications", "Your Notifications are off. If you want messages from Jimmy go to 'Device Settings' and turn on Notifications for this app.", "Stop", "OK");

                                Xamarin.Essentials.Preferences.Set(PreferenceKeys.NotificationWarning, result);
                            });
                        }
                    }
                    return false;
                });

                FirebasePushNotificationManager.Initialize(options, new NotificationUserCategory[]
                {
                    new NotificationUserCategory("message",new List<NotificationUserAction> {
                        new NotificationUserAction("Reply","Reply",NotificationActionType.Foreground)
                    }),
                    new NotificationUserCategory("request",new List<NotificationUserAction> {
                        new NotificationUserAction("Accept","Accept"),
                        new NotificationUserAction("Reject","Reject",NotificationActionType.Destructive)
                    })
                });

                FirebasePushNotificationManager.CurrentNotificationPresentationOption =
                     UserNotifications.UNNotificationPresentationOptions.Alert | UserNotifications.UNNotificationPresentationOptions.Sound;
            }

            _eventAggregator = _app.Container.Resolve<IEventAggregator>();

            return base.FinishedLaunching(application, options);
        }

        [Export("applicationWillEnterForeground:")]
        public override void WillEnterForeground(UIApplication application)
        {
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);

        }

        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);

            // Do your magic to handle the notification data
            //System.Console.WriteLine(userInfo);

            completionHandler(UIBackgroundFetchResult.NewData);
        }

        // Receive displayed notifications for iOS 10 devices.
        // Handle incoming notification messages while app is in the foreground.
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            var userInfo = notification.Request.Content.UserInfo;

            // With swizzling disabled you must let Messaging know about the message, for Analytics
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);

            // Print full message.
            Console.WriteLine(userInfo);

            // Change this to your preferred presentation option
            completionHandler(UNNotificationPresentationOptions.None);
        }

        // Handle notification messages after display notification is tapped by the user.
        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var userInfo = response.Notification.Request.Content.UserInfo;

            // Print full message.
            Console.WriteLine(userInfo);

            completionHandler();
        }
    }
}
