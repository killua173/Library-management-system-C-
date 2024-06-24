using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.IO;

namespace Library_project
{
    internal class PublicMethods
    {
        // Class-level variables
        public static string loggedUserName = "";
        public static string loggedUserRank = "0";

        // Class representing the result of a check
        public class checkResult
        {
            public bool blResult = false;
            public string srMsg = "";
        }

        // Allowed characters for the username
        private static string allowedCharacters = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZzĞğÜüİı";

        // Method to check the validity of a username
        public static checkResult checkUserName(string srUserName)
        {
            checkResult myResult = new checkResult();

            if (srUserName.Length < 3)
            {
                myResult.srMsg = "Username can't be shorter than 3 characters";
                return myResult;
            }

            if (srUserName.Length > 50)
            {
                myResult.srMsg = "Username can't be longer than 50 characters";
                return myResult;
            }

            foreach (var vrChar in srUserName.ToCharArray())
            {
                if (!allowedCharacters.Contains(vrChar))
                {
                    myResult.srMsg = $"Username can't contain '{vrChar}' character";
                    return myResult;
                }
            }

            myResult.blResult = true;
            return myResult;
        }

        // Method to check the validity of an email
        public static checkResult checkEmail(string srEmail)
        {
            checkResult myResult = new checkResult();

            var email = new EmailAddressAttribute();
            if (email.IsValid(srEmail) == false)
            {
                myResult.srMsg = $"Your email is not a valid email address. Make sure that you have typed your email correctly!";
                return myResult;
            }

            string srCommand = "select 1 from tblUsers where Email=@Email";

            DataTable dtUsers = dbConnection.cmd_SelectQuery(srCommand, new List<string> { "@Email" }, new List<object> { srEmail });

            if (dtUsers.Rows.Count > 0)
            {
                myResult.srMsg = $"This email is already being used";
                return myResult;
            }

            myResult.blResult = true;
            return myResult;
        }

        // Method to check the validity of a phone number
        public static checkResult CheckPhoneNumber(string phoneNumber)
        {
            checkResult result = new checkResult();

            string query = "SELECT COUNT(*) FROM Userstbl WHERE PhoneNumber=@PhoneNumber";

            List<string> parameterNames = new List<string>() { "@PhoneNumber" };
            List<object> parameterValues = new List<object>() { phoneNumber };

            int count = dbConnection.cmd_Count(query, parameterNames, parameterValues);

            if (count > 0)
            {
                result.srMsg = "This Phone Number is already being used";
                return result;
            }

            if (phoneNumber.Length > 14 || phoneNumber.Length < 9 || phoneNumber.Any(char.IsLetter))
            {
                result.srMsg = "Your phone number is incorrect";
                return result;
            }

            result.blResult = true;
            return result;
        }

        // Method to return the hashed password for a user
        public static string returnUserHashedPw(string srPwRaw, string srUserSalt)
        {
            return ComputeSha256Hash(srPwRaw + srUserSalt);
        }

        // Method to compute the SHA256 hash of a string
        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Method to load borrowed books for a student
        public static DataView BarrowedBloader(int StudentNo)
        {
            string query = $@"SELECT BookName, Writer, Category, StudentNo, BorrowingDate, AdminConfirmationState, BookState, Bookstbl.BookId FROM Bookstbl INNER JOIN BorrowedBookstbl ON Bookstbl.BookId = BorrowedBookstbl.BookId WHERE StudentNo = '{StudentNo}' AND AdminConfirmationState = 0 AND BookState = 0";

            DataTable dtbooks = dbConnection.selectTable(query);

            DataView dv = new DataView(dtbooks);

            return dv;
        }
    }
}
