using Assisticant;
using HerrNyani.PlaylistFileCopy_WPF.ViewModels;
using System.Windows;

namespace HerrNyani.PlaylistFileCopy_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ForView.Wrap(new MainWindowViewModel());
        }
    }
}
