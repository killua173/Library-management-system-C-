using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Library_project
{
    internal class UserCons
    {
        // Inner class representing the logged-in user information
        public class LoggedInUserInfo
        {
            public int StudentNo { get; set; }
            public int UserRank { get; set; }
            public int UserAllowedBooks { get; set; }
            public int BookReturnPeriod { get; set; }
            public string NotReturnedBooks { get; set; }
            public int BorrowedBooks { get; set; }
            public string UserName { get; set; }
            public string UserPhoneNumber { get; set; }
            public string UserEmail { get; set; }
        }

        // Static property to store the logged-in user
        public static LoggedInUserInfo LoggedInUser { get; set; }

        // Method to set the logged-in user based on the provided DataTable
        public static void SetLoggedInUser(DataTable loggedInUserData)
        {
            LoggedInUser = new LoggedInUserInfo()
            {
                // Extract and parse the necessary data from the DataTable
                StudentNo = Int32.Parse(loggedInUserData.Rows[0]["StudentNo"].ToString()),
                UserName = loggedInUserData.Rows[0]["FullName"].ToString(),
                UserEmail = loggedInUserData.Rows[0]["Email"].ToString(),
                UserPhoneNumber = loggedInUserData.Rows[0]["PhoneNumber"].ToString(),
                UserRank = Int32.Parse(loggedInUserData.Rows[0]["CurrentRank"].ToString()),
                UserAllowedBooks = Int32.Parse(loggedInUserData.Rows[0]["AllowedBooksCount"].ToString()),
                BorrowedBooks = Int32.Parse(loggedInUserData.Rows[0]["BorrowedBooksCount"].ToString()),
                BookReturnPeriod = Int32.Parse(loggedInUserData.Rows[0]["BookReturnPeriod"].ToString()),
                NotReturnedBooks = loggedInUserData.Rows[0]["BooksPastDue"].ToString()
            };
        }

        // Method to reset the logged-in user
        public static void ResetLoggedInUser()
        {
            LoggedInUser = null;
        }
    }
}
