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

namespace Library_project
{
    /// <summary>
    /// Interaction logic for AdminW.xaml
    /// </summary>
    public partial class AdminW : Window
    {
        public AdminW()
        {
            InitializeComponent();
        }

        // Event handler for the Users Need Approval button click event
        private void btnUsersNeedApproval_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the UsersApprovalRequests window
            UsersApprovalRequests BarrowedBooksW = new UsersApprovalRequests();

            // Show the UsersApprovalRequests window
            BarrowedBooksW.Show();
        }

        // Event handler for the Borrowed Books button click event
        private void btBorrowedBooks_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the BarrowedBooksAdmin window
            BarrowedBooksAdmin window = new BarrowedBooksAdmin();

            // Show the BarrowedBooksAdmin window
            window.Show();
        }

        // Event handler for the All Users button click event
        private void AbtnllUseres_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the UsersAdminPanel window
            UsersAdminPanel window = new UsersAdminPanel();

            // Show the UsersAdminPanel window
            window.Show();
        }

        // Event handler for the All Books button click event
        private void btnAllBooks_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the AllBooksW window
            AllBooksW window = new AllBooksW();

            // Show the AllBooksW window
            window.Show();
        }

        // Event handler for the Log Out button click event
        private void logOutBtn_Click(object sender, RoutedEventArgs e)
        {
            // Reset the logged-in user
            UserCons.ResetLoggedInUser();

            // Create an instance of the LogInW window
            LogInW loginnWindow = new LogInW();

            // Show the LogInW window
            loginnWindow.Show();

            // Close the current window
            Window.GetWindow(this).Close();
        }
    }
}
