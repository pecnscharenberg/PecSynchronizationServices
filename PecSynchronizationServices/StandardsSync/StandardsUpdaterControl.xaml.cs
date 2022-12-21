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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PecSynchronizationServices.StandardsSync
{
    /// <summary>
    /// Interaction logic for StandardsUpdaterControl.xaml
    /// </summary>
    public partial class StandardsUpdaterControl : UserControl
    {
        public delegate void UpdateButtonClicked();

        public event UpdateButtonClicked OnUpdateButtonClicked;

        public StandardsUpdaterControl()
        {
            InitializeComponent();

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

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            OnUpdateButtonClicked?.Invoke();
        }
    }
}
