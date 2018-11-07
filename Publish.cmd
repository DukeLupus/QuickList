
mkdir Publish
del /q Publish\*
ilmerge QuickList\bin\Release\QuickList.exe QuickList\bin\Release\Microsoft.WindowsAPICodePack.dll QuickList\bin\Release\Microsoft.WindowsAPICodePack.Shell.dll /out:Publish\QuickList.exe /ndebug /internalize /v4