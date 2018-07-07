using Assisticant.Fields;
using HerrNyani.PlaylistFileCopy_WPF.Models;
using HerrNyani.PlaylistFileCopy_WPF.Models.PlaylistParser;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WinForms = System.Windows.Forms;

namespace HerrNyani.PlaylistFileCopy_WPF.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly Observable<PlaylistFile> _loadedPlaylistFile = new Observable<PlaylistFile>(new PlaylistFile());
        private readonly Observable<string> _destinationFolderPath = new Observable<string>(String.Empty);
        private readonly BackgroundWorker _copyMusicFileWorker = new BackgroundWorker();
        private readonly Observable<bool> _isCopying = new Observable<bool>(false);
        private readonly Observable<string> _copyStatusMessage = new Observable<string>(String.Empty);
        private readonly Observable<int> _copyProgressPercentage = new Observable<int>();

        public MainWindowViewModel()
        {
            _copyMusicFileWorker.WorkerReportsProgress = true;
            _copyMusicFileWorker.DoWork += CopyMusicFileWorker_DoWork;
            _copyMusicFileWorker.ProgressChanged += CopyMusicFileWorker_ProgressChanged;
            _copyMusicFileWorker.RunWorkerCompleted += CopyMusicFileWorker_RunWorkerCompleted;

            UpdateStatusBar("Ready.", 0);
        }

        public string PlaylistFileAbsolutePath
        {
            get { return _loadedPlaylistFile.Value.AbsoluteFilePath; }
            set { _loadedPlaylistFile.Value.AbsoluteFilePath = value ?? String.Empty; }
        }

        public IEnumerable<MusicFileItemViewModel> MusicFiles => _loadedPlaylistFile.Value.MusicFileCollection
            .OrderBy(f => f.RelativePath)
            .Select(f => new MusicFileItemViewModel(f));

        public string DestinationFolderAbsolutePath
        {
            get { return _destinationFolderPath.Value; }
            set { _destinationFolderPath.Value = value ?? String.Empty; }
        }

        public string CopyStatusMessage => _copyStatusMessage.Value;

        public int CopyStatusProgressPercentage => _copyProgressPercentage.Value;

        public bool CanBrowseForPlaylistFile => !_isCopying.Value;

        public void BrowseForPlaylistFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Windows Media Player playlist|*.wpl"
            };

            bool isConfirmed = ofd.ShowDialog() ?? false;
            if (isConfirmed)
            {
                using (FileStream fileStream = new FileStream(ofd.FileName, FileMode.Open))
                {
                    IPlaylistParser playlistParser = new WindowsMediaPlayerPlaylistParser();
                    _loadedPlaylistFile.Value = playlistParser.Parse(fileStream);
                    _loadedPlaylistFile.Value.AbsoluteFilePath = ofd.FileName;
                }
            }
        }
        
        public bool CanBrowseForDestinationFolder => !_isCopying.Value;

        public void BrowseForDestinationFolder()
        {
            WinForms.FolderBrowserDialog fbd = new WinForms.FolderBrowserDialog
            {
                Description = "Select output folder."
            };

            WinForms.DialogResult folderBrowseResult = fbd.ShowDialog();
            if (folderBrowseResult.Equals(WinForms.DialogResult.OK))
            {
                _destinationFolderPath.Value = fbd.SelectedPath;
            }
        }

        public bool CanStartMusicFileCopy => _loadedPlaylistFile.Value.MusicFileCollection.Any()
            && !String.IsNullOrWhiteSpace(_destinationFolderPath.Value)
            && !_isCopying.Value;

        public void StartMusicFileCopy()
        {
            Debug.Assert(CanStartMusicFileCopy, $"{nameof(StartMusicFileCopy)} called while {nameof(CanStartMusicFileCopy)} returned false. Check bindings.");
            if(!CanStartMusicFileCopy)
            {
                return;
            }

            _isCopying.Value = true;

            UpdateStatusBar("Copying files...", 0);
            _copyMusicFileWorker.RunWorkerAsync(_loadedPlaylistFile.Value.MusicFileCollection);
        }
        
        private void CopyMusicFileWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker senderBackgroundWorker = (BackgroundWorker)sender;

            IEnumerable<MusicFile> musicFiles = e.Argument as IEnumerable<MusicFile>;
            if(musicFiles == null)
            {
                return;
            }

            int currentIndex = 0;
            foreach (MusicFile item in _loadedPlaylistFile.Value.MusicFileCollection)
            {
                string combinedInputFilePath = Path.Combine(Path.GetDirectoryName(_loadedPlaylistFile.Value.AbsoluteFilePath), item.RelativePath);
                string inputFilePath = Path.GetFullPath((new Uri(combinedInputFilePath)).LocalPath);
                
                string combinedOutputFilePath = Path.Combine(_destinationFolderPath.Value, item.RelativePath);
                string outputFilePath = Path.GetFullPath((new Uri(combinedOutputFilePath)).LocalPath);
                
                FileInfo copyFileInfo = new FileInfo(outputFilePath);
                if (copyFileInfo.Exists)
                {
                    continue;
                }

                copyFileInfo.Directory.Create();

                File.Copy(inputFilePath, outputFilePath);

                // TODO Feature: Copy album art

                // Report back to UI
                ++currentIndex;
                double completedFraction = (double)currentIndex / musicFiles.Count();
                senderBackgroundWorker.ReportProgress((int)Math.Round(completedFraction * 100.0));
                
                if(senderBackgroundWorker.CancellationPending)
                {
                    break;
                }
            }
        }

        private void CopyMusicFileWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateStatusBar("Copying files...", e.ProgressPercentage);
        }

        private void CopyMusicFileWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _isCopying.Value = false;
            UpdateStatusBar("Copy completed.", 100);
        }

        private void UpdateStatusBar(string message, int progressPercentage)
        {
            _copyStatusMessage.Value = message;
            _copyProgressPercentage.Value = progressPercentage;
        }
    }
}
