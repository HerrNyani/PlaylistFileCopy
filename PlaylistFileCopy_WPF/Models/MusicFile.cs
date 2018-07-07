using Assisticant.Fields;

namespace HerrNyani.PlaylistFileCopy_WPF.Models
{
    public class MusicFile
    {
        private Observable<string> _absolutePath = new Observable<string>(string.Empty);

        public string RelativePath
        {
            get { return _absolutePath.Value; }
            set { _absolutePath.Value = value; }
        }

    }
}
