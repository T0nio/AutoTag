using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3Lib;
using Mp3Lib;

namespace AutoTagLib
{
    public class Logger
    {
        private static Logger _instance;
        static readonly object instanceLock = new object();
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

        public void Log(Musics music,Events events)
        {
                        
        }
    }
}
