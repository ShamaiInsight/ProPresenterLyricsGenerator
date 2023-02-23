using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPresenterLyricsGenerator.Class
{
    public class CS_LyricsSection
    {
        public string Title="";
        public List<string> Lines;

        public CS_LyricsSection()
        {
            Lines = new List<string>();
        }
    }

    public class CS_Lyrics
    {
        public string MusicTitle="";
        public string Author="";
        public List<CS_LyricsSection> Sections = new List<CS_LyricsSection>();
    }
}
