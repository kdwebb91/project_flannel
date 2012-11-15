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
        private static string METADATA_DB_LOC = "Data Source=subset_track_metadata.db;Version=3;";
        private static string TERM_DB_LOC = "Data Source=subset_artist_term.db;Version=3;";
        private static string SIMILARITY_DB_LOC = "Data Source=subset_artist_similarity.db;Version=3;";

        private static SQLiteConnection METADATA_DB = new SQLiteConnection(METADATA_DB_LOC);
        private static SQLiteConnection TERM_DB = new SQLiteConnection(TERM_DB_LOC);
        private static SQLiteConnection SIMILARITY_DB = new SQLiteConnection(SIMILARITY_DB_LOC);

        /* http://en.wikipedia.org/wiki/List_of_popular_music_genres */
        private static List<string> popularMusicGenres = new List<string>()
        {
            "african",
            "asian",
            "avant-garde",
            "blues",
            "brazilian",
            "comedy",
            "country",
            "easy listening",
            "electronic",
            "modern folk",
            "hip hop",
            "jazz",
            "latin american",
            "pop",
            "r&b",
            "rock",
            "ska"
        };

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
            try
            {
                /* TERM_BB is the location of the SQLite .db file */
                TERM_DB.Open();
                string szQuery = "SELECT term FROM artist_term WHERE artist_id='" + szArtistId + "'";
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artistGenres.Add(reader[0].ToString());
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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

        private static int getNumberOfArtists()
        {
            int numberOfArtists = 0;
            try
            {
                TERM_DB.Open();
                string szQuery = "SELECT COUNT(artist_id) FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    numberOfArtists = Convert.ToInt32(reader[0].ToString());
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return numberOfArtists;
        }

        private static int getNumberOfGenres()
        {
            int numberOfGenres = 0;
            try
            {
                TERM_DB.Open();
                string szQuery = "SELECT COUNT(term) FROM terms";
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    numberOfGenres = Convert.ToInt32(reader[0].ToString());
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return numberOfGenres;
        }

        private static int getNumberOfMainGenres()
        {
            int numberOfGenres = getNumberOfGenres();
            try
            {
                TERM_DB.Open();
                List<string> genres = new List<string>();
                string szQuery = "SELECT term FROM terms";
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    genres.Add(reader["term"].ToString());
                }
                for (int i = 0; i < genres.Count; i++)
                {
                    int genLen = genres[i].Length;
                    szQuery = "SELECT COUNT(term) FROM terms WHERE SUBSTR(term,1," + genLen + ")='" + genres[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, TERM_DB);
                        reader = command.ExecuteReader();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Genre " + genres[i] + " threw an exception.");
                        continue;
                    }
                    if (reader.Read())
                    {
                        int count = Convert.ToInt32(reader[0].ToString());
                        numberOfGenres -= (count - 1);
                    }
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return numberOfGenres;
        }

        private static double getGenresPerArtist()
        {
            int genresPerArtist = 0;
            try
            {
                TERM_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                for (int i = 0; i < artists.Count; i++)
                {
                    int genLen = artists[i].Length;
                    szQuery = "SELECT COUNT(term) FROM artist_term WHERE artist_id='" + artists[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, TERM_DB);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            genresPerArtist += Convert.ToInt32(reader[0].ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Artist " + artists[i] + " threw an exception.");
                        continue;
                    }
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return Convert.ToDouble(genresPerArtist) / getNumberOfArtists();
        }

        private static double getArtistsPerGenre()
        {
            int artistsPerGenre = 0;
            try
            {
                TERM_DB.Open();
                List<string> genres = new List<string>();
                string szQuery = "SELECT term FROM terms";
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    genres.Add(reader["term"].ToString());
                }
                for (int i = 0; i < genres.Count; i++)
                {
                    int genLen = genres[i].Length;
                    szQuery = "SELECT COUNT(artist_id) FROM artist_term WHERE term='" + genres[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, TERM_DB);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            artistsPerGenre += Convert.ToInt32(reader[0].ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Genre " + genres[i] + " threw an exception.");
                        continue;
                    }
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return Convert.ToDouble(artistsPerGenre) / getNumberOfGenres();
        }

        private static double getAverageNumberOfSimilarArtists()
        {
            double avg = 0;
            try
            {
                SIMILARITY_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, SIMILARITY_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                for (int i = 0; i < artists.Count; i++)
                {
                    szQuery = "SELECT COUNT(similar) FROM similarity WHERE target='" + artists[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, SIMILARITY_DB);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            avg += Convert.ToInt32(reader[0].ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Artist " + artists[i] + " threw an exception.");
                        continue;
                    }
                }
                SIMILARITY_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return avg / getNumberOfArtists();
        }

        private static void getMostFamiliarArtist()
        {
            List<double> fam = new List<double>() { 0,0,0 };
            List<string> artist = new List<string>() { "", "", "" };
            try
            {
                SIMILARITY_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, SIMILARITY_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                SIMILARITY_DB.Close();
                METADATA_DB.Open();
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
            for (int i = fam.Count-1; i >= 0; i--)
            {
                Console.WriteLine(getArtistNameFromId(artist[i]));
                Console.WriteLine(fam[i]);
            }
        }

        private static void getLeastFamiliarArtist()
        {
            List<double> fam = new List<double>() { 1, 1, 1 };
            List<string> artist = new List<string>() { "", "", "" };
            try
            {
                SIMILARITY_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, SIMILARITY_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                SIMILARITY_DB.Close();
                METADATA_DB.Open();
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
                            if (temp < fam[fam.Count-1])
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

        private static Dictionary<string, int> getData()
        {
            Dictionary<string, int> data = new Dictionary<string, int>();
            try
            {
                TERM_DB.Open();
                for (int i = 0; i < popularMusicGenres.Count; i++)
                {
                    string szQuery = "SELECT COUNT(artist_id) FROM artist_term WHERE term='" + popularMusicGenres[i] + "'";
                    try
                    {
                        SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                        SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            data.Add(popularMusicGenres[i], Convert.ToInt32(reader[0].ToString()));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Genre " + popularMusicGenres[i] + " threw an exception.");
                        continue;
                    }
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return data;
        }

        private static int getNumberOfSongs(string szArtistId)
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

        private static double getNumberOfSongsPerArtist()
        {
            double avgNumSongs = 0;
            try
            {
                TERM_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
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
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return avgNumSongs;
        }

        static void Main(string[] args)
        {
            //Console.WriteLine(testArtistNameAndIdMapping());
            //testGetSimilarArtists();
            //testGetArtistGenres();
            //testGetGenreArtists();
            /*
            Console.WriteLine("Number of Artists:\t" + getNumberOfArtists());
            Console.WriteLine("Number of Genres:\t" + getNumberOfGenres());
            Console.WriteLine("Number of Main Genres:\t" + getNumberOfMainGenres());
            Console.WriteLine("Number of Genres per Artist:\t" + getGenresPerArtist());
            Console.WriteLine("Number of Artists per Genre:\t" + getArtistsPerGenre());
            Console.WriteLine("Number of Similar Artists per Artist:\t" + getAverageNumberOfSimilarArtists());
            getMostSimilarArtist();
            getLeastSimilarArtist();
            Dictionary<string,int> data = getData();
            for (int i = 0; i < popularMusicGenres.Count; i++)
            {
                if (data.ContainsKey(popularMusicGenres[i]))
                {
                    Console.WriteLine(popularMusicGenres[i] + " : " + data[popularMusicGenres[i]]);
                }
            }
            */
            Console.WriteLine(getNumberOfSongsPerArtist());
            getMostFamiliarArtist();
            getLeastFamiliarArtist();
            Console.Read();
        }
    }
}
