using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for BarrowedBooksW.xaml
    /// </summary>
    public partial class BarrowedBooksW : Window
    {
        public BarrowedBooksW()
        {
            InitializeComponent();
            dtgBooks.ItemsSource = PublicMethods.BarrowedBloader(UserCons.LoggedInUser.StudentNo);


            lbBarrowedBooks.Content = $"Borrowed Books count \"{UserCons.LoggedInUser.BorrowedBooks.ToString()}\"";
            if (UserCons.LoggedInUser.UserRank == 1)
            {
                lbReturnPeriod.Content = $"Return period\"{UserCons.LoggedInUser.BookReturnPeriod.ToString()}\" days";
                lbAllowedBooks.Content = $"Allowed Books count \"{UserCons.LoggedInUser.UserAllowedBooks.ToString()}\"";
            }
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            string query = "";

            // Check if an item is selected in the data grid
            if (dtgBooks.SelectedItem == null)
            {
                return;
            }

            // Get the selected item from the data grid
            DataRowView vrItem = (DataRowView)dtgBooks.SelectedItem;

            // Show a confirmation message box before returning the book
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to return the book \"{vrItem.Row["BookName"]}\" that has the following ID \"{vrItem.Row["BookId"]}\"?", "Returning Confirmation", System.Windows.MessageBoxButton.YesNo);

            // Check if the user confirmed the return
            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Update the book state to 1, indicating it is returned but not yet approved
            query = @$"UPDATE BorrowedBookstbl SET BookState = '1' WHERE BookId = '{vrItem.Row["BookId"]}'";

            // Execute the query and check the result
            int count1 = dbConnection.updateDeleteInsert(query);

            if (count1 == 0)
            {
                MessageBox.Show("Something went wrong");
                return;
            }

            // Refresh the data grid with the updated list of borrowed books
            dtgBooks.ItemsSource = PublicMethods.BarrowedBloader(UserCons.LoggedInUser.StudentNo);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Close the current window
            Window.GetWindow(this).Close();
        }
    }
}
