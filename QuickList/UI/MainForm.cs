using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sander.QuickList.Application;
using Sander.QuickList.Application.Enums;

namespace Sander.QuickList.UI
{
	internal partial class MainForm : Form
	{
		private readonly Configuration _configuration;
		private Stopwatch _stopWatch;


		internal MainForm(Configuration configuration)
		{
			_configuration = configuration;
			InitializeComponent();
		}




		private void MainForm_Load(object sender, EventArgs e)
		{
			Text = FormattableString.Invariant($"QuickList v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}");
			CurrentListLabel.Text = Path.GetFileNameWithoutExtension(_configuration.IniFile);

			switch (_configuration.FileInfo)
			{
				case FileInfoLevel.NoInfo:
					FiNoInfo.Checked = true;
					break;
				default:
					FiSize.Checked = true;
					break;
				case FileInfoLevel.Full:
					FiFull.Checked = true;
					break;
			}

			switch (_configuration.FolderHandling)
			{
				case FolderHandling.NoFolders:
					NoFolderRadio.Checked = true;
					break;
				default:

					PartialFolderRadio.Checked = true;
					break;
				case FolderHandling.IncludeFolders:
					IncludeFoldersRadio.Checked = true;
					break;
			}

			DeleteMediaInfo.Enabled = File.Exists(_configuration.MediaCacheFile);
		}

		private void CurrentListLabel_Click(object sender, EventArgs e)
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = _configuration.IniFile,
				UseShellExecute = true,
				Verb = "open"
			});
		}


		private void Disable(Control control)
		{
			if (control is Label || control.Name == "CurrentListPanel")
				return;

			if (control != this)
				control.Enabled = false;
			foreach (Control child in control.Controls)
			{
				Disable(child);
			}
		}

		private void GenerateListButton_Click(object sender, EventArgs e)
		{
			Disable(this);

			_configuration.FolderHandling = NoFolderRadio.Checked ? FolderHandling.NoFolders :
				IncludeFoldersRadio.Checked ? FolderHandling.IncludeFolders : FolderHandling.PartialFolders;

			_configuration.FileInfo = FiNoInfo.Checked ? FileInfoLevel.NoInfo :
				FiFull.Checked ? FileInfoLevel.Full : FileInfoLevel.Size;


			if (DeleteMediaInfo.Checked)
				File.Delete(_configuration.MediaCacheFile);

			_configuration.Update();

			callbackTimer.Enabled = true;
			_stopWatch = Stopwatch.StartNew();

			var main = new Runner(_configuration);
			Task.Run(() => main.MakeList());
		}

		private void ProjectLabel_Click(object sender, EventArgs e)
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "https://github.com/DukeLupus/QuickList",
				UseShellExecute = true,
				Verb = "open"
			});
		}

		private void ReportError_Click(object sender, EventArgs e)
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "https://gitreports.com/issue/DukeLupus/QuickList",
				UseShellExecute = true,
				Verb = "open"
			});
		}



		private void callbackTimer_Tick(object sender, EventArgs e)
		{
			if (_configuration.Status != null)
			{
				UpdateStatus();
			}
		}


		private void UpdateStatus()
		{

			if (_configuration.Status.IsTerminating)
			{
				callbackTimer.Stop();
				progressBar.Value = 100;
				statusLabel.Text = "Finished";
				callbackTimer.Interval = 1000;
				callbackTimer.Tick -= callbackTimer_Tick;
				callbackTimer.Tick += (sender, args) => Close();
				callbackTimer.Start();
				return;
			}

			GenerateListButton.Text = _stopWatch.Elapsed.ToString("mm\\:ss");

			progressBar.Value = (int)_configuration.Status.Percentage;
			statusLabel.Text = _configuration.Status.Text;

		}
	}
}
