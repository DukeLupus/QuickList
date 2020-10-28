using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Sander.QuickList.Application.Enums;

namespace Sander.QuickList.Application
{
	internal sealed class ListMaker
	{
		private readonly Configuration _configuration;
		private readonly List<Entry> _entries;


		/// <inheritdoc />
		internal ListMaker(Configuration configuration, List<Entry> entries)
		{
			_configuration = configuration;
			_configuration.Status = Status.Get("Creating fileLists", 90);
			_entries = entries;
		}


		/// <summary>
		///     Make the !trigger list
		/// </summary>
		internal void CreateNiceList()
		{
			var list = new StringBuilder(_entries.Count * 128);

			if (_configuration.FolderHandling == FolderHandling.NoFolders)
			{
				_entries.ForEach(x => list.AppendLine(FormatEntry(x)));
			}
			else
			{
				//Substring-LastIndexOf performs considerably better than Path.GetDirectoryName()
				foreach (var grouping in _entries.GroupBy(x => x.Fullname.Substring(0, x.Fullname.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase))))
				{
					var folder = _configuration.FolderHandling == FolderHandling.PartialFolders ? CreatePartialFolder(grouping.Key) : grouping.Key;

					var eqLine = new string('=', folder.Length);
					list.AppendLine();
					list.AppendLine(eqLine);
					list.AppendLine(folder);
					list.AppendLine(eqLine);
					foreach (var entry in grouping)
					{
						list.AppendLine(FormatEntry(entry));
					}
				}
			}

			if (!string.IsNullOrWhiteSpace(_configuration.HeaderFile) && File.Exists(_configuration.HeaderFile))
			{
				list.Insert(0, Environment.NewLine);
				list.Insert(0, File.ReadAllText(_configuration.HeaderFile));
			}

			var fileName = Path.Combine(_configuration.OutputFolder, "nicelist.txt");

			using (var sw = new StreamWriter(fileName, false, Encoding.UTF8, 2 << 16 /* 128KB*/))
			{
				sw.Write(list.ToString());
			}
		}


		/// <summary>
		///     Get entry as it appears in the list
		/// </summary>
		internal string FormatEntry(Entry entry)
		{
			//var formattedEntry = FormattableString.Invariant($"{_configuration.Trigger} {Path.GetFileName(entry.Fullname)}");
			var formattedEntry = FormattableString.Invariant($"{_configuration.Trigger} {Utils.GetFilename(entry.Fullname)}");
			if (_configuration.FileInfo == FileInfoLevel.NoInfo)
			{
				return formattedEntry;
			}

			return FormattableString.Invariant(
				$"{formattedEntry} ::INFO::{Utils.ReadableSize(entry.Size, " 0.##")}{(_configuration.FileInfo == FileInfoLevel.Full ? entry.MediaInfo : string.Empty)}");
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private string CreatePartialFolder(string folder)
		{
			foreach (var inputFolder in _configuration.InputFolders)
			{
				if (folder.StartsWith(inputFolder, StringComparison.OrdinalIgnoreCase))
				{
					folder = folder.Substring(inputFolder.Length).Trim('\\');
					if (folder.Length == 0)
					{
						folder = _configuration.ListName;
					}

					break;
				}
			}

			return folder;
		}


		/// <summary>
		///     Creates Statfile.txt in same folder as QuickList.exe.
		///     The format seems to be - per line:
		///     Size in bytes
		///     Number of entries
		///     Entries/second
		///     Total time in format hh:mm:ss
		///     Empty line
		///     Input ini file, excl. path
		/// </summary>
		internal void CreateStatFile(TimeSpan duration)
		{
			var stats = new StringBuilder();
			stats.AppendLine(_entries.Sum(x => x.Size)
				.ToString(CultureInfo.InvariantCulture));

			stats.AppendLine(_entries.Count.ToString(CultureInfo.InvariantCulture));
			if (duration.TotalSeconds > 1)
			{
				stats.AppendLine(Math.Ceiling(_entries.Count / duration.TotalSeconds)
					.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				stats.AppendLine(_entries.Count.ToString(CultureInfo.InvariantCulture));
			}

			stats.AppendLine(duration.ToString("c"));
			stats.AppendLine();
			stats.AppendLine(Path.GetFileName(_configuration.IniFile));

			// ReSharper disable once AssignNullToNotNullAttribute
			var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly()
				.Location), "statfile.txt");

			using (var sw = new StreamWriter(fileName, false, Encoding.UTF8, 2 << 16 /* 128KB*/))
			{
				sw.Write(stats.ToString());
			}
		}


		/// <summary>
		///     Create "list.txt", just fullpath files
		/// </summary>
		internal void CreateFileList()
		{
			var list = new StringBuilder(_entries.Count * 128);

			foreach (var entry in _entries)
			{
				list.AppendLine(entry.Fullname);
			}

			var fileName = Path.Combine(_configuration.OutputFolder, "list.txt");

			using (var sw = new StreamWriter(fileName, false, Encoding.UTF8, 2 << 16 /* 128KB*/))
			{
				sw.Write(list.ToString());
			}
		}
	}
}
