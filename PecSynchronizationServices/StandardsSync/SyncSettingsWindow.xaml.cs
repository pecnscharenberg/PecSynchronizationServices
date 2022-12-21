using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PecSynchronizationServices.StandardsSync
{
    /// <summary>
    /// Interaction logic for SyncSettingsWindow.xaml
    /// </summary>
    public partial class SyncSettingsWindow : Window
    {
        private SyncSettingsViewModel ViewModel => DataContext as SyncSettingsViewModel;

        public SyncSettingsWindow()
        {
            InitializeComponent();

            DataContext = new SyncSettingsViewModel();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectAllSynchronizationItems();
        }

        private void UnselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeselectAllSynchronizationItems();
        }
    }
}
