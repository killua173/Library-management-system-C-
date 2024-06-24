using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_project
{
    internal class dbConnection
    {
        //the connectio string
        public static string srConnectionString = "Data Source=DESKTOP-SFQ2USJ;Initial Catalog=Library; Trusted_Connection=Yes;persist security info=False;";


        //it would load infotmation from sql to datatable you must pass the parameters and values to this one
        public static DataTable cmd_SelectQuery(string stQuery, List<string> lstParameterNames, IList<object> lstParameters)
        {

            DataTable dsCmdPara = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(srConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(stQuery, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        for (int i = 0; i < lstParameterNames.Count; i++)
                        {
                            cmd.Parameters.AddWithValue(lstParameterNames[i], lstParameters[i].ToString());
                        }
                        using (SqlDataAdapter sqlDa = new SqlDataAdapter(cmd))
                        {
                            sqlDa.Fill(dsCmdPara);
                            return dsCmdPara;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                insertIntoTblSqlErrors(stQuery + " " + E.Message.ToString());


            }

            return dsCmdPara;



        }
        //same as the one ubove tho without passing any values you put the values in the query if it was safe

        public static DataTable selectTable(string strQuery)
        {
            //System.IO.File.AppendAllText(@"C:\temp\dbcon.txt", strQuery + "\r\n\r\n");

            DataTable dSet = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(srConnectionString))
                {
                    connection.Open();
                    using (SqlDataAdapter DA = new SqlDataAdapter(strQuery, connection))
                    {
                        DA.Fill(dSet);
                    }
                }
                return dSet;
            }
            catch (Exception E)
            {
                insertIntoTblSqlErrors(strQuery + " " + E.Message.ToString());
                return dSet;
            }
        }


        //insert errrors that being cought by all methods here to sql table

        private static void insertIntoTblSqlErrors(string srErrorQuery)
        {
            string srCommandTextSqlError = " insert into tblSqlErrors values(@ErrorQueryString,@StackTrace) ";
            cmd_UpdateDeleteQuery(srCommandTextSqlError, new List<string> { "@ErrorQueryString", "@StackTrace" }, new List<object> { srErrorQuery, Environment.StackTrace.ToString() });
        }


        // search it automaticly adds the parameters so should only put the values accept only one value

        public static DataView Searchtbl(string stQuery, string txtSearch)
        {
            List<string> lsNames = new List<string> { "@Text" };

            List<Object> lsValues = new List<Object> { "%" + txtSearch + "%" };
            if (txtSearch.Length == 1)
            {
                lsValues = new List<Object> { txtSearch + "%" };
            }
            DataView dv = new DataView(cmd_SelectQuery(stQuery, lsNames, lsValues));

            return dv;

        }





        //get the counts of queries with count command with out parameters
        public static int Count(string strQuery)
        {
            int irAffected = 0;


            try
            {


                using (SqlConnection connection = new SqlConnection(srConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(strQuery, connection))
                    {
                        irAffected = (Int32)command.ExecuteScalar();
                    }
                }

                return irAffected;
            }
            catch (Exception E)
            {
                insertIntoTblSqlErrors(strQuery + " " + E.Message.ToString());
                return irAffected;
            }

        }

        //get the counts of queries with count command with parameters
        public static int cmd_Count(string srCommandText, List<string> lstParameterNames, IList<object> lstParameters)
        {
            int irAffected = 0;


            try
            {


                using (SqlConnection connection = new SqlConnection(srConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(srCommandText, connection))
                    {
                        command.CommandType = CommandType.Text;
                        for (int i = 0; i < lstParameterNames.Count; i++)
                        {
                            command.Parameters.AddWithValue(lstParameterNames[i], lstParameters[i].ToString());
                        }


                        irAffected = Int32.Parse(command.ExecuteScalar().ToString());
                    }
                }

                return irAffected;
            }
            catch (Exception E)
            {
                insertIntoTblSqlErrors(srCommandText + " " + E.Message.ToString());
                return irAffected;
            }

        }


        //does insert updata delete you should pass the parameters and values


        public static bool cmd_UpdateDeleteQuery(string srCommandText, List<string> lstParameterNames, IList<object> lstParameters)
        {
            //   System.IO.File.AppendAllText(@"C:\temp\dbcon2.txt", srCommandText + "\r\n\r\n");

            try
            {

                using (SqlConnection connection = new SqlConnection(srConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(srCommandText, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        for (int i = 0; i < lstParameterNames.Count; i++)
                        {
                            cmd.Parameters.AddWithValue(lstParameterNames[i], lstParameters[i].ToString());
                        }


                        connection.Open();

                        cmd.ExecuteNonQuery();
                        return true;


                    }
                }
            }
            catch (Exception E)
            {
                insertIntoTblSqlErrors(srCommandText + " " + E.Message.ToString());
            }
            return false;
        }
        //same as the one upove without you passing the parameters ,or justing putting the values in the query

        public static int updateDeleteInsert(string strQuery, int irRetryCount = 1)
        {

            for (int i = 0; i < irRetryCount; i++)
            {




                try
                {
                    using (SqlConnection connection = new SqlConnection(srConnectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(strQuery, connection))
                        {
                            return command.ExecuteNonQuery();
                        }
                    }


                }
                catch (Exception E)
                {
                    insertIntoTblSqlErrors(strQuery + " " + E.Message.ToString());
                }

                System.Threading.Thread.Sleep(1000);
            }

            return 0;
        }





    }


}
