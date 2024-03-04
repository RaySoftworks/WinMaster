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
using System.Security.Principal;


namespace Typhoon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetUsername();
            DragButtonLow.IsHitTestVisible = false;
            Ico.IsHitTestVisible = false;
            Credits.Visibility = Visibility.Hidden;
        }

        private void SetUsername()
        {
            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
            if (currentUser != null)
            {
                string username = currentUser.Name;
                if (username.Contains("\\"))
                {
                    username = username.Split('\\')[1];
                }

                usernameLabel.Content = "User: " + username;
            }
            else
            {
                usernameLabel.Content = "Can't Show UserName.";
            }
        }

        private void TopBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Image_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Credits.Visibility == Visibility.Hidden)
            {
                Credits.Visibility = Visibility.Visible;
            }
            else
            {
                Credits.Visibility = Visibility.Hidden;

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Credits.Visibility = Visibility.Hidden;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Window2 second_window = new Window2();
            second_window.Show();
            Close();

        }
    }
}
