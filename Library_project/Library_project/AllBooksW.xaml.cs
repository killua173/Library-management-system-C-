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
    /// Interaction logic for AllBooksW.xaml
    /// </summary>
    public partial class AllBooksW : Window
    {
        public AllBooksW()
        {
            InitializeComponent();
            refreshDataGrid();
        }

        // Event handler for the text changed event of the search box
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            refreshDataGrid();
        }
        private int notGettingstuck;

        int irPageSize = 35;

        int irCountOfPages;

        int irPrevPageNumber, irNextPageNumber;



        // Event handler for the Add button click event
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Display a confirmation message box
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to add this book?", "Returning Confirmation", System.Windows.MessageBoxButton.YesNo);

            // Check the user's response
            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Prepare the query to insert the new book
            string query = "Insert into Bookstbl Values (@Writer,@BookName,@Quintity,@TotalBooks,@PublishingYear,@Category,@NumberOfPages)";

            // Create lists to store the parameter names and values
            List<string> stNames = new List<string> { "@Writer", "@BookName", "@Quintity", "@TotalBooks", "@PublishingYear", "@Category", "@NumberOfPages" };
            List<object> stValues = new List<object>() { txtWriter.Text, txtBookName.Text, txtQuantity.Text, txtQuantity.Text, txtYear.Text, txtCategory.Text, txtPages.Text };

            // Execute the query and get the result
            bool result = dbConnection.cmd_UpdateDeleteQuery(query, stNames, stValues);

            // Check the result and display appropriate messages
            if (!result)
            {
                MessageBox.Show("Something went wrong. Make sure you fill all the textboxes.");
                refreshDataGrid();
                return;
            }

            MessageBox.Show("You added a new book!");
            refreshDataGrid();
        }

        // Event handler for the Edit button click event
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item from the data grid
            DataRowView vrItem = (DataRowView)dtgAllBooks.SelectedItem;

            // Check if an item is selected
            if (dtgAllBooks.SelectedItem == null)
            {
                return;
            }

            // Display a confirmation message box
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to edit the book \"{vrItem.Row["BookName"]}\"?", "Approving Confirmation", System.Windows.MessageBoxButton.YesNo);

            // Check the user's response
            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Prepare the query to update the book
            string query = $@"update Bookstbl set Writer=@Writer, BookName=@BookName, Quintity=@Quintity ,TotalBooks=@TotalBooks, PublishingYear=@PublishingYear, Category=@Category, NumberOfPages=@NumberOfPages where BookId='{vrItem.Row["BookId"]}'";

            // Create lists to store the parameter names and values
            List<string> stNames = new List<string> { "@Writer", "@BookName", "@Quintity", "@TotalBooks", "@PublishingYear", "@Category", "@NumberOfPages" };
            List<object> stValues = new List<object>() { vrItem.Row["Writer"], vrItem.Row["BookName"], vrItem.Row["Quintity"], vrItem.Row["TotalBooks"], vrItem.Row["PublishingYear"], vrItem.Row["Category"], vrItem.Row["NumberOfPages"] };

            // Execute the query and get the result
            bool result = dbConnection.cmd_UpdateDeleteQuery(query, stNames, stValues);

            // Check the result and display appropriate messages
            if (!result)
            {
                MessageBox.Show("Something went wrong");
                refreshDataGrid();
                return;
            }

            MessageBox.Show("You edited a book!");
            refreshDataGrid();
        }

        // Event handler for the Next button click event
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            cbmPages.SelectedIndex = irNextPageNumber - 1;
            refreshDataGrid();
        }

        // Event handler for the Previous button click event
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            cbmPages.SelectedIndex = irPrevPageNumber - 1;
            refreshDataGrid();
        }

        // Event handler for the Delete button click event
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Check if an item is selected
            if (dtgAllBooks.SelectedItem == null)
            {
                return;
            }

            // Get the selected item from the data grid
            DataRowView vrItem = (DataRowView)dtgAllBooks.SelectedItem;

            // Display a confirmation message box
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to delete the book \"{vrItem.Row["BookName"]}\"?", "Approving Confirmation", System.Windows.MessageBoxButton.YesNo);

            // Check the user's response
            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Prepare the query to delete the book
            string query = @$"DELETE Bookstbl WHERE BookId='{vrItem.Row["BookId"]}'";

            // Execute the query and get the result
            int result = dbConnection.updateDeleteInsert(query);

            // Check the result and display appropriate messages
            if (result == 0)
            {
                MessageBox.Show("Something went wrong");
                refreshDataGrid();
                return;
            }

            MessageBox.Show("You deleted a book!");
            refreshDataGrid();
        }

        // Event handler for the Back button click event
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        // Method to refresh the data grid based on search criteria and pagination
        private void refreshDataGrid()
        {
            // Create lists to store the parameter names and values for the search
            List<string> lsNames = new List<string> { "@Text" };
            List<object> lsValues = new List<object> { "%" + txtSearch.Text + "%" };

            // Get the selected index of the pagination combobox
            int irSelectedIndex = cbmPages.SelectedIndex;

            // Adjust the search values if the search text length is 1
            if (txtSearch.Text.Length == 1)
            {
                lsValues.Clear();
                lsValues.Add(txtSearch.Text + "%");
            }

            // Prepare the query to count the total number of records based on search criteria
            string srQuery = "select COUNT (*) from Bookstbl where BookName like @Text or BookId like @Text ";

            // Execute the query and get the total record count
            int irRecordcount = dbConnection.cmd_Count(srQuery, lsNames, lsValues);

            // Calculate the total number of pages
            irCountOfPages = irRecordcount / irPageSize + 1;

            // Clear the combobox items
            cbmPages.Items.Clear();

            // Add page numbers to the combobox
            for (int i = 1; i < irCountOfPages + 1; i++)
            {
                cbmPages.Items.Add(i);
            }

            notGettingstuck = 1;

            // Set the selected index of the combobox
            if (irSelectedIndex == -1)
            {
                cbmPages.SelectedIndex = 0;
            }
            else
            {
                cbmPages.SelectedIndex = irSelectedIndex;
            }

            // Calculate the current record page, previous page number, and next page number
            int irCurrentRecordPage = irSelectedIndex + 1;

            if (irCurrentRecordPage < 1)
                irCurrentRecordPage = 1;

            irPrevPageNumber = ((irCurrentRecordPage < 2) ? irCountOfPages : irCurrentRecordPage - 1);
            irNextPageNumber = ((irCurrentRecordPage == irCountOfPages) ? 1 : irCurrentRecordPage + 1);

            // Prepare the query to select the records for the current page
            srQuery = $@"DECLARE @PageNumber AS INT
DECLARE @RowsOfPage AS INT
SET @PageNumber={irCurrentRecordPage}
SET @RowsOfPage={irPageSize}
SELECT * FROM Bookstbl where BookName like @Text or BookId like @Text
ORDER BY BookId 
OFFSET (@PageNumber-1)*@RowsOfPage ROWS
FETCH NEXT @RowsOfPage ROWS ONLY";

            // Execute the query and get the data
            DataTable dtData = dbConnection.cmd_SelectQuery(srQuery, lsNames, lsValues);
            DataView dvData = new DataView(dtData);

            // Set the data grid's item source to the data view
            dtgAllBooks.ItemsSource = dvData;
        }
    }
}
