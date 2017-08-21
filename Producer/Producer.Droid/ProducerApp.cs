using System;
using Android.App;
using Android.Runtime;

namespace Producer.Droid
{
	[Application]
	public class ProducerApp : Application
	{
		/// <summary>
		/// Base constructor which must be implemented if it is to successfully inherit from the Application class.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="transfer"></param>
		public ProducerApp (IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}


		public override void OnCreate ()
		{
			base.OnCreate ();

			//JsonHttpClient.GlobalHttpMessageHandlerFactory = () => new NativeMessageHandler ();
			//JsConfig.PropertyConvention = PropertyConvention.Lenient;
		}

	}
}