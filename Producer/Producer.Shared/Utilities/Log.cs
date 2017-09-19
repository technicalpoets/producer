using System;
using System.Linq;

#if DEBUG
using System.Runtime.CompilerServices;
#endif

namespace Producer
{
	public static class Log
	{
#if DEBUG

		public static void Debug (object caller, string methodName, string message)
		{
			System.Diagnostics.Debug.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] DEBUG: [{caller.GetType ().Name}] {methodName} : {message}");
		}


		public static void Debug (string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			sourceFilePath = sourceFilePath.Split ('/').LastOrDefault ();

			System.Diagnostics.Debug.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] DEBUG: [{sourceFilePath}] [{memberName}] [{sourceLineNumber}] : {message}");
		}

#else
		public static void Debug (string message, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
		{
			Console.WriteLine ($"DEBUG: {message}");
		}
#endif

#if DEBUG
		public static void Info (string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			sourceFilePath = sourceFilePath.Split ('/').LastOrDefault ();

			System.Diagnostics.Debug.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}]  Info: [{sourceFilePath}] [{memberName}] [{sourceLineNumber}] : {message}");
			//System.Diagnostics.Trace.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] [{sourceFilePath}] [{memberName}] [{sourceLineNumber}] : {message}");
		}
#else
		public static void Info (string message, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
		{
			Console.WriteLine ($"Info: {message}");
		}
#endif

#if DEBUG
		public static void Error (string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			sourceFilePath = sourceFilePath.Split ('/').LastOrDefault ();

			System.Diagnostics.Debug.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] ERROR: [{sourceFilePath}] [{memberName}] [{sourceLineNumber}] : {message}");
			//System.Diagnostics.Trace.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] [{sourceFilePath}] [{memberName}] [{sourceLineNumber}] : {message}");
		}
#else
		public static void Error (string message, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
		{
			memberName = string.IsNullOrEmpty (memberName) ? string.Empty : $"[{memberName}] ";
			sourceFilePath = string.IsNullOrEmpty (sourceFilePath) ? string.Empty : $"[{sourceFilePath}] ";
			var sourceLineNumberString = sourceLineNumber == 0 ? string.Empty : $"[{sourceLineNumber}] : ";

			Console.WriteLine ($"ERROR: {sourceFilePath}{memberName}{sourceLineNumberString}{message}");
		}
#endif

#if DEBUG
		public static void Error (Exception error, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			Error (error.Message, memberName, sourceFilePath, sourceLineNumber);
		}
#else
		public static void Error (Exception error, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
		{
			var message = $"{error.Message}{Environment.NewLine}{error.StackTrace}{Environment.NewLine}{error.InnerException?.StackTrace}";
			Error (message, memberName, sourceFilePath, sourceLineNumber);
		}
#endif
	}
}
