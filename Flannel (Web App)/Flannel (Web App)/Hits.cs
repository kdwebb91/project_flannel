using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Hits
    {
        private static string HITS_DB_LOC = "Data Source=hits.db;Version=3;";
        private static SQLiteConnection HITS_DB = new SQLiteConnection(HITS_DB_LOC);

        public static double GetHubScore(string artistId)
        {
            double score = 0;
            string szQuery = "SELECT hub FROM hits WHERE artist_id='" + artistId + "'";
            try
            {
                HITS_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, HITS_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    score =  Convert.ToDouble(reader["hub"].ToString());
                }
                HITS_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return score;
        }

        public static double GetAuthScore(string artistId)
        {
            double score = 0;
            string szQuery = "SELECT auth FROM hits WHERE artist_id='" + artistId + "'";
            try
            {
                HITS_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, HITS_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    score = Convert.ToDouble(reader["auth"].ToString());
                }
                HITS_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return score;
        }
    }
}
