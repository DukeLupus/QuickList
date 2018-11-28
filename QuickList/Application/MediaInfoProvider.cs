using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using Sander.QuickList.TagLib;
using File = Sander.QuickList.TagLib.File;
using Timer = System.Timers.Timer;

namespace Sander.QuickList.Application
{
	internal sealed class MediaInfoProvider
	{
		private readonly Configuration _configuration;
		private readonly MediaCache _mediaCache;
		private readonly HashSet<string> _mediaFormats;
		private readonly HashSet<string> _tagSharpFormats;


		/// <inheritdoc />
		internal MediaInfoProvider(Configuration configuration)
		{
			_configuration = configuration;

			_tagSharpFormats = new HashSet<string>(
@"aa, aac, aax, aif, aiff, ape, asf, avi, divx, dsf, fla, flac, m2a, m2v, m4a, m4b, m4p, m4v,
mka, mks, mkv, mp+, mp1, mp2, mp3, mp4, mpc, mpe, mpeg, mpg, mpp, mpv2, oga, ogg, ogv, opus, wav, webm, wma, wmv, wv"
					.Split(',').Select(x => $".{x.Trim()}"),
				StringComparer.OrdinalIgnoreCase);

			_mediaFormats = new HashSet<string>(_tagSharpFormats, StringComparer.OrdinalIgnoreCase);

			var additionalFormats =
				@"3g2, 3gp, act, alax, amd, awb, dct, dss, dvf,
f4a, f4b, f4v, flv, m4a, mov, mts, mts2, qt, vob"
					.Split(',').Select(x => $".{x.Trim()}").ToList();

			additionalFormats.ForEach(x => _mediaFormats.Add(x));

			_mediaCache = new MediaCache(_configuration);
		}


		internal Task AddMediaInfo(List<Entry> entries)
		{
			var step = Status.CurrentValue / entries.Count;
			var percentage = Status.CurrentValue;

			Timer timer = null;

			if (_configuration.ShowUi)
			{
				timer = new Timer(250)
				{
					Enabled = true
				};
				timer.Elapsed += (sender, args) => _configuration.Status = Status.Get("Getting media info", percentage);
			}

			entries
				.AsParallel()
				.WithDegreeOfParallelism(_configuration.FileReaderParallelism)
				.WithExecutionMode(ParallelExecutionMode.ForceParallelism)
				//.ForEach(entry =>
				.ForAll(entry =>
						 {

							 percentage += step;

							 var extension = Path.GetExtension(entry.Fullname) ?? string.Empty;

							 if (!_configuration.ForceShellMedia && _tagSharpFormats.Contains(extension))
							 {
								 if (!_mediaCache.GetCachedInfo(entry) && !GetTagLibInfo(entry))
									 GetShellInfo(entry);

								 _mediaCache.AddEntry(entry);
								 return;
							 }

							 if (_mediaFormats.Contains(extension))
							 {
								 if (!_mediaCache.GetCachedInfo(entry))
									 GetShellInfo(entry);
								 _mediaCache.AddEntry(entry);
							 }
						 });

			timer?.Dispose();

			return Task.Run(() => _mediaCache.UpdateMediaCache());
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool GetTagLibInfo(Entry entry)
		{
			try
			{
				using (var file = File.Create(new File.LocalFileAbstraction(entry.Fullname), null, ReadStyle.Average))
				{
					if (file == null || file.Properties.Duration == TimeSpan.Zero)
						return false;

					if ((file.Properties.MediaTypes & MediaTypes.Video) != 0)
					{
						entry.MediaInfo = FormatVideo(file.Properties.Duration, file.Properties.VideoWidth, file.Properties.VideoHeight, null);
						return true;
					}

					if ((file.Properties.MediaTypes & MediaTypes.Audio) != 0)
					{
						entry.MediaInfo = FormatAudio(file.Properties.Duration, file.Properties.AudioBitrate, file.Properties.AudioSampleRate / 1000f,
							file.Properties.AudioChannels);
						return true;
					}
				}

				return false;
			}
			catch (Exception e)
			{
				Trace.WriteLine($"Problem getting media info for \"{entry.Fullname}\". Error:\r\n{e.Source}: {e.Message}");
				return false;
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GetShellInfo(Entry entry)
		{
			lock (this)
			{
				using (var shellFile = ShellFile.FromFilePath(entry.Fullname))
				{
					var durationNs = shellFile.Properties.System.Media.Duration.Value;
					if (durationNs == null || durationNs == 0)
						return;

					var duration = TimeSpan.FromMilliseconds(durationNs.Value * 0.0001);
					//cheap video detection
					if (shellFile.Properties.System.Video.FrameWidth.Value.HasValue)
					{
						var video = shellFile.Properties.System.Video;
						entry.MediaInfo = FormatVideo(duration,
							(int)(video.FrameWidth.Value ?? 0),
							(int)(video.FrameHeight.Value ?? 0),
							video.FrameRate.Value / 1000);
					}
					else if (shellFile.Properties.System.Audio.ChannelCount.Value.HasValue)
					{
						entry.MediaInfo = FormatAudio(duration,
							(int)((shellFile.Properties.System.Audio.EncodingBitrate.Value ?? 0) / 1000),
							(shellFile.Properties.System.Audio.SampleRate.Value ?? 0) / 1000f,
							(int)shellFile.Properties.System.Audio.ChannelCount.Value.Value);

					}
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string FormatVideo(TimeSpan duration, int width, int height, uint? framerate)
		{
			//MediaInfo must start with a space
			return FormattableString.Invariant(
				$" {duration.ToString("m'm'''ss's'")} {width}x{height}{(framerate.HasValue ? string.Concat("/", framerate, "fps") : string.Empty)}");
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string FormatAudio(TimeSpan duration, int bitrate, float samplerate, int channelCount)
		{
			//MediaInfo must start with a space
			return FormattableString.Invariant($" {duration.ToString("m'm'''ss's'")} {bitrate}/{samplerate}/{(channelCount == 2 ? 'S' : 'M')}");
		}
	}
}
