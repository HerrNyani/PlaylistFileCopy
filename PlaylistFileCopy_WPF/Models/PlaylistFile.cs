using Assisticant.Collections;
using Assisticant.Fields;
using System;
using System.Collections.Generic;

namespace HerrNyani.PlaylistFileCopy_WPF.Models
{
    public class PlaylistFile
    {
        private readonly Observable<string> _absoluteFilePath = new Observable<string>(String.Empty);
        private readonly ObservableList<MusicFile> _musicFileList = new ObservableList<MusicFile>();

        public string AbsoluteFilePath
        {
            get { return _absoluteFilePath.Value; }
            set { _absoluteFilePath.Value = value; }
        }

        public ICollection<MusicFile> MusicFileCollection => _musicFileList;
    }
}
