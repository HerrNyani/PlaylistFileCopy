using HerrNyani.PlaylistFileCopy_WPF.Models;

namespace HerrNyani.PlaylistFileCopy_WPF.ViewModels
{
    public class MusicFileItemViewModel
    {
        private readonly MusicFile _model;

        public MusicFileItemViewModel(MusicFile model)
        {
            _model = model;
        }

        public string RelativePath => _model.RelativePath;
    }
}
