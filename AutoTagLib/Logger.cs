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
#region Properties
        private static Logger _instance;
        static readonly object instanceLock = new object();
        private StreamWriter logFile = new StreamWriter($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}" +
                                    $"logs{Path.DirectorySeparatorChar}{DateTime.Now.ToString("yyyyMMddHHmm")}.csv");
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
#endregion
#region Constructor
        private Logger()
        {
            logFile.WriteLine("Event;Status;Time;Path;File;Album;Artist;Composer;Disc;Genre;Title;Track;Year;New Path"); //Initialize the csv file        
        }
#endregion
#region Instance
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
#endregion
#region Methods
        /// <summary>
        /// Common logs for music files
        /// </summary>
        /// <param name="music">Music [Object]</param>
        private void MusicCommonLog(Musics music)
        {
            logFile.Write("Success;");
            logFile.Write($"{DateTime.Now};");
            logFile.Write($"{Path.GetDirectoryName(music.MusicFile.FileName)};");
            logFile.Write($"{Path.GetFileName(music.MusicFile.FileName)};");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();
            foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
            {
                foreach (PropertyInfo p in props)
                {
                    if (propName.ToString() == p.Name)
                    {
                        logFile.Write($"=\"{p.GetValue(music.MusicFile.TagHandler).ToString()}\";");
                    }
                }
            }
        }
        /// <summary>
        /// log when music file is loaded from directory
        /// </summary>
        /// <param name="music">Music [object]</param>
        public void LoadFromDirectoryLog(Musics music)
        {
            logFile.Write($"{Events.LoadFromDirectory};");
            MusicCommonLog(music);
            logFile.WriteLine(";");
        }
        /// <summary>
        /// log tags coming from ACR
        /// </summary>
        /// <param name="music">Music [object]</param>
        public void ReadfromACRLog(Musics music)
        {
            logFile.Write($"{Events.ReadFromACR};");
            MusicCommonLog(music);
            // Add information from ACR ?
            logFile.WriteLine(";");
        }
        /// <summary>
        /// log tags coming from API
        /// </summary>
        /// <param name="music">Music [object]</param>
        public void ReadfromAPILog(Musics music)
        {
            logFile.Write($"{Events.ReadFromAPI};");
            MusicCommonLog(music);
            logFile.WriteLine(";");

        }
        /// <summary>
        /// log tags that are choosen from old, ACR and API tags
        /// </summary>
        /// <param name="music">Music [object]</param>
        public void ArbitrateNewTagsLog(Musics music)
        {
            logFile.Write($"{Events.ArbitrateNewTags};");
            MusicCommonLog(music);
            logFile.WriteLine(";");
        }
        /// <summary>
        /// log when tags are written in the file
        /// </summary>
        /// <param name="music">Music [object]</param>
        public void WriteTagsLog(Musics music)
        {
            logFile.Write($"{Events.WriteTags};");
            MusicCommonLog(music);
            logFile.WriteLine(";");
        }
        /// <summary>
        /// log file when copied
        /// </summary>
        /// <param name="music">Music [object]</param>
        /// <param name="newPath">new path of the file with new name</param>
        public void CopyFileLog(Musics music,string newPath)
        {
            logFile.Write($"{Events.CopyFile};");
            MusicCommonLog(music);
            logFile.WriteLine($"{newPath};");
        }
        /// <summary>
        /// log file when moved
        /// </summary>
        /// <param name="music">Music [object]</param>
        /// <param name="newPath">new path of the file with new name</param>
        public void MoveFileLog(Musics music, string newPath)
        {
            logFile.Write($"{Events.MoveFile};");
            MusicCommonLog(music);
            logFile.WriteLine($"{newPath};");
        }
        /// <summary>
        /// Close the log when app is closed
        /// </summary>
        public void Closelog()
        {
            logFile.Close();
        }
#endregion
    }
}