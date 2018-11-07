using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sander.QuickList.Application.Enums;

namespace Sander.QuickList.Application
{
	internal sealed class Configuration
	{
		internal List<string> InputFolders { get; set; } = new List<string>();

		/// <summary>
		/// !trigger
		/// </summary>
		internal string Trigger { get; set; }

		/// <summary>
		/// Optional header file
		/// </summary>
		internal string HeaderFile { get; set; }

		/// <summary>
		/// Output file folder
		/// </summary>
		internal string OutputFolder { get; set; }

		/// <summary>
		/// How to include the folders?
		/// </summary>
		internal FolderHandling FolderHandling { get; set; } = FolderHandling.PartialFolders;

		internal bool ShowUi { get; set; }

		/// <summary>
		/// How much file info should be included?
		/// </summary>
		internal FileInfoLevel FileInfo { get; set; }

		/// <summary>
		/// List name, as per ini file name
		/// </summary>
		internal string ListName { get; set; }

		/// <summary>
		/// Current ini file
		/// </summary>
		internal string IniFile { get; set; }

		/// <summary>
		/// Media cache file
		/// </summary>
		internal string MediaCacheFile => Path.Combine(OutputFolder, "MediaCache.ql3");

		/// <summary>
		/// Force all media fetching to be done by shell, not TagLib#
		/// </summary>
		internal bool ForceShellMedia { get; set; }

		internal Status Status { get; set; }


		internal static Configuration Parse(string iniFile)
		{
			var configuration = new Configuration
			{
				IniFile = iniFile,
				HeaderFile = IniReader.ReadValue("ListMagic", "HeaderFile", iniFile),
				Trigger = IniReader.ReadValue("ListMagic", "trigger", iniFile),
				OutputFolder = IniReader.ReadValue("ListMagic", "ListLocation", iniFile),
				ShowUi = IniReader.ReadValue("ListMagic", "Auto", iniFile) == "N",
				ListName = Path.GetFileNameWithoutExtension(iniFile),
				FileInfo = (FileInfoLevel)Enum.Parse(typeof(FileInfoLevel), IniReader.ReadValue("QuickList", "FileInfo", iniFile, nameof(FileInfoLevel.Size))),
				FolderHandling = (FolderHandling)Enum.Parse(typeof(FolderHandling),
					IniReader.ReadValue("QuickList", "FolderHandling", iniFile, nameof(FolderHandling.PartialFolders))),
				ForceShellMedia = IniReader.ReadValue("QuickList", "ForceShellMedia", iniFile, "0") == "1"
			};

			var dirsFile = IniReader.ReadValue("ListMagic", "DirsFile", iniFile);

			if (!File.Exists(dirsFile))
				throw new FileNotFoundException($"Folders file \"{dirsFile}\" does not exist!");

			var folders = File.ReadAllLines(dirsFile)
							  .Select(x => x.Trim())
							  .Where(x => !string.IsNullOrWhiteSpace(x))
							  .Distinct()
							  .ToList();

			configuration.InputFolders = folders;
			configuration.Validate();

			return configuration;
		}


		internal void Validate()
		{
			if (!File.Exists(HeaderFile))
				HeaderFile = null;

			if (!Trigger.StartsWith("!", StringComparison.Ordinal))
				Trigger = $"!{Trigger}";

			Update();

			Directory.CreateDirectory(OutputFolder);
		}


		internal void Update()
		{
			IniReader.WriteValue("QuickList", "FolderHandling", FolderHandling.ToString(), IniFile);
			IniReader.WriteValue("QuickList", "FileInfo", FileInfo.ToString(), IniFile);
			IniReader.WriteValue("QuickList", "ForceShellMedia", ForceShellMedia ? "1" : "0", IniFile);
		}
	}
}
