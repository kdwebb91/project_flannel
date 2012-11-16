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
        /* http://en.wikipedia.org/wiki/List_of_popular_music_genres */
        public static List<string> popularMusicGenres = new List<string>()
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

        static void Main(string[] args)
        {
        }
    }
}
