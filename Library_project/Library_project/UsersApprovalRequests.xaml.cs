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
    /// Interaction logic for UsersApprovalRequests.xaml
    /// </summary>
    public partial class UsersApprovalRequests : Window
    {
        public UsersApprovalRequests()
        {
            InitializeComponent();
            Search();
        }

        // Performs the search based on the user's input
        private void Search()
        {
            // Define the base query for searching user approval requests
            string query = "select * from Userstbl where StudentNo like @Text and CurrentRank !=WantedRank ORDER BY StudentNo ";

            // Check if the input contains letters, in which case search by FullName instead of StudentNo
            if (txtSearch.Text.Any(char.IsLetter))
            {
                query = "select * from Userstbl where FullName like @Text and CurrentRank !=WantedRank ORDER BY FullName ";
            }

            // Perform the search using the query and input text
            dtgUsersNeedApproval.ItemsSource = dbConnection.Searchtbl(query, txtSearch.Text);
        }

        // Event handler for the text changed event in the search box
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        // Event handler for the "Approve" button click
        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item from the data grid
            DataRowView vrItem = (DataRowView)dtgUsersNeedApproval.SelectedItem;

            // Check if an item is selected
            if (dtgUsersNeedApproval.SelectedItem == null)
            {
                return;
            }

            // Confirmation dialog for approving user rank
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to approve the user rank for the user with the following name \"{vrItem.Row["FullName"]}\"?", "Approving Confirmation", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Update the user's current rank to the wanted rank
            string query = @$" update Userstbl set CurrentRank=WantedRank where StudentNo='{vrItem.Row["StudentNo"]}'";
            int result = dbConnection.updateDeleteInsert(query);

            // Check if the update was successful
            if (result != 1)
            {
                MessageBox.Show("Something went wrong");
                Search();
                return;
            }

            // Reload the data grid
            Search();

            MessageBox.Show($"You successfully updated the information of the user with the following id \"{vrItem.Row["StudentNo"]}\"");
        }

        // Event handler for the "Disapprove" button click
        private void btnDisapprove_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item from the data grid
            DataRowView vrItem = (DataRowView)dtgUsersNeedApproval.SelectedItem;

            // Check if an item is selected
            if (dtgUsersNeedApproval.SelectedItem == null)
            {
                return;
            }

            // Confirmation dialog for disapproving user rank
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"Are you sure you want to disapprove the user rank for the user with the following name \"{vrItem.Row["FullName"]}\"?", "Approving Confirmation", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes)
            {
                return;
            }

            // Update the user's wanted rank to the current rank
            string query = @$" update Userstbl set WantedRank=CurrentRank where StudentNo='{vrItem.Row["StudentNo"]}'";
            int result = dbConnection.updateDeleteInsert(query);

            // Check if the update was successful
            if (result != 1)
            {
                MessageBox.Show("Something went wrong");
                Search();
                return;
            }

            // Reload the data grid
            Search();

            MessageBox.Show($"You successfully updated the information of the user with the following id \"{vrItem.Row["StudentNo"]}\"");
        }

        // Event handler for the "Back" button click
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
