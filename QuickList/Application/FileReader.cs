using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sander.QuickList.Application.Enums;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

// ReSharper disable InconsistentNaming

namespace Sander.QuickList.Application
{
	internal sealed class FileReader
	{
		private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
		private readonly Configuration _configuration;


		/// <inheritdoc />
		internal FileReader(Configuration configuration)
		{
			_configuration = configuration;
		}


		internal List<Entry> GetEntries()
		{
			_configuration.Status = Status.Get("Gathering files", 0);
			if (_configuration.InputFolders == null || _configuration.InputFolders?.Count == 0)
			{
				Trace.TraceError("No input folders found");
				Program.OpenLog = true;

				return null;
			}

			var fileList = ListFiles();

			if (fileList.Count == 0)
			{
				Trace.TraceError("Input folders did not contain any files");
				Program.OpenLog = true;

				return null;
			}

			return fileList;
		}


		private List<Entry> ListFiles()
		{
			double folderRange = _configuration.FileInfo == FileInfoLevel.Full ? 40 : 80;

			var step = folderRange / _configuration.InputFolders.Count / 2;
			var percentage = 0d;

			var fileList = new List<Entry>(100000);
			foreach (var inputFolder in _configuration.InputFolders)
			{
				percentage += step;
				_configuration.Status = Status.Get("Gathering files", percentage);

				if (!Directory.Exists(inputFolder))
				{
					Trace.TraceWarning($"Input folder \"{inputFolder}\" does not exist!");
					Program.OpenLog = true;
					continue;
				}

				var files = ParallelFindNextFile(inputFolder);

				if (files.Count == 0)
				{
					Trace.TraceWarning($"Input folder \"{inputFolder}\" does not contain any files!");
					Program.OpenLog = true;
					continue;
				}

				//sort here, so we still have same input folder order
				fileList.AddRange(files.OrderBy(x => x.Fullname));

				percentage += step;
				_configuration.Status = Status.Get("Gathering files", percentage);
			}

			if (_configuration.ExcludedFilenames?.Count > 0)
			{
				ExcludeByName(fileList);
			}

			if (_configuration.ExcludedExtensions?.Count > 0)
			{
				ExcludeByExtension(fileList);
			}

			return fileList;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ExcludeByName(List<Entry> fileList)
		{
			foreach (var filename in _configuration.ExcludedFilenames)
			{
				fileList.RemoveAll(x => string.Compare(Utils.GetFilename(x.Fullname), filename, StringComparison.OrdinalIgnoreCase) == 0);
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ExcludeByExtension(List<Entry> fileList)
		{
			foreach (var extension in _configuration.ExcludedExtensions)
			{
				fileList.RemoveAll(x => string.Compare(Utils.GetExtension(x.Fullname), extension, StringComparison.OrdinalIgnoreCase) == 0);
			}
		}


		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData);


		[DllImport("kernel32.dll")]
		private static extern bool FindClose(IntPtr hFindFile);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern IntPtr FindFirstFileEx(
			string lpFileName,
			FINDEX_INFO_LEVELS fInfoLevelId,
			out WIN32_FIND_DATAW lpFindFileData,
			FINDEX_SEARCH_OPS fSearchOp,
			IntPtr lpSearchFilter,
			int dwAdditionalFlags);


		private static bool FindNextFile(string path, out List<Entry> files)
		{
			var fileList = new List<Entry>(32);
			var findHandle = INVALID_HANDLE_VALUE;

			try
			{
				findHandle = FindFirstFileEx(string.Concat(path, "\\*"), FINDEX_INFO_LEVELS.FindExInfoBasic, out var findData,
					FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, 2);

				if (findHandle != INVALID_HANDLE_VALUE)
				{
					do
					{
						// Skip current directory and parent directory symbols that are returned.
						if (!findData.cFileName.Equals(".", StringComparison.OrdinalIgnoreCase) && !findData.cFileName.Equals("..",
							StringComparison.OrdinalIgnoreCase))
						{
							var fullPath = string.Concat(path, "\\", findData.cFileName);
							// Check if this is a directory and not a symbolic link since symbolic links could lead to repeated files and folders as well as infinite loops.
							if ((findData.dwFileAttributes & FileAttributes.Directory) != 0 && (findData.dwFileAttributes & FileAttributes.ReparsePoint) == 0)
							{
								if (FindNextFile(fullPath, out var subDirectoryFileList))
								{
									fileList.AddRange(subDirectoryFileList);
								}
							}
							else if ((findData.dwFileAttributes & FileAttributes.Directory) == 0)
							{
								fileList.Add(new Entry
								{
									Fullname = fullPath,
									Size = ((long)findData.nFileSizeHigh << 0x20) | findData.nFileSizeLow
								});
							}
						}
					} while (FindNextFile(findHandle, out findData));
				}
			}
			catch (Exception exception)
			{
				Trace.TraceError("Exception while trying to enumerate a directory. {0}", exception);
				if (findHandle != INVALID_HANDLE_VALUE)
				{
					FindClose(findHandle);
				}

				files = null;
				return false;
			}

			if (findHandle != INVALID_HANDLE_VALUE)
			{
				FindClose(findHandle);
			}

			files = fileList;
			return true;
		}


		private List<Entry> ParallelFindNextFile(string path)
		{
			var fileList = new ConcurrentBag<Entry>();
			var directoryList = new List<string>(32);

			var findHandle = INVALID_HANDLE_VALUE;
			try
			{
				path = path[path.Length - 1] == '\\' ? path : FormattableString.Invariant($@"{path}\");
				findHandle = FindFirstFileEx(string.Concat(path, "*"), FINDEX_INFO_LEVELS.FindExInfoBasic, out var findData,
					FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, 2);

				if (findHandle != INVALID_HANDLE_VALUE)
				{
					do
					{
						// Skip current directory and parent directory symbols that are returned.
						if (!findData.cFileName.Equals(".", StringComparison.OrdinalIgnoreCase) && !findData.cFileName.Equals("..",
							StringComparison.OrdinalIgnoreCase))
						{
							var fullPath = path + findData.cFileName;
							// Check if this is a directory and not a symbolic link since symbolic links could lead to repeated files and folders as well as infinite loops.
							if ((findData.dwFileAttributes & FileAttributes.Directory) != 0 && (findData.dwFileAttributes & FileAttributes.ReparsePoint) == 0)
							{
								directoryList.Add(fullPath);
							}
							else if ((findData.dwFileAttributes & FileAttributes.Directory) == 0)
							{
								fileList.Add(new Entry
								{
									Fullname = fullPath,
									Size = ((long)findData.nFileSizeHigh << 0x20) | findData.nFileSizeLow
								});
							}
						}
					} while (FindNextFile(findHandle, out findData));

					directoryList
						.AsParallel()
						.WithDegreeOfParallelism(_configuration.FileReaderParallelism)
						.WithExecutionMode(ParallelExecutionMode.ForceParallelism)
						.ForAll(x =>
						{
							if (FindNextFile(x, out var subDirectoryFileList))
							{
								foreach (var entry in subDirectoryFileList)
								{
									fileList.Add(entry);
								}
							}
						});
				}
			}
			catch (Exception exception)
			{
				Trace.TraceError($"Caught exception while trying to enumerate a directory. {exception}");
				if (findHandle != INVALID_HANDLE_VALUE)
				{
					FindClose(findHandle);
				}

				return fileList.ToList();
			}

			if (findHandle != INVALID_HANDLE_VALUE)
			{
				FindClose(findHandle);
			}

			return fileList.ToList();
		}


		private enum FINDEX_INFO_LEVELS
		{
			FindExInfoBasic = 1
		}

		private enum FINDEX_SEARCH_OPS
		{
			FindExSearchNameMatch = 0
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct WIN32_FIND_DATAW
		{
			internal readonly FileAttributes dwFileAttributes;
			internal readonly FILETIME ftCreationTime;
			internal readonly FILETIME ftLastAccessTime;
			internal readonly FILETIME ftLastWriteTime;
			internal readonly uint nFileSizeHigh;
			internal readonly uint nFileSizeLow;
			internal readonly int dwReserved0;
			internal readonly int dwReserved1;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal readonly string cFileName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			internal readonly string cAlternateFileName;
		}
	}
}
