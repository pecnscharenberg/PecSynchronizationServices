using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// Interaction logic for UpdateStandardsWindow.xaml
    /// </summary>
    public partial class UpdateStandardsWindow : Window, IDisposable
    {
        private UpdateStandardsViewModel ViewModel { get; set; }

        public UpdateStandardsWindow()
        {
            InitializeComponent();

            DataContext = ViewModel = new UpdateStandardsViewModel(this.Dispatcher);

            (MessagesListView.Items.SourceCollection as INotifyCollectionChanged).CollectionChanged += Messages_CollectionChanged;
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(MessagesListView) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(MessagesListView, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.StartUpdate();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Dispose()
        {
            ViewModel?.Dispose();
            ViewModel = null;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SyncSettingsWindow()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            settingsWindow.ShowDialog();
        }
    }
}
