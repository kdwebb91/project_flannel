using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Metadata
    {
        private static string METADATA_DB_LOC = "Data Source=subset_track_metadata.db;Version=3;";
        private static SQLiteConnection METADATA_DB = new SQLiteConnection(METADATA_DB_LOC);

        public static string getArtistIdFromName(string szArtistName)
        {
            string szArtistId = string.Empty;
            string szQuery = "SELECT artist_id FROM songs WHERE artist_name='" + szArtistName + "'";
            try
            {
                METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    szArtistId = reader["artist_id"].ToString();
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return szArtistId;
        }

        public static string getArtistNameFromId(string szArtistId)
        {
            string szArtistName = string.Empty;
            string szQuery = "SELECT artist_name FROM songs WHERE artist_id='" + szArtistId + "'";
            try
            {
                METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    szArtistName = reader["artist_name"].ToString();
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return szArtistName;
        }

        public static int getNumberOfArtists()
        {
            int numberOfArtists = 0;
            string szQuery = "SELECT COUNT(artist_id) FROM artists";
            try
            {
                METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    numberOfArtists = Convert.ToInt32(reader[0].ToString());
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return numberOfArtists;
        }

        public static void getMostFamiliarArtist()
        {
            List<double> fam = new List<double>() { 0, 0, 0 };
            List<string> artist = new List<string>() { "", "", "" };
            try
            {
                METADATA_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                for (int i = 0; i < artists.Count; i++)
                {
                    szQuery = "SELECT artist_familiarity FROM songs WHERE artist_id='" + artists[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, METADATA_DB);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            double temp = Convert.ToDouble(reader[0].ToString());
                            if (temp > fam[0])
                            {
                                fam[0] = temp;
                                fam.Sort();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Artist " + artists[i] + " threw an exception.");
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                for (int i = 0; i < fam.Count; i++)
                {
                    szQuery = "SELECT artist_id FROM songs WHERE artist_familiarity='" + fam[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, METADATA_DB);
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string temp = reader["artist_id"].ToString();
                            if (artist.Contains(temp))
                            {
                                continue;
                            }
                            else
                            {
                                artist[i] = temp;
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            for (int i = fam.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(getArtistNameFromId(artist[i]));
                Console.WriteLine(fam[i]);
            }
        }

        public static void getLeastFamiliarArtist()
        {
            List<double> fam = new List<double>() { 1, 1, 1 };
            List<string> artist = new List<string>() { "", "", "" };
            try
            {
                METADATA_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                for (int i = 0; i < artists.Count; i++)
                {
                    szQuery = "SELECT artist_familiarity FROM songs WHERE artist_id='" + artists[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, METADATA_DB);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            double temp = Convert.ToDouble(reader[0].ToString());
                            if (temp < fam[fam.Count - 1])
                            {
                                fam[fam.Count - 1] = temp;
                                fam.Sort();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Artist " + artists[i] + " threw an exception.");
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                for (int i = 0; i < fam.Count; i++)
                {
                    szQuery = "SELECT artist_id FROM songs WHERE artist_familiarity='" + fam[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, METADATA_DB);
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string temp = reader["artist_id"].ToString();
                            if (artist.Contains(temp))
                            {
                                continue;
                            }
                            else
                            {
                                artist[i] = temp;
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            for (int i = fam.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(getArtistNameFromId(artist[i]));
                Console.WriteLine(fam[i]);
            }
        }

        public static int getNumberOfSongs(string szArtistId)
        {
            int numSongs = 0;
            try
            {
                METADATA_DB.Open();
                string szQuery = "SELECT COUNT(song_id) FROM songs WHERE artist_id='" + szArtistId + "'";
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    numSongs = Convert.ToInt32(reader[0].ToString());
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return numSongs;
        }

        public static double getNumberOfSongsPerArtist()
        {
            double avgNumSongs = 0;
            try
            {
                METADATA_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                for (int i = 0; i < artists.Count; i++)
                {
                    avgNumSongs += getNumberOfSongs(artists[i]);
                }
                avgNumSongs /= artists.Count;
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return avgNumSongs;
        }
    }
}
