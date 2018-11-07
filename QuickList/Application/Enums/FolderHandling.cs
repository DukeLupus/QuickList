namespace Sander.QuickList.Application.Enums
{
	internal enum FolderHandling
	{
		/// <summary>
		/// Flat file list
		/// </summary>
		NoFolders = 0,
		/// <summary>
		/// Whether to strip initial input folder information from the list.
		/// E.g. if the input folder is "D:\Arthur\My Files\Books\", then "D:\Arthur\My Files\Books\William Shakespeare\" becomes "William Shakespeare" in the list
		/// </summary>
		PartialFolders = 1,
		/// <summary>
		/// Full folder information
		/// </summary>
		IncludeFolders = 2
	}
}
