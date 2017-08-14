using System;

using AVFoundation;
using CoreFoundation;
using CoreMedia;
using Foundation;
using MediaPlayer;

namespace Producer.iOS
{
	public class AssetPlaybackManager : NSObject
	{
		static AssetPlaybackManager _shared;
		public static AssetPlaybackManager Shared => _shared ?? (_shared = new AssetPlaybackManager ());


		readonly static NSString AVPlayerItem_Status = new NSString ("status");
		readonly static NSString AVUrlAsset_Playable = new NSString ("playable");
		readonly static NSString AVPlayer_CurrentItem = new NSString ("currentItem");


		//public event EventHandler<AVPlayer> ReadyToPlay;
		public event EventHandler<AVPlayer> CurrentItemChanged;


		public bool IsPlaying => Player?.Rate == 1;

		NSObject timeObservationToken;


		IntPtr observerContext = IntPtr.Zero;


		bool readyForPlayback;


		readonly AVPlayer Player = new AVPlayer ();


		AVPlayerItem _playerItem;
		AVPlayerItem PlayerItem {
			get { return _playerItem; }
			set {
				_playerItem?.RemoveObserver (this, AVPlayerItem_Status, observerContext);

				_playerItem = value;

				_playerItem?.AddObserver (this, AVPlayerItem_Status, NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, observerContext);
			}
		}


		public MusicAsset CurrentAsset => Asset;

		MusicAsset _asset;
		MusicAsset Asset {
			get { return _asset; }
			set {
				_asset?.UrlAsset?.RemoveObserver (this, AVUrlAsset_Playable, observerContext);

				_asset = value;

				if (_asset != null)
				{
					_asset.UrlAsset.AddObserver (this, AVUrlAsset_Playable, NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, observerContext);
				}
				else
				{
					PlayerItem = null;

					Player.ReplaceCurrentItemWithPlayerItem (null);

					removeRemoteTransportControls ();

					// observer will set this to true
					readyForPlayback = false;
				}
			}
		}


		AssetPlaybackManager ()
		{
			Player.AddObserver (this, AVPlayer_CurrentItem, NSKeyValueObservingOptions.New, observerContext);
		}

		//Player.RemoveObserver (this, AVPlayer_CurrentItem);

		public bool TogglePlayback () => TogglePlayback (CurrentAsset);

		public bool TogglePlayback (MusicAsset asset)
		{
			if (asset == null)
			{
				Asset = null;
				return false;
			}

			if (Asset != asset)
			{
				Asset = asset;
				return true;
			}

			if (Asset == asset && readyForPlayback)
			{
				if (Player?.Rate == 0)
				{
					Player.Play ();

					updateCommandCenterNowPlayingInfo ();

					return true;
				}

				if (Player?.Rate == 1)
				{
					Player.Pause ();

					updateCommandCenterNowPlayingInfo ();

					return false;
				}
			}

			return false;
		}


		public override void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (context != observerContext)
			{
				Log.Debug ($"context != observerContext | returning");

				base.ObserveValue (keyPath, ofObject, change, context);

				return;
			}

			if (keyPath == null)
			{
				return;
			}


			if (keyPath == AVUrlAsset_Playable)
			{
				Log.Debug ($"ObserveValue: {keyPath} | {Asset?.UrlAsset?.Playable}");

				if ((Asset?.UrlAsset?.Cache?.IsPlayableOffline ?? false) || (Asset?.UrlAsset?.Playable ?? false))
				{
					PlayerItem = new AVPlayerItem (Asset.UrlAsset);

					Player.ReplaceCurrentItemWithPlayerItem (PlayerItem);
				}
				else
				{
					Log.Debug ($"FAILED: {Asset?.UrlAsset?.Url}");
				}
			}
			else if (keyPath == AVPlayerItem_Status)
			{
				if (PlayerItem?.Status == AVPlayerItemStatus.ReadyToPlay)
				{
					Log.Debug ($"PlayerItem.Status = ReadyToPlay | {PlayerItem?.Error}");

					if (!readyForPlayback)
					{
						readyForPlayback = true;

						//ReadyToPlay?.Invoke (this, Player);
						Player.Play ();

						setupRemoteTransportControls ();
					}
					else
					{
						if (Player?.Rate == 0)
						{
							Player.Play ();
						}

						//addPeriodicTimeObserver ();
					}

					updateCommandCenterNowPlayingInfo ();
				}
				else if (PlayerItem?.Status == AVPlayerItemStatus.Unknown)
				{
					//removePeriodicTimeObserver ();
					Log.Debug ($"PlayerItem.Status = Unknown | {PlayerItem?.Error}");
				}
				else if (PlayerItem?.Status == AVPlayerItemStatus.Failed)
				{
					Log.Debug ($"PlayerItem.Status = Failed | {PlayerItem?.Error}");
				}
			}
			else if (keyPath == AVPlayer_CurrentItem)
			{
				Log.Debug ($"ObserveValue: {keyPath} | ");

				CurrentItemChanged?.Invoke (this, Player);
			}
			else
			{
				Log.Debug ($"ObserveValue: {keyPath} | ");

				base.ObserveValue (keyPath, ofObject, change, context);
			}
		}


		#region PeriodicTimeObserver


		void addPeriodicTimeObserver ()
		{
			if (timeObservationToken == null)
			{
				// Notify every half second
				var timeScale = new CMTimeScale (1000000000);

				var time = CMTime.FromSeconds (0.5, timeScale.Value);

				timeObservationToken = Player?.AddPeriodicTimeObserver (time, DispatchQueue.MainQueue, handlePeriodicTimeObserver);
			}
		}


		void handlePeriodicTimeObserver (CMTime obj)
		{
			Log.Debug ($"handlePeriodicTimeObserver: {obj}");

			//MPNowPlayingInfoCenter.DefaultCenter.NowPlaying.PlaybackProgress = obj.Value;
		}


		void removePeriodicTimeObserver ()
		{
			if (timeObservationToken != null)
			{
				Player?.RemoveTimeObserver (timeObservationToken);

				timeObservationToken = null;
			}
		}


		#endregion


		#region Remote Command Center

		NSObject commandCenterChangePlaybackPositionCommandToken;
		NSObject commandCenterNextTrackCommandToken;
		NSObject commandCenterPauseCommandToken;
		NSObject commandCenterPlayCommandToken;
		NSObject commandCenterPreviousTrackCommandToken;
		NSObject commandCenterSeekBackwardCommandToken;
		NSObject commandCenterSeekForwardCommandToken;
		NSObject commandCenterStopCommandToken;
		NSObject commandCenterTogglePlayPauseCommandToken;


		void updateCommandCenterNowPlayingInfo ()
		{
			// Define Now Playing Info
			var nowPlayingInfo = new MPNowPlayingInfo
			{
				Title = Asset.Music.DisplayName,
				ElapsedPlaybackTime = PlayerItem.CurrentTime.Seconds,
				PlaybackDuration = PlayerItem.Asset.Duration.Seconds,
				PlaybackRate = Player.Rate
			};

			// var image = UIImage.FromBundle ("lockscreen");
			// nowPlayingInfo.Artwork = new MPMediaItemArtwork (image.Size, (arg) => { return image; });

			// Set the metadata
			MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = nowPlayingInfo;
		}


		void setupRemoteTransportControls ()
		{
			commandCenterChangePlaybackPositionCommandToken = MPRemoteCommandCenter.Shared.ChangePlaybackPositionCommand.AddTarget (handleCommandCenterChangePlaybackPositionCommand);
			commandCenterNextTrackCommandToken = MPRemoteCommandCenter.Shared.NextTrackCommand.AddTarget (handleCommandCenterNextTrackCommand);
			commandCenterPauseCommandToken = MPRemoteCommandCenter.Shared.PauseCommand.AddTarget (handleCommandCenterPauseCommand);
			commandCenterPlayCommandToken = MPRemoteCommandCenter.Shared.PlayCommand.AddTarget (handleCommandCenterPlayCommand);
			commandCenterPreviousTrackCommandToken = MPRemoteCommandCenter.Shared.PreviousTrackCommand.AddTarget (handleCommandCenterPreviousTrackCommand);
			commandCenterSeekBackwardCommandToken = MPRemoteCommandCenter.Shared.SeekBackwardCommand.AddTarget (handleCommandCenterSeekBackwardCommand);
			commandCenterSeekForwardCommandToken = MPRemoteCommandCenter.Shared.SeekForwardCommand.AddTarget (handleCommandCenterSeekForwardCommand);
			commandCenterStopCommandToken = MPRemoteCommandCenter.Shared.StopCommand.AddTarget (handleCommandCenterStopCommand);
			commandCenterTogglePlayPauseCommandToken = MPRemoteCommandCenter.Shared.TogglePlayPauseCommand.AddTarget (handleCommandCenterTogglePlayPauseCommand);
		}


		void removeRemoteTransportControls ()
		{
			if (commandCenterChangePlaybackPositionCommandToken != null)
				MPRemoteCommandCenter.Shared.ChangePlaybackPositionCommand.RemoveTarget (commandCenterChangePlaybackPositionCommandToken);
			commandCenterChangePlaybackPositionCommandToken = null;

			if (commandCenterNextTrackCommandToken != null)
				MPRemoteCommandCenter.Shared.NextTrackCommand.RemoveTarget (commandCenterNextTrackCommandToken);
			commandCenterNextTrackCommandToken = null;

			if (commandCenterPauseCommandToken != null)
				MPRemoteCommandCenter.Shared.PauseCommand.RemoveTarget (commandCenterPauseCommandToken);
			commandCenterPauseCommandToken = null;

			if (commandCenterPlayCommandToken != null)
				MPRemoteCommandCenter.Shared.PlayCommand.RemoveTarget (commandCenterPlayCommandToken);
			commandCenterPlayCommandToken = null;

			if (commandCenterPreviousTrackCommandToken != null)
				MPRemoteCommandCenter.Shared.PreviousTrackCommand.RemoveTarget (commandCenterPreviousTrackCommandToken);
			commandCenterPreviousTrackCommandToken = null;

			if (commandCenterSeekBackwardCommandToken != null)
				MPRemoteCommandCenter.Shared.SeekBackwardCommand.RemoveTarget (commandCenterSeekBackwardCommandToken);
			commandCenterSeekBackwardCommandToken = null;

			if (commandCenterSeekForwardCommandToken != null)
				MPRemoteCommandCenter.Shared.SeekForwardCommand.RemoveTarget (commandCenterSeekForwardCommandToken);
			commandCenterSeekForwardCommandToken = null;

			if (commandCenterStopCommandToken != null)
				MPRemoteCommandCenter.Shared.StopCommand.RemoveTarget (commandCenterStopCommandToken);
			commandCenterStopCommandToken = null;

			if (commandCenterTogglePlayPauseCommandToken != null)
				MPRemoteCommandCenter.Shared.TogglePlayPauseCommand.RemoveTarget (commandCenterTogglePlayPauseCommandToken);
			commandCenterTogglePlayPauseCommandToken = null;
		}


		MPRemoteCommandHandlerStatus handleCommandCenterChangePlaybackPositionCommand (MPRemoteCommandEvent commandEvent)
		{
			return MPRemoteCommandHandlerStatus.CommandFailed;
		}


		MPRemoteCommandHandlerStatus handleCommandCenterNextTrackCommand (MPRemoteCommandEvent commandEvent)
		{
			return MPRemoteCommandHandlerStatus.CommandFailed;
		}


		MPRemoteCommandHandlerStatus handleCommandCenterPauseCommand (MPRemoteCommandEvent commandEvent)
		{
			if (Player?.Rate == 1)
			{
				Player.Pause ();

				updateCommandCenterNowPlayingInfo ();

				return MPRemoteCommandHandlerStatus.Success;
			}

			return MPRemoteCommandHandlerStatus.CommandFailed;
		}


		MPRemoteCommandHandlerStatus handleCommandCenterPlayCommand (MPRemoteCommandEvent commandEvent)
		{
			if (Player?.Rate == 0)
			{
				Player.Play ();

				updateCommandCenterNowPlayingInfo ();

				return MPRemoteCommandHandlerStatus.Success;
			}

			return MPRemoteCommandHandlerStatus.CommandFailed;
		}


		MPRemoteCommandHandlerStatus handleCommandCenterPreviousTrackCommand (MPRemoteCommandEvent commandEvent)
		{
			return MPRemoteCommandHandlerStatus.CommandFailed;
		}


		MPRemoteCommandHandlerStatus handleCommandCenterSeekBackwardCommand (MPRemoteCommandEvent commandEvent)
		{
			return MPRemoteCommandHandlerStatus.CommandFailed;
		}


		MPRemoteCommandHandlerStatus handleCommandCenterSeekForwardCommand (MPRemoteCommandEvent commandEvent)
		{
			return MPRemoteCommandHandlerStatus.CommandFailed;
		}


		MPRemoteCommandHandlerStatus handleCommandCenterStopCommand (MPRemoteCommandEvent commandEvent)
		{
			return MPRemoteCommandHandlerStatus.CommandFailed;
		}


		MPRemoteCommandHandlerStatus handleCommandCenterTogglePlayPauseCommand (MPRemoteCommandEvent commandEvent)
		{
			if (Player?.Rate == 0)
			{
				Player.Play ();

				updateCommandCenterNowPlayingInfo ();

				return MPRemoteCommandHandlerStatus.Success;
			}

			if (Player?.Rate == 1)
			{
				Player.Pause ();

				updateCommandCenterNowPlayingInfo ();

				return MPRemoteCommandHandlerStatus.Success;
			}

			return MPRemoteCommandHandlerStatus.CommandFailed;
		}

		#endregion
	}
}
