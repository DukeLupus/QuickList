using System.ComponentModel;
using System.Windows.Forms;

namespace Sander.QuickList.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.CurrentListPanel = new System.Windows.Forms.Panel();
			this.CurrentListLabel = new System.Windows.Forms.Label();
			this.CurrentListTitle = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.FiNoInfo = new System.Windows.Forms.RadioButton();
			this.FiSize = new System.Windows.Forms.RadioButton();
			this.FiFull = new System.Windows.Forms.RadioButton();
			this.PartialFolderRadio = new System.Windows.Forms.RadioButton();
			this.IncludeFoldersRadio = new System.Windows.Forms.RadioButton();
			this.DeleteMediaInfo = new System.Windows.Forms.CheckBox();
			this.NoFolderRadio = new System.Windows.Forms.RadioButton();
			this.ProjectLabel = new System.Windows.Forms.Label();
			this.ReportError = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.GenerateListButton = new System.Windows.Forms.Button();
			this.callbackTimer = new System.Windows.Forms.Timer(this.components);
			this.statusStrip1.SuspendLayout();
			this.CurrentListPanel.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.progressBar});
			this.statusStrip1.Location = new System.Drawing.Point(10, 214);
			this.statusStrip1.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(471, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 7;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// statusLabel
			// 
			this.statusLabel.AutoSize = false;
			this.statusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.RaisedInner;
			this.statusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(254, 17);
			this.statusLabel.Spring = true;
			this.statusLabel.Text = "Waiting...";
			// 
			// progressBar
			// 
			this.progressBar.AutoSize = false;
			this.progressBar.ForeColor = System.Drawing.Color.Gold;
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(200, 16);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			// 
			// CurrentListPanel
			// 
			this.CurrentListPanel.Controls.Add(this.CurrentListLabel);
			this.CurrentListPanel.Controls.Add(this.CurrentListTitle);
			this.CurrentListPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.CurrentListPanel.Location = new System.Drawing.Point(10, 10);
			this.CurrentListPanel.Name = "CurrentListPanel";
			this.CurrentListPanel.Size = new System.Drawing.Size(471, 43);
			this.CurrentListPanel.TabIndex = 0;
			// 
			// CurrentListLabel
			// 
			this.CurrentListLabel.AutoSize = true;
			this.CurrentListLabel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.CurrentListLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.CurrentListLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CurrentListLabel.ForeColor = System.Drawing.Color.Navy;
			this.CurrentListLabel.Location = new System.Drawing.Point(99, 0);
			this.CurrentListLabel.Name = "CurrentListLabel";
			this.CurrentListLabel.Padding = new System.Windows.Forms.Padding(5, 10, 0, 0);
			this.CurrentListLabel.Size = new System.Drawing.Size(5, 30);
			this.CurrentListLabel.TabIndex = 1;
			this.toolTip1.SetToolTip(this.CurrentListLabel, "Current list. Click to open the associated ini file.");
			this.CurrentListLabel.Click += new System.EventHandler(this.CurrentListLabel_Click);
			// 
			// CurrentListTitle
			// 
			this.CurrentListTitle.AutoSize = true;
			this.CurrentListTitle.Dock = System.Windows.Forms.DockStyle.Left;
			this.CurrentListTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CurrentListTitle.Location = new System.Drawing.Point(0, 0);
			this.CurrentListTitle.Name = "CurrentListTitle";
			this.CurrentListTitle.Padding = new System.Windows.Forms.Padding(10, 10, 0, 0);
			this.CurrentListTitle.Size = new System.Drawing.Size(99, 30);
			this.CurrentListTitle.TabIndex = 0;
			this.CurrentListTitle.Text = "Current list:";
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 100;
			// 
			// FiNoInfo
			// 
			this.FiNoInfo.AutoSize = true;
			this.FiNoInfo.Location = new System.Drawing.Point(7, 20);
			this.FiNoInfo.Name = "FiNoInfo";
			this.FiNoInfo.Size = new System.Drawing.Size(93, 17);
			this.FiNoInfo.TabIndex = 0;
			this.FiNoInfo.Text = "No information";
			this.toolTip1.SetToolTip(this.FiNoInfo, "List does not contain file size or media information.\r\nNot recommended. \r\nAlso, n" +
        "ot faster than the size option.");
			this.FiNoInfo.UseVisualStyleBackColor = true;
			// 
			// FiSize
			// 
			this.FiSize.AutoSize = true;
			this.FiSize.Checked = true;
			this.FiSize.Location = new System.Drawing.Point(7, 43);
			this.FiSize.Name = "FiSize";
			this.FiSize.Size = new System.Drawing.Size(97, 17);
			this.FiSize.TabIndex = 1;
			this.FiSize.TabStop = true;
			this.FiSize.Text = "Include file size";
			this.toolTip1.SetToolTip(this.FiSize, "List includes file sizes. \r\nRecommended option for all non-media lists.");
			this.FiSize.UseVisualStyleBackColor = true;
			// 
			// FiFull
			// 
			this.FiFull.AutoSize = true;
			this.FiFull.Location = new System.Drawing.Point(7, 66);
			this.FiFull.Name = "FiFull";
			this.FiFull.Size = new System.Drawing.Size(111, 17);
			this.FiFull.TabIndex = 2;
			this.FiFull.Text = "Include media info";
			this.toolTip1.SetToolTip(this.FiFull, "List includes file sizes and media info for video and audio files\r\nCan be slow. \r" +
        "\nRecommended for all lists that include audio and video files.");
			this.FiFull.UseVisualStyleBackColor = true;
			// 
			// PartialFolderRadio
			// 
			this.PartialFolderRadio.AutoSize = true;
			this.PartialFolderRadio.Checked = true;
			this.PartialFolderRadio.Location = new System.Drawing.Point(14, 43);
			this.PartialFolderRadio.Name = "PartialFolderRadio";
			this.PartialFolderRadio.Size = new System.Drawing.Size(88, 17);
			this.PartialFolderRadio.TabIndex = 1;
			this.PartialFolderRadio.TabStop = true;
			this.PartialFolderRadio.Text = "Partial folders";
			this.toolTip1.SetToolTip(this.PartialFolderRadio, "Partial folder information. Recommended for privacy.\r\nThis option strips out inpu" +
        "t folder from the folder name:\r\nE.g. \"c:\\Users\\Scott\\Files\\Books\\King, Stephen\" " +
        "becomes just \"King, Stephen\"");
			this.PartialFolderRadio.UseVisualStyleBackColor = true;
			// 
			// IncludeFoldersRadio
			// 
			this.IncludeFoldersRadio.AutoSize = true;
			this.IncludeFoldersRadio.Location = new System.Drawing.Point(14, 66);
			this.IncludeFoldersRadio.Name = "IncludeFoldersRadio";
			this.IncludeFoldersRadio.Size = new System.Drawing.Size(90, 17);
			this.IncludeFoldersRadio.TabIndex = 2;
			this.IncludeFoldersRadio.TabStop = true;
			this.IncludeFoldersRadio.Text = "Full folder info";
			this.toolTip1.SetToolTip(this.IncludeFoldersRadio, "Include folder information to the list.\r\nNote that this can give away private inf" +
        "ormation, e.g.\r\n\"c:\\Users\\Scott\\Files\\Books\". \r\nUse Partial folders to avoid tha" +
        "t.");
			this.IncludeFoldersRadio.UseVisualStyleBackColor = true;
			// 
			// DeleteMediaInfo
			// 
			this.DeleteMediaInfo.AutoSize = true;
			this.DeleteMediaInfo.Location = new System.Drawing.Point(19, 89);
			this.DeleteMediaInfo.Name = "DeleteMediaInfo";
			this.DeleteMediaInfo.Size = new System.Drawing.Size(141, 17);
			this.DeleteMediaInfo.TabIndex = 3;
			this.DeleteMediaInfo.Text = "Delete media info cache";
			this.toolTip1.SetToolTip(this.DeleteMediaInfo, resources.GetString("DeleteMediaInfo.ToolTip"));
			this.DeleteMediaInfo.UseVisualStyleBackColor = true;
			// 
			// NoFolderRadio
			// 
			this.NoFolderRadio.AutoSize = true;
			this.NoFolderRadio.Location = new System.Drawing.Point(14, 20);
			this.NoFolderRadio.Name = "NoFolderRadio";
			this.NoFolderRadio.Size = new System.Drawing.Size(122, 17);
			this.NoFolderRadio.TabIndex = 0;
			this.NoFolderRadio.Text = "No folder information";
			this.toolTip1.SetToolTip(this.NoFolderRadio, "Flat list with no folder information included.\r\nNot recommended.");
			this.NoFolderRadio.UseVisualStyleBackColor = true;
			// 
			// ProjectLabel
			// 
			this.ProjectLabel.AutoSize = true;
			this.ProjectLabel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.ProjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ProjectLabel.ForeColor = System.Drawing.Color.Navy;
			this.ProjectLabel.Location = new System.Drawing.Point(348, 73);
			this.ProjectLabel.Name = "ProjectLabel";
			this.ProjectLabel.Size = new System.Drawing.Size(102, 13);
			this.ProjectLabel.TabIndex = 5;
			this.ProjectLabel.Text = "QuickList on GitHub";
			this.toolTip1.SetToolTip(this.ProjectLabel, "Project home page with source and help.");
			this.ProjectLabel.Click += new System.EventHandler(this.ProjectLabel_Click);
			// 
			// ReportError
			// 
			this.ReportError.AutoSize = true;
			this.ReportError.Cursor = System.Windows.Forms.Cursors.Hand;
			this.ReportError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportError.ForeColor = System.Drawing.Color.Navy;
			this.ReportError.Location = new System.Drawing.Point(348, 98);
			this.ReportError.Name = "ReportError";
			this.ReportError.Size = new System.Drawing.Size(118, 13);
			this.ReportError.TabIndex = 6;
			this.ReportError.Text = "Problems && suggestions";
			this.toolTip1.SetToolTip(this.ReportError, "Report a problem or suggestion. \r\nNo registration needed.");
			this.ReportError.Click += new System.EventHandler(this.ReportError_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.DeleteMediaInfo);
			this.groupBox1.Controls.Add(this.FiFull);
			this.groupBox1.Controls.Add(this.FiSize);
			this.groupBox1.Controls.Add(this.FiNoInfo);
			this.groupBox1.Location = new System.Drawing.Point(10, 53);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(160, 126);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "File information";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.IncludeFoldersRadio);
			this.groupBox2.Controls.Add(this.PartialFolderRadio);
			this.groupBox2.Controls.Add(this.NoFolderRadio);
			this.groupBox2.Location = new System.Drawing.Point(170, 53);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(165, 126);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "List options";
			// 
			// GenerateListButton
			// 
			this.GenerateListButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.GenerateListButton.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.GenerateListButton.Location = new System.Drawing.Point(10, 185);
			this.GenerateListButton.Name = "GenerateListButton";
			this.GenerateListButton.Size = new System.Drawing.Size(471, 29);
			this.GenerateListButton.TabIndex = 8;
			this.GenerateListButton.Text = "Generate list";
			this.GenerateListButton.UseVisualStyleBackColor = true;
			this.GenerateListButton.Click += new System.EventHandler(this.GenerateListButton_Click);
			// 
			// callbackTimer
			// 
			this.callbackTimer.Interval = 250;
			this.callbackTimer.Tick += new System.EventHandler(this.callbackTimer_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(491, 246);
			this.Controls.Add(this.GenerateListButton);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.ReportError);
			this.Controls.Add(this.ProjectLabel);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.CurrentListPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Padding = new System.Windows.Forms.Padding(10);
			this.Text = "QuickList";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.CurrentListPanel.ResumeLayout(false);
			this.CurrentListPanel.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private Panel CurrentListPanel;
        private Label CurrentListTitle;
        private Label CurrentListLabel;
        private ToolTip toolTip1;
        private GroupBox groupBox1;
        private RadioButton FiFull;
        private RadioButton FiSize;
        private RadioButton FiNoInfo;
        private GroupBox groupBox2;
        private RadioButton NoFolderRadio;
        private RadioButton PartialFolderRadio;
        private RadioButton IncludeFoldersRadio;
        private CheckBox DeleteMediaInfo;
        private Label ProjectLabel;
        private Label ReportError;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar progressBar;
        private Button GenerateListButton;
		private Timer callbackTimer;
	}
}
