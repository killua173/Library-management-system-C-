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
    /// Interaction logic for UsersW.xaml
    /// </summary>
    public partial class UsersW : Window
    {
        public UsersW()
        {
            InitializeComponent();
            lbname.Content = $"Hello {UserCons.LoggedInUser.UserName.ToString()}";

            lbBarrowedBooks.Content = $"Borrowed Books count \"{UserCons.LoggedInUser.BorrowedBooks.ToString()}\"";
            if (UserCons.LoggedInUser.UserRank == 1)
            {
                lbReturnPeriod.Content = $"Return period\"{UserCons.LoggedInUser.BookReturnPeriod.ToString()}\" days";
                lbAllowedBooks.Content = $"Allowed Books count \"{UserCons.LoggedInUser.UserAllowedBooks.ToString()}\"";
            }
        }

        // Variable declarations
        private int notGettingstuck;
        int irPageSize = 35;
        int irCountOfPages;
        int irPrevPageNumber, irNextPageNumber;

        // Event handler for the "Borrow" button click
        private void btnBarrow_Click(object sender, RoutedEventArgs e)
        {
            // Check if a book is selected
            if (dtgBooks.SelectedItem == null)
            {
                return;
            }

            // Check if the user has reached their book limit
            if (UserCons.LoggedInUser.UserAllowedBooks <= UserCons.LoggedInUser.BorrowedBooks)
            {
                MessageBox.Show("You have reached your book limit.");
                return;
            }

            // Get the selected book's information
            DataRowView vrItem = (DataRowView)dtgBooks.SelectedItem;

            // Check if the selected book is out of stock
            if (vrItem.Row["Quintity"].ToString() == "0")
            {
                MessageBox.Show("The book is out of stock");
                return;
            }

            // Confirmation dialog for borrowing the book
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to borrow the book \"{vrItem.Row["BookName"]}\" that is written by \"{vrItem.Row["Writer"]}\"?", "Borrowing Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Get the current date and time
            DateTime rndate = DateTime.Now;

            // Insert a new row in the BorrowedBookstbl table
            string query = $@"Insert into BorrowedBookstbl Values ('{UserCons.LoggedInUser.StudentNo}','{vrItem.Row["BookId"]}','0','0','{rndate.ToString("yyyy-MM-dd HH:mm:ss.000")}')";

            int result1 = dbConnection.updateDeleteInsert(query);

            // Update the book's quantity
            query = $@" update Bookstbl set Quintity = Quintity - 1 where BookId='{vrItem.Row["BookId"]}' ";
            int result2 = dbConnection.updateDeleteInsert(query);

            // Check if the database operations were successful
            if (result1 == 0 || result2 == 0)
            {
                MessageBox.Show("Something went wrong");
                return;
            }

            // Update the user's borrowed books count
            if (UserCons.LoggedInUser.UserRank == 1)
            {
                query = $@" update Userstbl set BorrowedBooksCount = BorrowedBooksCount + 1 where StudentNo='{UserCons.LoggedInUser.StudentNo}' ";
                int result3 = dbConnection.updateDeleteInsert(query);
                if (result3 == 0)
                {
                    MessageBox.Show("Something went wrong");
                    return;
                }
                UserCons.LoggedInUser.BorrowedBooks = UserCons.LoggedInUser.BorrowedBooks + 1;
            }



            lbBarrowedBooks.Content = $"Borrowed Books count \"{UserCons.LoggedInUser.BorrowedBooks.ToString()}\"";
            // Refresh the data grid
            refreshDataGrid();

            MessageBox.Show("You borrowed a book");
        }

        // Event handler for the text changed event in the search box
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            refreshDataGrid();
        }

        // Event handler for the "Next" button click
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            cbmPages.SelectedIndex = irNextPageNumber - 1;
            refreshDataGrid();
        }

        // Event handler for the "Previous" button click
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            cbmPages.SelectedIndex = irPrevPageNumber - 1;
            refreshDataGrid();
        }

        // Event handler for the selection changed event in the data grid
        private void dtgBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (notGettingstuck == 1)
            {
                notGettingstuck = 0;
                return;
            }
            refreshDataGrid();
        }

        // Event handler for the data grid loaded event
        private void dtgBooks_Loaded(object sender, RoutedEventArgs e)
        {
            refreshDataGrid();
            notGettingstuck = 0;
        }

        // Event handler for the "Borrowed Books" button click
        private void btnBarrowedBooks_Click(object sender, RoutedEventArgs e)
        {
            BarrowedBooksW BarrowedBooksWindow = new BarrowedBooksW();
            BarrowedBooksWindow.Show();
        }

        // Event handler for the "Log Out" button click
        private void logOutBtn_Click(object sender, RoutedEventArgs e)
        {
            // Reset the logged in user information
            UserCons.ResetLoggedInUser();

            // Open the login window
            LogInW loginnWindow = new LogInW();
            loginnWindow.Show();

            // Close the current window
            Window.GetWindow(this).Close();
        }

        // Refreshes the data grid with updated data based on the search text and current page
        private void refreshDataGrid()
        {
            List<string> lsNames = new List<string> { "@Text" };
            List<object> lsValues = new List<object> { "%" + txtSearch.Text + "%" };
            int irSelectedIndex = cbmPages.SelectedIndex;

            // Check if the search text is a single character
            if (txtSearch.Text.Length == 1)
            {
                lsValues.Clear();
                lsValues.Add(txtSearch.Text + "%");
            }

            string srQuery = "select COUNT (*) from Bookstbl where BookName like @Text or BookId like @Text ";
            int irRecordcount = dbConnection.cmd_Count(srQuery, lsNames, lsValues);
            irCountOfPages = irRecordcount / irPageSize + 1;

            cbmPages.Items.Clear();
            for (int i = 1; i < irCountOfPages + 1; i++)
            {
                cbmPages.Items.Add(i);
            }
            notGettingstuck = 1;
            if (irSelectedIndex == -1)
            {
                cbmPages.SelectedIndex = 0;
            }
            else
            {
                cbmPages.SelectedIndex = irSelectedIndex;
            }

            int irCurrentRecordPage = irSelectedIndex + 1;
            if (irCurrentRecordPage < 1)
                irCurrentRecordPage = 1;
            irPrevPageNumber = ((irCurrentRecordPage < 2) ? irCountOfPages : irCurrentRecordPage - 1);
            irNextPageNumber = ((irCurrentRecordPage == irCountOfPages) ? 1 : irCurrentRecordPage + 1);

            srQuery = $@"DECLARE @PageNumber AS INT
DECLARE @RowsOfPage AS INT
SET @PageNumber={irCurrentRecordPage}
SET @RowsOfPage={irPageSize}
SELECT * FROM Bookstbl where BookName like @Text or BookId like @Text
ORDER BY BookId 
OFFSET (@PageNumber-1)*@RowsOfPage ROWS
FETCH NEXT @RowsOfPage ROWS ONLY";

            DataTable dtData = dbConnection.cmd_SelectQuery(srQuery, lsNames, lsValues);
            DataView dvData = new DataView(dtData);
            dtgBooks.ItemsSource = dvData;
        }
    }
}
