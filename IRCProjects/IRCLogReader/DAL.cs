using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCLogReader
{
    class DAL
    {
        //public void asdasasd()
        //{

        //    System.Data.SQLite.SQLiteConnection c = new System.Data.SQLite.SQLiteConnection("Data Source=IRC.db3;Pooling=true;FailIfMissing=false");
        //    var conn = System.Data.SQLite.SQLiteFactory.Instance.CreateConnection();
        //    conn.Open();

        //    var command = conn.CreateCommand();
        //    command.
        //}
        static string connectionString = "Data Source=C:\\anime4logs\\Unified\\LOGS.s3db";


        public static DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(connectionString);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch
            {
                // Catching exceptions is for communists 
            }
            return dt;
        }

        public static int ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(connectionString);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            cnn.Close();
            return rowsUpdated;
        }



        public static string ExecuteScalar(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(connectionString);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            object value = mycommand.ExecuteScalar();
            cnn.Close();
            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }
    }
}
