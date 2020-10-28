using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sander.QuickList.Application.Enums;

namespace Sander.QuickList.Application
{
	internal sealed class Configuration
	{
		internal List<string> InputFolders { get; set; } = new List<string>();

		/// <summary>
		///     !trigger
		/// </summary>
		internal string Trigger { get; set; }

		/// <summary>
		///     Optional header file
		/// </summary>
		internal string HeaderFile { get; set; }

		/// <summary>
		///     Output file folder
		/// </summary>
		internal string OutputFolder { get; set; }

		/// <summary>
		///     How to include the folders?
		/// </summary>
		internal FolderHandling FolderHandling { get; set; } = FolderHandling.PartialFolders;

		internal bool ShowUi { get; set; }

		/// <summary>
		///     How much file info should be included?
		/// </summary>
		internal FileInfoLevel FileInfo { get; set; }

		/// <summary>
		///     List name, as per ini file name
		/// </summary>
		internal string ListName { get; set; }

		/// <summary>
		///     Current ini file
		/// </summary>
		internal string IniFile { get; set; }

		/// <summary>
		///     Media cache file
		/// </summary>
		internal string MediaCacheFile => Path.Combine(OutputFolder, "MediaCache.ql3");

		/// <summary>
		///     Force all media fetching to be done by shell, not TagLib#
		/// </summary>
		internal bool ForceShellMedia { get; set; }

		/// <summary>
		///     Status/progress to display on form
		/// </summary>
		internal Status Status { get; set; }

		/// <summary>
		///     How many threads to run at once for file reader?
		/// </summary>
		internal int FileReaderParallelism { get; set; }

		/// <summary>
		///     Excluded extensions
		/// </summary>
		internal List<string> ExcludedExtensions { get; set; }

		/// <summary>
		///     Excluded file names. Defaults to "desktop.ini:thumbs.db"
		/// </summary>
		internal List<string> ExcludedFilenames { get; set; }


		internal static Configuration Parse(string iniFile)
		{
			var configuration = new Configuration
			{
				IniFile = iniFile,
				HeaderFile = IniReader.ReadValue("ListMagic", "HeaderFile", iniFile),
				Trigger = IniReader.ReadValue("ListMagic", "trigger", iniFile),
				OutputFolder = IniReader.ReadValue("ListMagic", "ListLocation", iniFile),
				ShowUi = string.Compare(IniReader.ReadValue("ListMagic", "Auto", iniFile, "Y").Trim(), "N", StringComparison.OrdinalIgnoreCase) == 0,
				ListName = Path.GetFileNameWithoutExtension(iniFile),
				FileInfo = (FileInfoLevel)Enum.Parse(typeof(FileInfoLevel), IniReader.ReadValue("QuickList", "FileInfo", iniFile, nameof(FileInfoLevel.Size))),
				FolderHandling = (FolderHandling)Enum.Parse(typeof(FolderHandling),
					IniReader.ReadValue("QuickList", "FolderHandling", iniFile, nameof(FolderHandling.PartialFolders))),
				ForceShellMedia = IniReader.ReadValue("QuickList", "ForceShellMedia", iniFile, "0") == "1"
			};

			var excludedExtensions = IniReader.ReadValue("ListMagic", "Exclude", iniFile);

			if (!string.IsNullOrWhiteSpace(excludedExtensions))
			{
				configuration.ExcludedExtensions = excludedExtensions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim().TrimStart('.').ToLowerInvariant())
					.ToList();
			}

			int.TryParse(IniReader.ReadValue("QuickList", "FileReaderParallelism", iniFile, "1"), out var fileReaderParallelism);
			if (fileReaderParallelism <= 0)
			{
				fileReaderParallelism = 1;
			}

			configuration.FileReaderParallelism = fileReaderParallelism;

			var excludedFilenames = IniReader.ReadValue("QuickList", "ExcludedFilenames", iniFile, "desktop.ini:thumbs.db");

			if (!string.IsNullOrWhiteSpace(excludedFilenames))
			{
				configuration.ExcludedFilenames = excludedFilenames
					.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim())
					.ToList();
			}

			var dirsFile = IniReader.ReadValue("ListMagic", "DirsFile", iniFile);

			if (!File.Exists(dirsFile))
			{
				throw new FileNotFoundException($"Folders file \"{dirsFile}\" does not exist!");
			}

			configuration.InputFolders = File.ReadAllLines(dirsFile)
				.Select(x => x.Trim())
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Distinct()
				.ToList();
			configuration.Validate();

			return configuration;
		}


		internal void Validate()
		{
			if (!File.Exists(HeaderFile))
			{
				HeaderFile = null;
			}

			if (!Trigger.StartsWith("!", StringComparison.Ordinal))
			{
				Trigger = $"!{Trigger}";
			}

			Update();

			Directory.CreateDirectory(OutputFolder);
		}


		internal void Update()
		{
			IniReader.WriteValue("QuickList", "FolderHandling", FolderHandling.ToString(), IniFile);
			IniReader.WriteValue("QuickList", "FileInfo", FileInfo.ToString(), IniFile);
			IniReader.WriteValue("QuickList", "ForceShellMedia", ForceShellMedia ? "1" : "0", IniFile);
			IniReader.WriteValue("QuickList", "FileReaderParallelism", FileReaderParallelism.ToString(), IniFile);

			if (ExcludedFilenames?.Count > 0)
			{
				IniReader.WriteValue("QuickList", "ExcludedFilenames", string.Join(":", ExcludedFilenames), IniFile);
			}
		}


		/// <inheritdoc />
		public override string ToString()
		{
			var sb = new StringBuilder("\r\n=== Configuration ===\r\n");
			sb.AppendLine($"IniFile: {IniFile}");
			sb.AppendLine($"ListName: {ListName}");
			sb.AppendLine($"Trigger: {Trigger}");
			sb.AppendLine($"InputFolders: {string.Join(";", InputFolders)}");
			sb.AppendLine($"OutputFolder: {OutputFolder}");
			sb.AppendLine($"HeaderFile: {HeaderFile}");
			sb.AppendLine($"FileInfoLevel: {FileInfo}");
			sb.AppendLine($"FolderHandling: {FolderHandling}");
			sb.AppendLine($"ShowUi: {ShowUi}");
			sb.AppendLine($"FileReaderParallelism: {FileReaderParallelism}");
			sb.AppendLine($"ExcludedExtensions: {string.Join(";", ExcludedExtensions)}");
			sb.AppendLine($"ExcludedFilenames: {string.Join(";", ExcludedFilenames)}");
			sb.AppendLine($"MediaCacheFile: {MediaCacheFile}");
			sb.AppendLine($"ForceShellMedia: {ForceShellMedia}");
			return sb.ToString();
		}
	}
}
