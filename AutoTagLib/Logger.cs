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
        private StreamWriter logFile = new StreamWriter(@"C:\Users\pierr\Documents\PY-ECP\POOA - C#\Project AutoTag\logv1.csv");
        public enum Events
        {
            LoadFromDirectory,
            ReadFromACR,
            ReadFromAPI,
            ArbitrateNewTags,
            WriteTags,
            MoveFile
        }
        private Logger()
        {
            // Create file with date and time when GUI is open
            //Open a StreamWriter


            logFile.WriteLine("Time;File;Event;Status;Result;Artist;Album;Composer;Disc;FileName;Genre;Title;Track;Year");
            
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

        public void Log(Musics music, Events events, string status,string result)
        {
            logFile.Write(DateTime.Now+";");
            logFile.Write(music.File.FileName+";");
            logFile.Write(events+";");
            logFile.Write(status+";");
            logFile.Write(result+";");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();

            foreach (PropertyInfo p in props)
            {
                foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
                {
                    if (propName.ToString() == p.Name)
                    {
                        logFile.Write(p.GetValue(music.File.TagHandler).ToString()+";");
                    }
                }
            }
            logFile.WriteLine();
        }
    }
}
