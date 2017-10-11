using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;

namespace Producer.Droid
{
	public static class NotificationExtensions
	{
		public static void InitNotificationChannels (this Context context)
		{
			if (Build.VERSION.SdkInt < BuildVersionCodes.O)
			{
				return;
			}

			//var notificationManager = NotificationManagerCompat.From (Context);

			var notificationManager = (NotificationManager) context.GetSystemService (Context.NotificationService);
			// The id of the channel.
			String id = NotificationChannels.ContentPublished.Id;
			// The user-visible name of the channel.
			var name = NotificationChannels.ContentPublished.Name;
			// The user-visible description of the channel.
			var description = NotificationChannels.ContentPublished.Description;

			var channel = new NotificationChannel (id, name, NotificationImportance.High);
			// Configure the notification channel.
			channel.Description = description;
			channel.EnableLights (true);
			// Sets the notification light color for notifications posted to this
			// channel, if the device supports this feature.
			channel.LightColor = Color.Red;
			channel.EnableVibration (true);
			channel.SetVibrationPattern (new long [] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });

			notificationManager.CreateNotificationChannel (channel);
		}


		public static Notification.Builder GetChannelNotification (this Context context, string channel, string title, string body)
		{
			//NotificationCompat.Builder 2 param constructor not available yet :(

			return new Notification.Builder (context, channel)
					.SetContentTitle (title)
					.SetContentText (body)
					.SetSmallIcon (Android.Resource.Drawable.StatNotifyMore)
					.SetAutoCancel (true);
		}


		public static NotificationCompat.Builder GetNotification (this Context context, string title, string body)
		{
			//NotificationCompat.Builder 2 param constructor not available yet :(

			return new NotificationCompat.Builder (context)
					.SetContentTitle (title)
					.SetContentText (body)
					.SetSmallIcon (Android.Resource.Drawable.StatNotifyMore)
					.SetAutoCancel (true);
		}


		public static Notification.Builder SetLaunchActivity (this Notification.Builder builder, Type activityType)
		{
			var intent = new Intent (Application.Context, activityType);
			//intent.putExtra (...); // add some extras here
			var pendingIntent = PendingIntent.GetActivity (Application.Context, builder.GetHashCode (), intent, PendingIntentFlags.UpdateCurrent);
			builder.SetContentIntent (pendingIntent);

			return builder;
		}


		public static NotificationCompat.Builder SetLaunchActivity (this NotificationCompat.Builder builder, Type activityType)
		{
			var intent = new Intent (Application.Context, activityType);
			//intent.putExtra (...); // add some extras here
			var pendingIntent = PendingIntent.GetActivity (Application.Context, builder.GetHashCode (), intent, PendingIntentFlags.UpdateCurrent);
			builder.SetContentIntent (pendingIntent);

			return builder;
		}


		public static void Send (this Notification.Builder builder)
		{
			var notification = builder.Build ();
			//var notificationId = Convert.ToInt32 (DateTime.Now.Ticks); //meh?  how unique does this need to be?

			//we want to use the same thing here we passed as request id, above
			var notificationId = builder.GetHashCode ();

			NotificationManagerCompat notificationManager = NotificationManagerCompat.From (Application.Context);
			notificationManager.Notify (notificationId, notification);
		}


		public static void Send (this NotificationCompat.Builder builder)
		{
			var notification = builder.Build ();
			//var notificationId = Convert.ToInt32 (DateTime.Now.Ticks); //meh?  how unique does this need to be?

			//we want to use the same thing here we passed as request id, above
			var notificationId = builder.GetHashCode ();

			NotificationManagerCompat notificationManager = NotificationManagerCompat.From (Application.Context);
			notificationManager.Notify (notificationId, notification);
		}
	}
}