using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Rec
    {
        public static List<Song> GeneratePlaylist(List<string> artistNames)
        {
            // names --> ids (TODO: add matcher, e.g. beatles --> The Beatles)
            Console.WriteLine("Matching Artist Names with IDs...");
            List<string> artistIds = new List<string>();
            foreach (string artistName in artistNames)
            {
                artistIds.Add(Metadata.getArtistIdFromName(artistName));
            }

            // generate set of all artist and similar artist ids
            Console.WriteLine("Getting Similar Artists...");
            HashSet<string> artistIdPool = new HashSet<string>();
            foreach (string artistId in artistIds)
            {
                List<string> similarArtists = Similarity.GetSimilarArtists(artistId);
                foreach (string similarArtist in similarArtists)
                {
                    artistIdPool.Add(similarArtist);
                }
            }

            // THIS IS THE SLOW PART!!
            // sort by hub score
            Console.WriteLine("Sorting Artists...");
            List<string> sortedArtistIds = Hits.SortByHubScore(artistIdPool.ToList());

            // get top song from each of the top 10 hubs
            Console.WriteLine("Getting Songs...");
            List<string> topSongs = new List<string>();
            for (int i = 0; i < 10 && i < sortedArtistIds.Count; i++)
            {
                topSongs.Add(Metadata.GetTopSong(sortedArtistIds[i]));
            }

            // print playlist
            Console.WriteLine("\nProject Flannel's Recommended Playlist:");
            for (int i = 0; i < topSongs.Count; i++)
            {
                Console.WriteLine("\t" + (i+1).ToString() + ".\t" + topSongs[i] + " - " + Metadata.getArtistNameFromId(sortedArtistIds[i]));
            }

            // generate output
            List<Song> playlist = new List<Song>();
            for (int i = 0; i < topSongs.Count; i++)
            {
                playlist.Add(new Song(topSongs[i],sortedArtistIds[i]));
            }

            return playlist;
        }
    }
}
