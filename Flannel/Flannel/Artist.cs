using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Artist
    {
        public string ArtistId;
        public string ArtistName;
        public double ArtistFamiliarity;
        public double ArtistHotttnesss;

        public Artist()
        {
            ArtistId = string.Empty;
            ArtistName = string.Empty;
            ArtistFamiliarity = 0;
            ArtistHotttnesss = 0;
        }

        public Artist(string szArtistId,
                string szArtistName,
                double artistFamiliarity,
                double artistHotttnesss)
        {
            ArtistId = szArtistId;
            ArtistName = szArtistName;
            ArtistFamiliarity = artistFamiliarity;
            ArtistHotttnesss = artistHotttnesss;
        }
    }
}
