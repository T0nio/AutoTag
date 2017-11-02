using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3Lib;
using Mp3Lib;
using System.IO;
using System.Reflection;

namespace AutoTagLib
{
    public class Logger
    {
        private static Logger _instance;
        static readonly object instanceLock = new object();
        private StreamWriter logFile = new StreamWriter(@"C:\Users\pierr\Documents\PY-ECP\POOA - C#\Project AutoTag\Autotag\logs\log"+DateTime.Now.Day+".csv");
        public enum Events
        {
            LoadFromDirectory,
            ReadFromACR,
            ReadFromAPI,
            ArbitrateNewTags,
            WriteTags,
            MoveFile,
            CopyFile
        }
        private Logger()
        {
            // Create file with date and time when GUI is open
            //Open a StreamWriter


            logFile.WriteLine("Event;Status;Result;Time;Path;File;Album;Artist;Composer;Disc;Genre;Title;Track;Year");
            
        }

        public static Logger Instance
        {
            get
            {
                if (_instance == null) //Les locks prennent du temps, il est préférable de vérifier d'abord la nullité de l'instance.
                {
                    lock (instanceLock)
                    {
                        if (_instance == null) //on vérifie encore, au cas où l'instance aurait été créée entretemps.
                            _instance = new Logger();
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// Common logs between music files
        /// </summary>
        /// <param name="music">Music [Object]</param>
        /// <param name="musicfile">Is music or folder ?</param>
        private void MusicCommonLog(Musics music)
        {
            logFile.Write(DateTime.Now+";");
            logFile.Write(Path.GetDirectoryName(music.MusicFile.FileName)+";");
            logFile.Write(Path.GetFileName(music.MusicFile.FileName) + ";");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();

            foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
            {
                foreach (PropertyInfo p in props)
                {
                    if (propName.ToString() == p.Name)
                    {
                        logFile.Write(p.GetValue(music.MusicFile.TagHandler).ToString() + ";");
                    }
                }
            }

            logFile.WriteLine();
        }

        public void LoadFromDirectoryLog(Musics music)
        {
            logFile.Write(Events.LoadFromDirectory + ";");
            logFile.Write(";");
            logFile.Write(";");

            MusicCommonLog(music);
        }
        public void ReadfromACRLog(Musics music)
        {
            logFile.Write(Events.ReadFromACR + ";");
            logFile.Write(";");
            logFile.Write(";");

            MusicCommonLog(music);
        }
        public void ReadfromAPILog(Musics music)
        {
            logFile.Write(Events.ReadFromAPI + ";");
            logFile.Write(";");
            logFile.Write(";");

            MusicCommonLog(music);
        }
        public void WriteTagsLog(Musics music)
        {
            logFile.Write(Events.WriteTags + ";");
            logFile.Write(";");
            logFile.Write(";");

            MusicCommonLog(music);
        }
        public void ArbitrateNewTagsLog(Musics music)
        {
            logFile.Write(Events.ArbitrateNewTags + ";");
            logFile.Write(";");
            logFile.Write(";");

            MusicCommonLog(music);
        }
        public void CopyFileLog(Musics music)
        {
            logFile.Write(Events.CopyFile + ";");
            logFile.Write(";");
            logFile.Write(";");

            MusicCommonLog(music);
        }
        public void MoveFileLog(Musics music)
        {
            logFile.Write(Events.MoveFile + ";");
            logFile.Write(";");
            logFile.Write(";");

            MusicCommonLog(music);
        }

        public void Closelog()
        {
            logFile.Close();
        }
    }
}
