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
    /// Interaction logic for BarrowedBooksAdmin.xaml
    /// </summary>
    public partial class BarrowedBooksAdmin : Window
    {
        public BarrowedBooksAdmin()
        {
            InitializeComponent();
            loader();
        }

        private void loader()
        {
            // Query to retrieve the borrowed books data for admin approval
            string query = "SELECT BookName,Writer,BorrowingDate,Bookstbl.BookId,BorrowedBookstbl.StudentNo,Userstbl.FullName,BorrowedBookstbl.OprationId FROM Bookstbl INNER JOIN BorrowedBookstbl ON Bookstbl.BookId = BorrowedBookstbl.BookId INNER JOIN Userstbl on BorrowedBookstbl.StudentNo = Userstbl.StudentNo  where AdminConfirmationState=0 and BookState=1";

            // Retrieve the borrowed books data
            DataTable dtBbooks = dbConnection.selectTable(query);

            // Create a data view from the retrieved data
            DataView dv = new DataView(dtBbooks);

            // Set the item source of the data grid to the data view
            dtgBooks.ItemsSource = dv;
        }

        private void btnDisapproveBook_Click(object sender, RoutedEventArgs e)
        {
            if (dtgBooks.SelectedItem == null)
            {
                return;
            }

            // Get the selected item from the data grid
            DataRowView vrItem = (DataRowView)dtgBooks.SelectedItem;

            // Show a confirmation message box before disapproving the book
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to disapprove the book \"{vrItem.Row["BookName"]}\"?", "Approving Confirmation", System.Windows.MessageBoxButton.YesNo);

            // Check if the user confirmed the disapproval
            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Update the book state to 0, indicating it is not returned
            string query = @$" update BorrowedBookstbl set BookState='0' where OprationId='{vrItem.Row["OprationId"]}'";

            // Execute the query and check the result
            int count = dbConnection.updateDeleteInsert(query);

            // Check if the update was successful
            if (count == 0)
            {
                MessageBox.Show("Something went wrong");
                return;
            }

            // Show a success message and refresh the data grid
            MessageBox.Show($"You disapproved the book with the following ID \"{vrItem.Row["BookId"]}\"");
            loader();
        }

        private void btnApproveBook_Click(object sender, RoutedEventArgs e)
        {
            if (dtgBooks.SelectedItem == null)
            {
                return;
            }

            // Get the selected item from the data grid
            DataRowView vrItem = (DataRowView)dtgBooks.SelectedItem;

            // Show a confirmation message box before approving the book
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to approve the book \"{vrItem.Row["BookName"]}\"?", "Approving Confirmation", System.Windows.MessageBoxButton.YesNo);

            // Check if the user confirmed the approval
            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Update the admin confirmation state to 1, indicating the book is approved
            string query = @$" update BorrowedBookstbl set AdminConfirmationState='1' where OprationId='{vrItem.Row["OprationId"]}'";

            // Execute the query and check the result
            int count = dbConnection.updateDeleteInsert(query);

            // Update the book quantity by incrementing it by 1
            query = @$" update Bookstbl set Quintity= Quintity + 1 where BookId='{vrItem.Row["BookId"]}'";

            // Execute the query and check the result
            int count1 = dbConnection.updateDeleteInsert(query);

            // Update the user's borrowed books count by decrementing it by 1
            query = $@" update Userstbl set BorrowedBooksCount = BorrowedBooksCount - 1 where StudentNo='{vrItem.Row["StudentNo"]}' ";

            // Execute the query and check the result
            int count2 = dbConnection.updateDeleteInsert(query);

            // Check if all updates were successful
            if (count == 0 || count1 == 0 || count2 == 0)
            {
                MessageBox.Show("Something went wrong");
                return;
            }

            // Show a success message and refresh the data grid
            MessageBox.Show($"You approved the book with the following ID \"{vrItem.Row["BookId"]}\"");
            loader();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            // Disable the approve and disapprove buttons
            btnApproveBook.IsEnabled = false;
            btnDisapproveBook.IsEnabled = false;

            // Query to retrieve all borrowed books data
            string query = "SELECT BookName,Writer,BorrowingDate,Bookstbl.BookId,BorrowedBookstbl.StudentNo,Userstbl.FullName,BorrowedBookstbl.OprationId FROM Bookstbl INNER JOIN BorrowedBookstbl ON Bookstbl.BookId = BorrowedBookstbl.BookId INNER JOIN Userstbl on BorrowedBookstbl.StudentNo = Userstbl.StudentNo";

            // Retrieve all borrowed books data
            DataTable dtBbooks = dbConnection.selectTable(query);

            // Create a data view from the retrieved data
            DataView dv = new DataView(dtBbooks);

            // Set the item source of the data grid to the data view
            dtgBooks.ItemsSource = dv;

            // Disable the approve button
            btnApproveBook.IsEnabled = false;
            // Disable the disapprove button
            btnDisapproveBook.IsEnabled = false;
        }

        private void checkBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            // Refresh the data grid with the approved and not returned borrowed books
            loader();
            // Enable the approve button
            btnApproveBook.IsEnabled = true;
            // Enable the disapprove button
            btnDisapproveBook.IsEnabled = true;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Close the current window
            Window.GetWindow(this).Close();
        }
    }
}
