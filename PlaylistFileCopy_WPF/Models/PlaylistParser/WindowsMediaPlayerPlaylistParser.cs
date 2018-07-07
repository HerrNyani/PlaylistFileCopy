using System.Diagnostics;
using System.IO;
using System.Xml;

namespace HerrNyani.PlaylistFileCopy_WPF.Models.PlaylistParser
{
    public class WindowsMediaPlayerPlaylistParser :
        IPlaylistParser
    {
        public PlaylistFile Parse(Stream playlistStream)
        {
            PlaylistFile playlist = new PlaylistFile();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(playlistStream);

            FillMusicFileCollection(xmlDocument, playlist);

            return playlist;
        }

        private static void FillMusicFileCollection(XmlDocument xmlDocument, PlaylistFile playlist)
        {
            XmlNodeList mediaNodeList = xmlDocument.DocumentElement.SelectNodes("/smil/body/seq/media");

            foreach (XmlNode mediaNode in mediaNodeList)
            {
                MusicFile musicFile = new MusicFile()
                {
                    RelativePath = mediaNode.Attributes["src"].Value
                };

                playlist.MusicFileCollection.Add(musicFile);
            }

            Trace.TraceInformation($"Playlist contains {playlist.MusicFileCollection.Count} entries.");
        }
    }
}
