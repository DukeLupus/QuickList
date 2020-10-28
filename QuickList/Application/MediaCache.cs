using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sander.QuickList.Application
{
	internal sealed class MediaCache
	{
		private readonly Configuration _configuration;
		private readonly ConcurrentBag<Entry> _newCache = new ConcurrentBag<Entry>();
		private Dictionary<string, Entry> _mediaCache;


		/// <inheritdoc />
		internal MediaCache(Configuration configuration)
		{
			_configuration = configuration;
			if (File.Exists(configuration.MediaCacheFile))
			{
				LoadMediaCache();
			}
		}


		private void LoadMediaCache()
		{
			var cacheLines = File.ReadAllLines(_configuration.MediaCacheFile);
			_mediaCache = new Dictionary<string, Entry>();
			foreach (var cacheLine in cacheLines)
			{
				var split = cacheLine.Split('|');
				_mediaCache[split[0]] = new Entry
				{
					Size = long.Parse(split[1]),
					MediaInfo = split[2]
				};
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool GetCachedInfo(Entry entry)
		{
			if (_mediaCache == null || _mediaCache.Count == 0)
			{
				return false;
			}

			if (_mediaCache.TryGetValue(entry.Fullname, out var cached)
			    && cached.Size == entry.Size
			    && !string.IsNullOrWhiteSpace(cached.MediaInfo))
			{
				entry.MediaInfo = cached.MediaInfo;
				return true;
			}

			return false;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void AddEntry(Entry entry)
		{
			if (entry.MediaInfo != null)
			{
				_newCache.Add(entry);
			}
		}


		internal void UpdateMediaCache()
		{
			if (_newCache.Count > 0)
			{
				var sb = new StringBuilder(_newCache.Count * 128);
				sb.Append(string.Join(Environment.NewLine, _newCache
					.Where(x => !string.IsNullOrWhiteSpace(x.MediaInfo))
					.Select(x => FormattableString.Invariant($"{x.Fullname}|{x.Size}|{x.MediaInfo}"))));

				using (var sw = new StreamWriter(_configuration.MediaCacheFile, false, Encoding.UTF8, 2 << 16 /* 128KB*/))
				{
					sw.Write(sb.ToString());
				}
			}
		}
	}
}
