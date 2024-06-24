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
    /// Interaction logic for LogInW.xaml
    /// </summary>
    public partial class LogInW : Window
    {
        public LogInW()
        {
            InitializeComponent();
        }

        private void logInBtn_Click(object sender, RoutedEventArgs e)
        {
            // Check if the StudentNoOrEmailtxt and pwtxt fields are empty
            if (StudentNoOrEmailtxt.Text == "" || pwtxt.Password == "")
            {
                MessageBox.Show("Fields can't be empty!!");
                return;
            }

            // Retrieve the password salt for the given email or student number
            string query = "SELECT PasswordSalt FROM Userstbl WHERE Email = @Text OR StudentNo = @Text";
            DataTable dbPwsalt = dbConnection.cmd_SelectQuery(query, new List<String> { "@Text" }, new List<Object> { StudentNoOrEmailtxt.Text });

            // Check if a password salt was found for the given email or student number
            if (dbPwsalt.Rows.Count == 0)
            {
                MessageBox.Show("Invalid login!");
                return;
            }

            // Retrieve the password salt
            string stSalt = dbPwsalt.Rows[0]["PasswordSalt"].ToString();

            // Hash the password with the salt
            string UserHashedSaledPw = PublicMethods.returnUserHashedPw(pwtxt.Password, stSalt.ToString());

            // Retrieve the user information based on the hashed password and email or student number
            query = $@"SELECT StudentNo, CurrentRank, FullName, AllowedBooksCount, BooksPastDue, BookReturnPeriod, BorrowedBooksCount, Email, PhoneNumber
                        FROM Userstbl
                        WHERE (PasswordHash = '{UserHashedSaledPw}' AND StudentNo = '{StudentNoOrEmailtxt.Text}')
                            OR (PasswordHash = '{UserHashedSaledPw}' AND Email = '{StudentNoOrEmailtxt.Text}')";

            DataTable LoggedUser = dbConnection.selectTable(query);

            // Check if a user was found with the given credentials
            if (LoggedUser.Rows.Count == 0)
            {
                MessageBox.Show("Invalid login");
                return;
            }

            // Set the logged-in user information
            UserCons.SetLoggedInUser(LoggedUser);

            // Check if the logged-in user is an admin (StudentNo == 3)
            if (UserCons.LoggedInUser.StudentNo == 3)
            {
                // Create an instance of the AdminW window
                AdminW usersWs = new AdminW();

                // Show the AdminW window
                usersWs.Show();

                // Close the current window
                Window.GetWindow(this).Close();

                return;
            }

            // Create an instance of the UsersW window
            UsersW usersW = new UsersW();

            // Show the UsersW window
            usersW.Show();

            // Close the current window
            Window.GetWindow(this).Close();
        }

        private void signUpbtn_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the RegistrationW window
            RegistrationW registrationWindow = new RegistrationW();

            // Show the RegistrationW window
            registrationWindow.Show();

            // Close the current window
            Window.GetWindow(this).Close();
        }

        private void dsad_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the UsersAdminPanel window
            UsersAdminPanel usersW = new UsersAdminPanel();

            // Show the UsersAdminPanel window
            usersW.Show();

            // Close the current window
            Window.GetWindow(this).Close();
        }
    }
}
