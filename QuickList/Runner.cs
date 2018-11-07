using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sander.QuickList.Application;
using Sander.QuickList.Application.Enums;

namespace Sander.QuickList
{
	internal sealed class Runner
	{
		private readonly Configuration _configuration;


		/// <inheritdoc />
		internal Runner(Configuration configuration)
		{
			_configuration = configuration;
		}


		internal void MakeList()
		{
			var sw = Stopwatch.StartNew();
			var entries = new FileReader(_configuration).GetEntries();

			if (entries == null)
			{
				return;
			}

			Trace.TraceInformation($"Getting {entries.Count} files took {sw.Elapsed}");
			Trace.Flush();
			var tasks = new Task[2];

			if (_configuration.FileInfo == FileInfoLevel.Full)
			{
				var sw2 = Stopwatch.StartNew();
				tasks[0] = new MediaInfoProvider(_configuration).AddMediaInfo(entries);
				Trace.TraceInformation($"Getting media info took {sw2.Elapsed}");
			}

			Trace.Flush();


			var listmaker = new ListMaker(_configuration, entries);

			tasks[1] = Task.Run(() => listmaker.CreateFileList());
			var sw3 = Stopwatch.StartNew();
			listmaker.CreateNiceList();
			Trace.TraceInformation($"Time making the public (\"nice\") list: {sw3.Elapsed}");

			listmaker.CreateStatFile(sw.Elapsed);
			Task.WaitAll(tasks.Where(x => x != null).ToArray());

			Trace.TraceInformation($"Total time: {sw.Elapsed}");
			Trace.Flush();
			_configuration.Status = Status.Get(string.Empty, 100, true);
		}
	}
}
