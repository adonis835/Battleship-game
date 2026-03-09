using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace ExamsFirstApp
{
    public class Database
    {
        private String connectionString = "Data source = Battleship.db;Version = 3";
        private SQLiteConnection connection;
        private string playername;

        public Database()
        {
            connection = new SQLiteConnection(connectionString);
        }
        public void DB(string playername , int n , int wins , int loses , int minutes, int seconds)
        {
            connection.Open();
            String insertSQL = "Insert into Battleship (Name,Tries,Wins,Loses,Minutes,Seconds) " +
            "values(@Name,@Tries,@Wins,@Loses,@Minutes,@Seconds)";
            SQLiteCommand command = new SQLiteCommand(insertSQL, connection);
            command.Parameters.AddWithValue("Name", playername);
            command.Parameters.AddWithValue("Tries", n);
            command.Parameters.AddWithValue("Wins", wins);
            command.Parameters.AddWithValue("Loses", loses);
            command.Parameters.AddWithValue("Minutes", minutes);
            command.Parameters.AddWithValue("Seconds", seconds);
            command.ExecuteNonQuery();

            connection.Close();

        }
        public void CreateDatabase()
        {
            connection.Open();
            string createTableSQL = "Create table if not exists Battleship (" +
                "ID integer primary key autoincrement," +
                "Name text," +
                "Tries integer," +
                "Wins integer," +
                "Loses integer," +
                "Minutes integer," +
                "Seconds integer)";
            SQLiteCommand command = new SQLiteCommand(createTableSQL, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
