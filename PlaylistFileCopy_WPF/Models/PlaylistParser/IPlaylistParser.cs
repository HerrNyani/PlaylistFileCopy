using System.IO;

namespace HerrNyani.PlaylistFileCopy_WPF.Models.PlaylistParser
{
    public interface IPlaylistParser
    {
        PlaylistFile Parse(Stream playlistStream);
    }
}
