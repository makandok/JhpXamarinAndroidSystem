using JhpDataSystem;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //AppInstance.Instance.InitialiseAppResources
            //var keys = Resources.Keys;


            AppInstance.Instance.InitialiseAppResources(null,null);
            //we have loaded the app
            this.menuServerSync.Click += MenuServerSync_Click;
            this.menuConfigure.Click += MenuConfigure_Click;
            this.menuAllData.Click += MenuAllData_Click;
            this.menuSmmaries.Click += MenuSmmaries_Click;
        }

        private void MenuSmmaries_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu item clicked ");
        }

        private void MenuAllData_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu item clicked ");
        }

        private void MenuConfigure_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Menu item clicked ");
        }

        private async void MenuServerSync_Click(object sender, RoutedEventArgs e)
        {
            var res = await AppInstance.Instance.CloudDbInstance.doServerSync(null);

            var fixMe = "Ended here"
            //we test the connection
            var isConnected = false;
            //var isConnected = await new TestServerConnection().BeginTest();
            MessageBox.Show("Connection Status: " + isConnected);

            //we get list of files to download

            //for each file, we download

            //we decrypt

            //we deidentify

            //and save to the main datastore
            MessageBox.Show("Menu item clicked ");
        }
    }
}
