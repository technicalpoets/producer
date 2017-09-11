#if __MOBILE__
using System;
using Newtonsoft.Json;
#endif

namespace Producer.Domain
{
	public class AvContent : Content
	{
		public double Duration { get; set; }

		public AvContentTypes ContentType { get; set; }

#if __MOBILE__

		[JsonIgnore]
		public string DurationString => $"{Math.Floor (Duration / 60)}:{(Duration % 60).ToString ("00")}";

		//#if __IOS__

		[JsonIgnore]
		public string LocalInboxPath
		{
			get => Settings.StringForKey ($"inbox-{Id}");
			set => Settings.SetSetting ($"inbox-{Id}", value ?? string.Empty);
		}

		[JsonIgnore]
		public bool HasLocalInboxPath => !string.IsNullOrEmpty (LocalInboxPath);

		//#endif

#endif
	}
}
