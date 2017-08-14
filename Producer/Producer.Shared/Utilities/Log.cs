#if DEBUG
using System;
using System.Runtime.CompilerServices;
using System.Linq;
#endif

namespace Producer
{
	public static class Log
	{
#if DEBUG

		public static void Debug (object caller, string methodName, string message)
		{
			System.Diagnostics.Debug.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] [{caller.GetType ().Name}] {methodName} : {message}");
		}


		public static void Debug (string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			sourceFilePath = sourceFilePath.Split ('/').LastOrDefault ();

			System.Diagnostics.Debug.WriteLine ($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] [{sourceFilePath}] [{memberName}] [{sourceLineNumber}] : {message}");
		}

#else
		public static void Debug (string message, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0) { }
#endif
	}
}
