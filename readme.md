### Installation

1. Open OmenServe\ListMagic folder - usually &lt;mirc folder&gt;\OmenServe\ListMagic
2. [Optional] Rename existing QuickList.exe to something else, in case you want to revert to v2
3. Put new, v3 QuickList.exe to  OmenServe\ListMagic folder
4. [Optional] Double-click QuickList.exe, to make sure everything works. You should see message "Commandline parameter pointing to ini file is missing!", this is expected
5. Make and manage lists in OmenServe same as always


### Changes & new features
* Optional partial folders in the public list, reducing chances of leaking personal info. This strips the initial input portion of the folder from the list.  
	E.g. if the list input folder is "c:\Users\Scott\Files\Books\" then instead of including folder name "c:\Users\Scott\Files\Books\William Shakespeare\Plays", public list only includes "William Shakespeare\Plays" as a folder name.
* Much, much faster list generation, especially for very large non-media lists
* Much faster list generation with the cached media info
* More accurate matching of the cached vs actual media file information
* Support for more media types: aa, aac, aax, aif, aiff, ape, asf, avi, divx, dsf, fla, flac, m2a, m2v, m4a, m4b, m4p, m4v, mka, mks, mkv, mp+, mp1, mp2, mp3, mp4, mpc, mpe, mpeg, mpg, mpp, mpv2, oga, ogg, ogv, opus, wav, webm, wma, wmv, wv.
* Supports additional media types with shell integration (shell aka Windows Explorer must be capable of showing the media duration and additional info. This may require an installation of specific codecs/codec packs): g2, 3gp, act, alax, amd, awb, dct, dss, dvf, f4a, f4b, f4v, flv, m4a, mov, mts, mts2, qt, vob






### Used components

* Code from [taglib#](https://github.com/mono/taglib-sharp), licensed under [GNU Lesser General Public License v2.1](https://github.com/mono/taglib-sharp/blob/master/COPYING)
* [Windows-API-Code-Pack-1.1](https://github.com/aybe/Windows-API-Code-Pack-1.1), licensed under [custom licence](https://github.com/aybe/Windows-API-Code-Pack-1.1/blob/master/LICENCE)
* Not used directly, but to combine multiple assemblies for public releases, [ILMerge](https://github.com/dotnet/ILMerge)