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
        public static List<string> Artists = new List<string>()
        {
            "Incubus",
            "Aerosmith",
            "Stone Temple Pilots",
            "Foo Fighters",
            "The Smashing Pumpkins",
            "Nirvana",
            "Mastodon",
            "Red Hot Chili Peppers",
            "The Who",
            "Metallica"
        };

        /* http://en.wikipedia.org/wiki/List_of_popular_music_genres */
        public static List<string> PopularMusicGenres = new List<string>()
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

        public static bool test_FindArtistInDB(List<string> artists)
        {
            bool allArtistsFound = true;
            for (int i = 0; i < artists.Count; i++)
            {
                string artist_id = Metadata.getArtistIdFromName(artists[i]);
                if (string.Equals(artist_id, string.Empty))
                {
                    allArtistsFound = false;
                    Console.Write("ERROR artist not found:\t" + artists[i]);
                }
            }
            return allArtistsFound;
        }

        public static void printHITS(List<string> artists)
        {
            Dictionary<double, string> hubs = new Dictionary<double, string>();
            Dictionary<double, string> auths = new Dictionary<double, string>();

            List<double> hubScores = new List<double>();
            List<double> authScores = new List<double>();

            for (int i = 0; i < Artists.Count; i++)
            {
                double hubScore = Hits.GetHubScore(Metadata.getArtistIdFromName(Artists[i]));
                double authScore = Hits.GetAuthScore(Metadata.getArtistIdFromName(Artists[i]));
                hubScores.Add(hubScore);
                authScores.Add(authScore);
                hubs[hubScore] = Artists[i];
                auths[authScore] = Artists[i];
            }

            Console.WriteLine("\nHUBS:\n");
            hubScores.Sort();
            for (int i = hubScores.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(hubs[hubScores[i]] + " (" + hubScores[i] + ")");
            }
            Console.WriteLine("\nAUTHS:\n");
            authScores.Sort();
            for (int i = authScores.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(auths[authScores[i]] + " (" + authScores[i] + ")");
            }
        }

        public static void printFamiliarities(List<string> artists)
        {
            Dictionary<double, string> fams = new Dictionary<double, string>();
            
            List<double> famValues = new List<double>();

            for (int i = 0; i < Artists.Count; i++)
            {
                double famValue = Metadata.GetArtistFamiliarity(Metadata.getArtistIdFromName(Artists[i]));
                famValues.Add(famValue);
                fams[famValue] = Artists[i];
            }

            Console.WriteLine("\nFamiliarity:\n");
            famValues.Sort();
            for (int i = famValues.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(fams[famValues[i]] + " (" + famValues[i] + ")");
            }
        }

        public static void printHotttnessses(List<string> artists)
        {
            Dictionary<double, string> hotttnessses = new Dictionary<double, string>();

            List<double> hotttnesssValues = new List<double>();

            for (int i = 0; i < Artists.Count; i++)
            {
                double hotttnesss = Metadata.GetArtistHotttness(Metadata.getArtistIdFromName(Artists[i]));
                hotttnesssValues.Add(hotttnesss);
                hotttnessses[hotttnesss] = Artists[i];
            }
            
            Console.WriteLine("\nHotttnesss:\n");
            hotttnesssValues.Sort();
            for (int i = hotttnesssValues.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(hotttnessses[hotttnesssValues[i]] + " (" + hotttnesssValues[i] + ")");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine(test_FindArtistInDB(Artists));
            printHITS(Artists);
            printFamiliarities(Artists);
            printHotttnessses(Artists);
            Metadata.GetSongsFromArtist(Metadata.getArtistIdFromName(Artists[0]));
            Console.Read();
        }
    }
}
