using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProPresenterLyricsGenerator.Class;

namespace ProPresenterLyricsGenerator
{
    public partial class CS_FormMain : Form
    {

        CS_Lyrics m_Lyrics;

        public CS_FormMain()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Clear();
                treeView1.Nodes.Clear();
                richTextBox1.AppendText(File.ReadAllText(openFileDialog.FileName));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "")
            {
                foreach(string line in richTextBox1.Lines)
                {

                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        enum ParserState
        {
            eLookingForSong,
            eLookingForArtis,
            eLookingForLyricsSection,
            eLookingForLyricsSectionLines,
            eLookingForLyricsEnd
        }

        class ErroList
        {
            public string Line;
            public string Message;
            public int LineNumber;
        }
        List<ErroList> Errors = new List<ErroList>();
        List<CS_Lyrics> FileDataLyrics = new List<CS_Lyrics>();
        private void autoCorrectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ParserState State= ParserState.eLookingForSong;
            richTextBox2.Clear();
            int lineNumber = 0;
            CS_Lyrics CurrentLyrics = null;
            CS_LyricsSection CurrentLyricsSection = null;
            try
            {
                foreach (string cline in richTextBox1.Lines)
                {
                    lineNumber++;
                    string dataline = cline.Trim();
                    switch (State)
                    {
                        case ParserState.eLookingForSong:
                            {
                                if (CurrentLyrics == null)
                                {
                                    if ((dataline.ToLower().IndexOf("song") >= 0)  && (dataline.ToLower().IndexOf(":") > 0))
                                    {
                                        FileDataLyrics.Add(new CS_Lyrics() { MusicTitle = dataline.Substring(dataline.ToLower().IndexOf(":") + 1).Trim() });
                                        CurrentLyrics = FileDataLyrics[FileDataLyrics.Count - 1];
                                        State = ParserState.eLookingForArtis;
                                    }
                                    else if ( (dataline.ToLower().IndexOf("artist") >= 0)   && (dataline.ToLower().IndexOf(":") > 0))
                                    {
                                        FileDataLyrics.Add(new CS_Lyrics() { Author = dataline.Substring(dataline.ToLower().IndexOf(":") + 1).Trim() });
                                        CurrentLyrics = FileDataLyrics[FileDataLyrics.Count - 1];
                                        Errors.Add(new ErroList() { Line = dataline, LineNumber = lineNumber, Message = "Not expected when looking for song name" });
                                        State = ParserState.eLookingForLyricsSection;
                                    }
                                    else if (dataline != "")
                                    {
                                        Errors.Add(new ErroList() { Line = dataline, LineNumber = lineNumber, Message = "Not expected when looking for song name" });
                                    }
                                }
                                else
                                {
                                    Errors.Add(new ErroList() { Line = dataline, LineNumber = lineNumber, Message = "Last Lyrics error" });
                                    CurrentLyrics = null;
                                }
                            }
                            break;
                        case ParserState.eLookingForArtis:
                            {
                                if (CurrentLyrics != null)
                                {

                                    if ( (dataline.ToLower().IndexOf("artist") >= 0) && (dataline.ToLower().IndexOf(":") > 0))
                                    {
                                        if (CurrentLyrics.Author != "")
                                            Errors.Add(new ErroList() { Line = dataline, LineNumber = lineNumber, Message = "Artist defined too many times" });
                                        else if (CurrentLyrics.MusicTitle == "")
                                            Errors.Add(new ErroList() { Line = dataline, LineNumber = lineNumber, Message = "Song title not defined" });

                                        CurrentLyrics.Author = dataline.Substring(dataline.ToLower().IndexOf(":") + 1).Trim();
                                        State = ParserState.eLookingForLyricsSection;
                                    }
                                    else if (dataline != "")
                                    {
                                        Errors.Add(new ErroList() { Line = dataline, LineNumber = lineNumber, Message = "Not expected when looking for Artist" });
                                    }
                                }
                                else
                                {

                                }
                            }
                            break;

                        case ParserState.eLookingForLyricsSection:
                            {
                                if (dataline.ToLower().IndexOf("end of song") >= 0)
                                {
                                    State = ParserState.eLookingForLyricsEnd;
                                }
                                else if (dataline.ToLower().IndexOf("verse") >= 0)
                                {
                                    CurrentLyrics.Sections.Add(new CS_LyricsSection() { });
                                    CurrentLyricsSection = CurrentLyrics.Sections[CurrentLyrics.Sections.Count - 1];
                                    CurrentLyricsSection.Title = dataline.Substring(dataline.ToLower().IndexOf("verse") + 5);
                                    State = ParserState.eLookingForLyricsSectionLines;
                                }
                                else if (dataline.ToLower().IndexOf("chorus") >= 0)
                                {
                                    CurrentLyrics.Sections.Add(new CS_LyricsSection() { });
                                    CurrentLyricsSection = CurrentLyrics.Sections[CurrentLyrics.Sections.Count - 1];
                                    CurrentLyricsSection.Title = dataline.Substring(dataline.ToLower().IndexOf("chorus") + 5);
                                    State = ParserState.eLookingForLyricsSectionLines;
                                }
                                else if (dataline.Length > 4)
                                {
                                    CurrentLyrics.Sections.Add(new CS_LyricsSection() { });
                                    CurrentLyricsSection = CurrentLyrics.Sections[CurrentLyrics.Sections.Count - 1];
                                    CurrentLyricsSection.Title = dataline;
                                    State = ParserState.eLookingForLyricsSectionLines;
                                }
                            }
                            break;

                        case ParserState.eLookingForLyricsSectionLines:
                            {
                                if (dataline != "")
                                {
                                    if (dataline.ToLower().IndexOf("end of song") >= 0)
                                    {
                                        Errors.Add(new ErroList() { Line = dataline, LineNumber = lineNumber, Message = "Missing break Line before End of Song" });

                                        if ((dataline.ToLower() == "end of song") || (dataline.ToLower() == "end of song."))
                                        {
                                            foreach (ErroList erroList in Errors)
                                            {
                                                richTextBox2.AppendText("Line : " + erroList.LineNumber + " >> " + erroList.Message + ":" + erroList.Line + "\n");
                                            }
                                            Errors.Clear();
                                            if (CurrentLyrics.MusicTitle != null)
                                                treeView1.Nodes.Add(CurrentLyrics.MusicTitle.Trim());
                                            State = ParserState.eLookingForSong;
                                            CurrentLyrics = null;
                                        }
                                        else
                                            CurrentLyricsSection.Lines.Add(dataline);
                                    }
                                    else
                                        CurrentLyricsSection.Lines.Add(dataline);
                                }
                                else
                                {

                                    State = ParserState.eLookingForLyricsSection;
                                }
                            }
                            break;

                        case ParserState.eLookingForLyricsEnd:
                            {
                                foreach (ErroList erroList in Errors)
                                {
                                    richTextBox2.AppendText("Line : "+ erroList.LineNumber+" >> "+erroList.Message+":"+erroList.Line+"\n");
                                }
                                Errors.Clear();
                                if (CurrentLyrics.MusicTitle != null)
                                treeView1.Nodes.Add(CurrentLyrics.MusicTitle.Trim());
                                State = ParserState.eLookingForSong;
                                CurrentLyrics = null;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                richTextBox2.AppendText("ERROR");
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            foreach(CS_Lyrics cS_Lyrics in FileDataLyrics)
            {
                if (e.Node.Text.ToLower() == cS_Lyrics.MusicTitle.ToLower())
                {
                    richTextBox1.Clear();
                    richTextBox1.AppendText(cS_Lyrics.MusicTitle+"\n");
                    richTextBox1.AppendText(cS_Lyrics.Author + "\n");

                    foreach(CS_LyricsSection cS_LyricsSection in cS_Lyrics.Sections)
                    {
                        richTextBox1.AppendText(((CS_LyricsSection)cS_LyricsSection).Title+"\n");
                        foreach(string line in cS_LyricsSection.Lines)
                            richTextBox1.AppendText((line).Trim()+"\n");
                    }
                }
            }
        }
    }

}
