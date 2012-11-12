using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Data.SQLite;

namespace Flannel
{
    class Program
    {
        private static string METADATA_DB_LOC = "Data Source=C:\\Users\\Kevin\\Documents\\MillionSongSubset\\AdditionalFiles\\subset_track_metadata.db;Version=3;";
        private static string TERM_DB_LOC = "Data Source=C:\\Users\\Kevin\\Documents\\MillionSongSubset\\AdditionalFiles\\subset_artist_term.db;Version=3;";
        private static string SIMILARITY_DB_LOC = "Data Source=C:\\Users\\Kevin\\Documents\\MillionSongSubset\\AdditionalFiles\\subset_artist_similarity.db;Version=3;";

        private static SQLiteConnection METADATA_DB = new SQLiteConnection(METADATA_DB_LOC);
        private static SQLiteConnection TERM_DB = new SQLiteConnection(TERM_DB_LOC);
        private static SQLiteConnection SIMILARITY_DB = new SQLiteConnection(SIMILARITY_DB_LOC);

        private static void run_py_script(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Python27\\python.exe";
            start.Arguments = string.Format("{0} {1}",cmd,args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using(Process proc = Process.Start(start))
            {
                using(StreamReader reader = proc.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }

        private static void pyEx()
        {
            string szCmd = "C:\\Users\\Kevin\\Documents\\hello_world.py";
            string szArgs = string.Empty;
            run_py_script(szCmd, szArgs);
            szCmd = "C:\\Users\\Kevin\\Documents\\print.py";
            Console.Write("Enter a string:\t");
            szArgs = Console.ReadLine();
            Console.Write("You entered:\t");
            run_py_script(szCmd, szArgs);
        }

        static string getArtistIdFromName(string szArtistName)
        {
            METADATA_DB.Open();
            string szArtistId = string.Empty;
            string szQuery = "SELECT artist_id FROM songs WHERE artist_name='" + szArtistName + "'";
            try
            {
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    szArtistId = reader["artist_id"].ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            METADATA_DB.Close();
            return szArtistId;
        }

        static string getArtistNameFromId(string szArtistId)
        {
            METADATA_DB.Open();
            string szArtistName = string.Empty;
            string szQuery = "SELECT artist_name FROM songs WHERE artist_id='" + szArtistId + "'";
            try
            {
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    szArtistName = reader["artist_name"].ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            METADATA_DB.Close();
            return szArtistName;
        }

        static bool testArtistNameAndIdMapping()
        {
            bool result = false;
            string szArtistName = "The Beatles";
            string szArtistId = getArtistIdFromName(szArtistName);
            if (szArtistId != string.Empty)
            {
                if (szArtistName == getArtistNameFromId(szArtistId))
                {
                    //Console.WriteLine(szArtistName + " <--> " + szArtistId);
                    result = true;
                }
                else
                {
                    //Console.WriteLine("ERROR: Mapping failed.");
                }
            }
            else
            {
                //Console.WriteLine("ERROR: Could not find artist:\t'" + szArtistName + "'");
            }
            return result;
        }

        static List<string> getSimilarArtists(string szArtistId)
        {
            List<string> similarArtists = new List<string>();
            SIMILARITY_DB.Open();
            string szQuery = "SELECT similar FROM similarity WHERE target='" + szArtistId + "'";
            try
            {
                SQLiteCommand command = new SQLiteCommand(szQuery, SIMILARITY_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    similarArtists.Add(reader[0].ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            SIMILARITY_DB.Close();
            return similarArtists;
        }

        private static void testGetSimilarArtists()
        {
            Console.WriteLine("\n-----------------------");
            Console.WriteLine("GET SIMILAR ARTISTS");
            Console.WriteLine("-----------------------");
            while (true)
            {
                Console.Write("\nEnter Artist Name:\t");
                string szArtistName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(szArtistName))
                {
                    break;
                }
                string szArtistId = getArtistIdFromName(szArtistName);
                if (string.IsNullOrWhiteSpace(szArtistId))
                {
                    Console.WriteLine("ERROR: Artist not found!");
                    continue;
                }
                List<string> similarArtists = getSimilarArtists(szArtistId);
                Console.WriteLine('\n' + szArtistName + " has " + similarArtists.Count.ToString() + " similar artist(s):");
                for (int i = 0; i < similarArtists.Count; i++)
                {
                    Console.WriteLine(getArtistNameFromId(similarArtists[i]));
                }
            }
        }

        static List<string> getArtistGenres(string szArtistId)
        {
            List<string> artistGenres = new List<string>();
            TERM_DB.Open();
            string szQuery = "SELECT term FROM artist_term WHERE artist_id='" +szArtistId + "'";
            try
            {
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artistGenres.Add(reader[0].ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            TERM_DB.Close();
            return artistGenres;
        }

        private static void testGetArtistGenres()
        {
            Console.WriteLine("\n-----------------------");
            Console.WriteLine("GET ARTIST GENRES");
            Console.WriteLine("-----------------------");
            while (true)
            {
                Console.Write("\nEnter Artist Name:\t");
                string szArtistName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(szArtistName))
                {
                    break;
                }
                string szArtistId = getArtistIdFromName(szArtistName);
                if (string.IsNullOrWhiteSpace(szArtistId))
                {
                    Console.WriteLine("ERROR: Artist not found!");
                    continue;
                }
                List<string> artistGenres = getArtistGenres(szArtistId);
                Console.WriteLine('\n' + szArtistName + " belongs to the following " + artistGenres.Count.ToString() + " genre(s):");
                for (int i = 0; i < artistGenres.Count; i++)
                {
                    Console.WriteLine(artistGenres[i]);
                }
            }
        }

        static List<string> getGenreArtists(string szGenre)
        {
            List<string> genreArtists = new List<string>();
            TERM_DB.Open();
            string szQuery = "SELECT artist_id FROM artist_term WHERE term='" + szGenre + "'";
            try
            {
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    genreArtists.Add(reader[0].ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            TERM_DB.Close();
            return genreArtists;
        }

        private static void testGetGenreArtists()
        {
            Console.WriteLine("\n-----------------------");
            Console.WriteLine("GET GENRE ARTISTS");
            Console.WriteLine("-----------------------");
            while (true)
            {
                Console.Write("\nEnter Genre:\t");
                string szGenre = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(szGenre))
                {
                    break;
                }
                List<string> genreArtists = getGenreArtists(szGenre);
                Console.WriteLine('\n' + "The genre '"  + szGenre + "' includes to the following " + genreArtists.Count.ToString() + " artist(s):");
                for (int i = 0; i < genreArtists.Count; i++)
                {
                    Console.WriteLine(getArtistNameFromId(genreArtists[i]));
                }
            }
        }

        static void Main(string[] args)
        {
            //Console.WriteLine(testArtistNameAndIdMapping());
            testGetSimilarArtists();
            testGetArtistGenres();
            testGetGenreArtists();
        }
    }
}
