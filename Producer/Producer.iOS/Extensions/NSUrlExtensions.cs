using System;

using Foundation;
using MobileCoreServices;

namespace Producer.iOS
{
	public static class NSUrlExtensions
	{
		public static (string UTType, string UTSubtype, string Filename) GetAvUtiConformance (this NSUrl filePath)
		{
			NSError error;

			NSObject typeValue;

			if (filePath.TryGetResource (NSUrl.TypeIdentifierKey, out typeValue, out error))
			{
				var typeValueString = typeValue as NSString;

				if (typeValueString != null)
				{
					if (UTType.ConformsTo (typeValueString, UTType.Movie))
					{
						if (UTType.ConformsTo (typeValueString, UTType.MPEG4))
						{
							return (UTType.Movie, UTType.MPEG4, filePath.LastPathComponent);
						}

						if (UTType.ConformsTo (typeValueString, UTType.QuickTimeMovie))
						{
							return (UTType.Movie, UTType.QuickTimeMovie, filePath.LastPathComponent);
						}

						// alert that file is recognized as a movie, but we don't handle it (yet) log it
						return (UTType.Movie, UTType.Movie, filePath.LastPathComponent);
					}

					if (UTType.ConformsTo (typeValueString, UTType.Audio))
					{
						if (UTType.ConformsTo (typeValueString, UTType.MP3))
						{
							return (UTType.Audio, UTType.MP3, filePath.LastPathComponent);
						}

						if (UTType.ConformsTo (typeValueString, UTType.WaveformAudio))
						{
							return (UTType.Audio, UTType.WaveformAudio, filePath.LastPathComponent);
						}

						// alert that file is recognized as a audio, but we don't handle it (yet) log it
						return (UTType.Audio, UTType.Audio, filePath.LastPathComponent);
					}
				}
			}
			else if (error != null)
			{
				//Log.Debug ($"Error trying to get resource identifier\n\t{error.Code}\n\t{error.Domain}\n\t{error.Description}");
				return (UTType.PlainText, $"Error trying to get resource identifier\n\t{error.Code}\n\t{error.Domain}\n\t{error.Description}", filePath.LastPathComponent);
			}

			return (UTType.PlainText, $"Unsupported file type or path", filePath.LastPathComponent);
		}


		public static void PrintAttributes (this NSUrl filePath)
		{
			NSError error;

			var fileAttributes = NSFileManager.DefaultManager.GetAttributes (filePath.Path, out error);

			if (error != null)
			{
				Log.Debug ($"{error}");
				Log.Debug ($"Error trying to get resource attributes\n\t{error.Code}\n\t{error.Domain}\n\t{error.Description}");
			}
			else
			{
				Log.Debug ($"fileAttributes.AppendOnly = {fileAttributes.AppendOnly}");
				Log.Debug ($"fileAttributes.Busy = {fileAttributes.Busy}");
				Log.Debug ($"fileAttributes.CreationDate = {fileAttributes.CreationDate}");
				Log.Debug ($"fileAttributes.DeviceIdentifier = {fileAttributes.DeviceIdentifier}");
				Log.Debug ($"fileAttributes.ExtensionHidden = {fileAttributes.ExtensionHidden}");
				Log.Debug ($"fileAttributes.GroupOwnerAccountID = {fileAttributes.GroupOwnerAccountID}");
				Log.Debug ($"fileAttributes.GroupOwnerAccountName = {fileAttributes.GroupOwnerAccountName}");
				Log.Debug ($"fileAttributes.HfsCreatorCode = {fileAttributes.HfsCreatorCode}");
				Log.Debug ($"fileAttributes.HfsTypeCode = {fileAttributes.HfsTypeCode}");
				Log.Debug ($"fileAttributes.Immutable = {fileAttributes.Immutable}");
				Log.Debug ($"fileAttributes.ModificationDate = {fileAttributes.ModificationDate}");
				Log.Debug ($"fileAttributes.OwnerAccountID = {fileAttributes.OwnerAccountID}");
				Log.Debug ($"fileAttributes.OwnerAccountName = {fileAttributes.OwnerAccountName}");
				Log.Debug ($"fileAttributes.PosixPermissions = {fileAttributes.PosixPermissions}");
				Log.Debug ($"fileAttributes.ProtectionKey = {fileAttributes.ProtectionKey}");
				Log.Debug ($"fileAttributes.ReferenceCount = {fileAttributes.ReferenceCount}");
				Log.Debug ($"fileAttributes.Size = {fileAttributes.Size}");
				Log.Debug ($"fileAttributes.SystemFileNumber = {fileAttributes.SystemFileNumber}");
				Log.Debug ($"fileAttributes.SystemNumber = {fileAttributes.SystemNumber}");
				Log.Debug ($"fileAttributes.Type = {fileAttributes.Type}");
			}
		}
	}
}
