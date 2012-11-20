using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Song
    {
        public string TrackId;
        public string Title;
        public string Release;
        public string Duration;
        public string Year;
        public Artist Artist;

        public Song()
        {
            TrackId = string.Empty;
            Title = string.Empty;
            Release = string.Empty;
            Duration = string.Empty;
            Year = string.Empty;
            Artist = null;
        }

        public Song(string szTrackId,
                string szTitle,
                string szRelease,
                string szDuration,
                string szYear,
                Artist artist)
        {
            TrackId = szTrackId;
            Title = szTitle;
            Release = szRelease;
            Duration = szDuration;
            Year = szYear;
            Artist = artist;
        }
    }
}
