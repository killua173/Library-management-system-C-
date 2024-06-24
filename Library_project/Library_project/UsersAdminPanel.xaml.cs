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
    /// Interaction logic for UsersAdminPanel.xaml
    /// </summary>
    public partial class UsersAdminPanel : Window
    {
        public UsersAdminPanel()
        {
            InitializeComponent();
            Loader();
        }

        // Event handler for the "Edit" button click
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item from the data grid
            DataRowView vrItem = (DataRowView)dtgAllusers.SelectedItem;

            // Check if an item is selected
            if (dtgAllusers.SelectedItem == null)
            {
                return;
            }

            // Confirmation dialog for editing the user
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to edit the user \"{vrItem.Row["StudentNo"]}\"?", "Approving Confirmation", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Update the user's information in the database
            string query = @$" update Userstbl set FullName=@FullName, AllowedBooksCount=@AllowedBooksCount,BookReturnPeriod=@BookReturnPeriod,Email=@Email,CurrentRank=@CurrentRank where StudentNo='{vrItem.Row["StudentNo"]}'";
            List<String> stNames = new List<String> { "@FullName", "AllowedBooksCount", "BookReturnPeriod", "Email", "CurrentRank" };
            List<Object> stValues = new List<Object>() { txtUserFullName.Text, txtAllowedBookesCount.Text, txtBookReturnPeriod.Text, txtEmail.Text, txtUserCurrentRank.Text };
            bool result = dbConnection.cmd_UpdateDeleteQuery(query, stNames, stValues);

            // Check if the update was successful
            if (!result)
            {
                MessageBox.Show("Something went wrong");
                Loader();
                return;
            }

            // Reload the data grid
            Loader();

            MessageBox.Show($"You successfully updated the student with the following id \"{vrItem.Row["StudentNo"]}\"");
        }

        // Event handler for the "Delete" button click
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Check if an item is selected
            if (dtgAllusers.SelectedItem == null)
            {
                return;
            }
            DataRowView vrItem = (DataRowView)dtgAllusers.SelectedItem;

            // Confirmation dialog for deleting the user
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to delete the user \"{vrItem.Row["StudentNo"]}\"?", "Approving Confirmation", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Delete the user from the database
            string query = @$"DELETE Userstbl WHERE StudentNo='{vrItem.Row["StudentNo"]}'";
            int result = dbConnection.updateDeleteInsert(query);

            // Check if the deletion was successful
            if (result == 0)
            {
                MessageBox.Show("Something went wrong");
                Loader();
                return;
            }

            MessageBox.Show("You Deleted a User!");
            Loader();
        }

        // Event handler for the text changed event in the search box
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Define the base query for searching users
            string query = "select * from Userstbl where StudentNo like @Text ORDER BY StudentNo ";

            // Check if the input contains letters, in which case search by FullName instead of StudentNo
            if (txtSearch.Text.Any(char.IsLetter))
            {
                query = "select * from Userstbl where FullName like @Text ORDER BY FullName ";
            }

            // Perform the search using the query and input text
            dtgAllusers.ItemsSource = dbConnection.Searchtbl(query, txtSearch.Text);
        }

        // Loads all users into the data grid
        private void Loader()
        {
            // Define the query to retrieve all users
            string query = "select * from Userstbl order by FullName ";

            // Retrieve all users from the database
            DataTable allUserstb = dbConnection.selectTable(query);

            // Create a data view from the retrieved data
            DataView dv = new DataView(allUserstb);

            // Set the data grid's item source to the data view
            dtgAllusers.ItemsSource = dv;
        }

        // Event handler for the selection changed event in the data grid
        private void dtgAllusers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (dtgAllusers.SelectedItem == null)
            {
                return;
            }

            // Get the selected item from the data grid
            DataRowView selectedItem = (DataRowView)dtgAllusers.SelectedItem;

            // Set the text boxes with the selected user's information
            txtUserFullName.Text = selectedItem["FullName"].ToString();
            txtAllowedBookesCount.Text = selectedItem["AllowedBooksCount"].ToString();
            txtBookReturnPeriod.Text = selectedItem["BookReturnPeriod"].ToString();
            txtEmail.Text = selectedItem["Email"].ToString();
            txtUserCurrentRank.Text = selectedItem["CurrentRank"].ToString();
        }

        // Event handler for the "Back" button click
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
