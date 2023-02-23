using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProPresenterLyricsGenerator.Class
{

    class Parser
    {
        public string KeyWord;
        public CS_LyricsLoader.LoaderState State;
    }

    public static class CS_LyricsLoader
    {

        public enum LoaderState
        {
            eNoState,
            eFoundSongTitle,
            eFoundAuthor,
            eLookingForParagraph,
            eFoundVerse,
            eFoundBridge,
            eFoundChorus,
            eFoundFirstLine,
            eFoundSentense
        };

        static List<string> SongList = new List<string>()
        {
            "",
            "SongTitle",
            "Author",
            "Paragraph",
            "Verse",
            "Bridge",
            "Chorus",
            "FirstLine",
            "Sentense"

        };

        static List<Parser> ParserList = new List<Parser>()
        {
            new Parser(){KeyWord="sound:", State= CS_LyricsLoader.LoaderState.eFoundSongTitle },
            new Parser(){KeyWord="artist:", State= CS_LyricsLoader.LoaderState.eFoundAuthor },
            new Parser(){KeyWord="verse", State= CS_LyricsLoader.LoaderState.eFoundVerse },
            new Parser(){KeyWord="refrain", State= CS_LyricsLoader.LoaderState.eFoundChorus },
            new Parser(){KeyWord="bridge", State= CS_LyricsLoader.LoaderState.eFoundBridge },
            new Parser(){KeyWord="bridge", State= CS_LyricsLoader.LoaderState.eFoundBridge },
        };

        static LoaderState ParserState = LoaderState.eNoState;


    static LoaderState CheckLine(string txt)
    {
        Parser ps = ParserList.Find(x => x.KeyWord == txt);
        if (ps != null)
        {
            return ps.State;
        }
        else
            return LoaderState.eFoundSentense;
    }
}
}
