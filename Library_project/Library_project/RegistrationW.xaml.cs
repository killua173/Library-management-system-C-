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
    /// Interaction logic for RegistrationW.xaml
    /// </summary>
    public partial class RegistrationW : Window
    {
        public RegistrationW()
        {
            InitializeComponent();
            initRankComboBox();
        }

        // Class representing user ranks for the combo box
        private class csUserRanks
        {
            public int irUserRankId { get; set; }
            public string srUserRankDisplay { get; set; }
        }

        // Method to initialize the user ranks combo box
        private void initRankComboBox()
        {
            List<csUserRanks> lstUserRanks = new List<csUserRanks> { };
            lstUserRanks.Add(new csUserRanks { irUserRankId = 0, srUserRankDisplay = "Please Select User Rank" });

            // Query to retrieve user ranks from the database
            string query = " select * from Rankstbl order by RanksLevel asc";

            foreach (DataRow drw in dbConnection.selectTable(query).Rows)
            {
                lstUserRanks.Add(new csUserRanks
                {
                    irUserRankId = Convert.ToInt32(drw["RanksLevel"].ToString()),
                    srUserRankDisplay = drw["UserRankDisplay"].ToString()
                });
            }

            // Set the combo box's item source and display member
            UserRanksComboBox.ItemsSource = lstUserRanks;
            UserRanksComboBox.DisplayMemberPath = "srUserRankDisplay";

            UserRanksComboBox.SelectedIndex = 0;
        }

        // Event handler for the "Register" button click
        private void btRegister_Click(object sender, RoutedEventArgs e)
        {
            // Check if the passwords match
            if (PasswordBox1.Password != PasswordBox2.Password)
            {
                MessageBox.Show("Passwords don't match");
                return;
            }

            // Check password length
            if (PasswordBox1.Password.Length < 3)
            {
                MessageBox.Show("The password should be at least 3 characters");
                return;
            }

            // Validate the first name
            var vrResult = PublicMethods.checkUserName(FirstNameTextBox.Text);
            if (!vrResult.blResult)
            {
                MessageBox.Show("Error: " + vrResult.srMsg);
                return;
            }

            // Validate the last name
            vrResult = PublicMethods.checkUserName(LastNameTextBox.Text);
            if (!vrResult.blResult)
            {
                MessageBox.Show("Error: " + vrResult.srMsg);
                return;
            }

            // Validate the email
            vrResult = PublicMethods.checkEmail(EmailTextBox.Text);
            if (!vrResult.blResult)
            {
                MessageBox.Show("Error: " + vrResult.srMsg);
                return;
            }

            // Validate the phone number
            vrResult = PublicMethods.CheckPhoneNumber(PhoneNumberTextBox.Text);
            if (!vrResult.blResult)
            {
                MessageBox.Show("Error: " + vrResult.srMsg);
                return;
            }

            // Check if a user rank is selected
            if (UserRanksComboBox.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a Rank");
                return;
            }

            // Generate a random salt for password hashing
            int userSalt = new Random().Next();

            // Hash the password using the generated salt
            string userHashedPassword = PublicMethods.returnUserHashedPw(PasswordBox1.Password.ToString(), userSalt.ToString());

            // Get the registration date
            DateTime registrationDate = DateTime.Now;

            // Insert the user data into the database
            string query = "INSERT INTO Userstbl VALUES (@UserFullName, @Email, @Password, @PasswordSalt, @PhoneNumber, @UserCurrentRank, @UserWantedRank, @RegistrationDate, @AllowedBookesCount, @NotReternedBooks, @Period, @BorrowedBooksCount)";
            List<string> parameterNames = new List<string> { "@UserFullName", "@Email", "@Password", "@PasswordSalt", "@PhoneNumber", "@UserCurrentRank", "@UserWantedRank", "@RegistrationDate", "@AllowedBookesCount", "@NotReternedBooks", "@Period", "@BorrowedBooksCount" };
            List<object> parameterValues = new List<object>() { FirstNameTextBox.Text + " " + LastNameTextBox.Text, EmailTextBox.Text, userHashedPassword, userSalt, PhoneNumberTextBox.Text, 1, ((csUserRanks)UserRanksComboBox.SelectedItem).irUserRankId, registrationDate.ToString("yyyy-MM-dd HH:mm:ss.000"), 5, 0, 7, 0 };
            bool newAccount = dbConnection.cmd_UpdateDeleteQuery(query, parameterNames, parameterValues);

            // Check if the account creation was successful
            if (newAccount)
            {
                MessageBox.Show("You have created a new account");

                // Retrieve the student number for the newly registered user
                string query1 = "SELECT StudentNo FROM Userstbl WHERE Email = @Text";
                DataTable dbPwsalt = dbConnection.cmd_SelectQuery(query1, new List<String> { "@Text" }, new List<Object> { EmailTextBox.Text });
                string studentno = dbPwsalt.Rows[0]["StudentNo"].ToString();

                MessageBox.Show($"Your student number is \"{studentno}\". Please remember it.");

                // Open the login window and close the current window
                LogInW loginnWindow = new LogInW();
                loginnWindow.Show();
                Window.GetWindow(this).Close();
                return;
            }

            MessageBox.Show("Something went wrong. Please try again.");
        }

        // Event handler for the "Login" button click
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the login window
            LogInW loginnWindow = new LogInW();
            loginnWindow.Show();

            // Close the current window
            Window.GetWindow(this).Close();
        }
    }
}
