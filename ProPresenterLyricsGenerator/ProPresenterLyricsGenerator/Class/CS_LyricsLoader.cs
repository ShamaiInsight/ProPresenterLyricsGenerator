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

    public  static class CS_LyricsLoader
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

        public static List<CS_Lyrics> ExtractLyrics(string[] Lines)
        {
            List<CS_Lyrics> cslist = new List<CS_Lyrics>();

            foreach (string line in Lines)
            {
                switch(ParserState)
                {
                    case LoaderState.eNoState:
                        // If something is founded
                        if (line.Trim() != "")
                        {
                            ParserState = CheckLine(line);
                            if (ParserState == LoaderState.eFoundSongTitle)
                            {
                                cslist.Add(new CS_Lyrics() { MusicTitle=line});
                            }
                        }
                        break;
                    case LoaderState.eFoundSongTitle:
                        // If something is founded
                        if (line.Trim() != "")
                        {
                            ParserState = CheckLine(line);
                            if (ParserState == LoaderState.eFoundAuthor)
                            {
                                if (cslist == null)
                                cslist.Add(new CS_Lyrics() { Author = line });
                                else 
                                    cslist[cslist.Count - 1].Author = line;
                            }
                            else if (ParserState == LoaderState.eFoundVerse)
                            {
                                if (cslist == null)
                                    cslist.Add(new CS_Lyrics() { MusicTitle = "---", Author = "---" });

                                    cslist[cslist.Count - 1].Paragraphs = new List<CS_paragraph>();
                            }
                        }
                        break;
                }
            }

            return cslist;
        }

        static LoaderState CheckLine(string txt)
        {
            Parser ps = ParserList.Find(x=>x.KeyWord == txt);
            if (ps != null)
            {
                return ps.State;
            }
            else
                return LoaderState.eFoundSentense;
        }
    }
}
