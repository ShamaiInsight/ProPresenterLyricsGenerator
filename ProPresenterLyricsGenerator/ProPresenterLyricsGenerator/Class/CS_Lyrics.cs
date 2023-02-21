using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPresenterLyricsGenerator.Class
{
    public class CS_paragraph
    {
        public string Title;
        public List<string> Lines;
    }

    public class CS_Lyrics
    {
        public string MusicTitle;
        public string Author;
        public List<CS_paragraph> Paragraphs;
    }
}
