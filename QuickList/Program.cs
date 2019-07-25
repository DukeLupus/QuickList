using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sander.QuickList.Application;
using Sander.QuickList.Application.Enums;
using Sander.QuickList.UI;

namespace Sander.QuickList
{
	internal static class Program
	{
		/// <summary>
		/// Default log file. Trace writer can use different file if locked
		/// </summary>
		private const string QuicklistLog = "QuickList.log";

		internal const string Version = "3.0 beta 6";

		private static string _traceName;

		internal static bool OpenLog { get; set; }


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(params string[] parameters)
		{
#if DEV
			Initialize(@"D:\mIRC\OmenServe\ListMagic\testaudio\testaudio.ini");
#else
			Initialize(parameters);
#endif
			//not using mutex, as I have no idea what was the mutex name for QuickList v2
			if (Process.GetProcessesByName("QuickList").Length > 1)
			{
				MessageBox.Show("Another instance of QuickList is already running", "QuickList", MessageBoxButtons.OK);
				Environment.Exit(1);
			}
#if !DEV
			if (parameters.Length == 0)
			{
				MessageBox.Show("Commandline parameter pointing to ini file is missing!", "QuickList", MessageBoxButtons.OK);
				Environment.Exit(2);
			}

			var configuration = Configuration.Parse(parameters[0]);
#else
			var configuration = new Configuration
			{
				FileInfo = FileInfoLevel.Full,
				Trigger = "!DukeLupus",
				FolderHandling = FolderHandling.PartialFolders,
				InputFolders = new List<string> { @"c:\temp\", @"C:\Dev" },
				OutputFolder = @"c:\temp\out",
				ListName = "audio",
				IniFile = @"c:\temp\out\testaudio.ini",
				ShowUi = true,
				ForceShellMedia = false,
				FileReaderParallelism = 4,
				ExcludedExtensions = new List<string> { "txt", "json", "xml", "dll", "pdb"},
				ExcludedFilenames = new List<string> { "desktop.ini" },
			};
#endif
			Trace.TraceInformation($"QuickList v{Version}");
			Trace.TraceInformation(configuration.ToString());
			configuration.Validate();

			if (configuration.ShowUi)
			{
				var form = new MainForm(configuration);

				System.Windows.Forms.Application.Run(form);
			}
			else
			{
				var main = new Runner(configuration);
				main.MakeList();
			}

#if DEBUG
			OpenLog = true;
#endif
			if (OpenLog && File.Exists(_traceName))
				Process.Start(new ProcessStartInfo
				{
					FileName = _traceName,
					UseShellExecute = true,
					Verb = "open"
				});


		}


		private static void Initialize(params string[] parameters)
		{
			_traceName = parameters.Length > 0 && !string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(parameters[0]))
				? $"{Path.GetFileNameWithoutExtension(parameters[0])}.{QuicklistLog}"
				: QuicklistLog;

			//delete old log file
			File.Delete(_traceName);

			Trace.Listeners.Clear();
			Trace.Listeners.Add(new TextWriterTraceListener(_traceName)
			{
				IndentLevel = 2,
				TraceOutputOptions = TraceOptions.DateTime
			});

			Trace.AutoFlush = false;

			System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
			AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args)
														  {
															  LogUnhandledException(args.ExceptionObject as Exception,
																  $"Uncaught exception: {sender}, terminating: {args.IsTerminating}");
														  };

			TaskScheduler.UnobservedTaskException += delegate (object sender, UnobservedTaskExceptionEventArgs args)
													 {
														 LogUnhandledException(args.Exception,
															 $"Uncaught task exception: {sender}");
														 args.SetObserved();
													 };
		}


		private static void LogUnhandledException(Exception ex, string message)
		{
			MessageBox.Show($"Fatal error generating list. Check the {QuicklistLog} for more details\r\n\r\n{ex.Message}",
				"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			OpenLog = true;
			Trace.TraceError(FormattableString.Invariant($"{message}\r\n{ex}"));
			Trace.Flush();
		}
	}
}
