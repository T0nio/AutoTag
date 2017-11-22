using System;
using System.Collections.Generic;
using Id3Lib;
using System.IO;
using System.Reflection;

namespace AutoTagLib
{
    public class Logger
    {
        #region Properties

        private static Logger _instance;
        static readonly object instanceLock = new object();
        private StreamWriter logFile;
        private string pathLog = $"logs{Path.DirectorySeparatorChar}Running_{DateTime.Now.ToString("yyyyMMddHHmm")}.csv";

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
            Directory.CreateDirectory(Path.GetDirectoryName(pathLog));
            logFile = new StreamWriter(pathLog) { AutoFlush = true };
            logFile.WriteLine("Time;Event;Result;Path;File;Album;Artist;Composer;Disc;Genre;Title;Track;Year"); //Initialize the csv file        
        }

        #endregion

        #region Instance

        public static Logger Instance
        {
            get
            {
                if (_instance == null) // Les locks prennent du temps, il est préférable de vérifier d'abord la nullité de l'instance.
                {
                    lock (instanceLock)
                    {
                        if (_instance == null) // on vérifie encore, au cas où l'instance aurait été créée entretemps.
                            _instance = new Logger();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// log when music file is loaded from directory
        /// </summary>
        /// <param name="music">Music [object]</param>
        /// <param name="exception">Exception.toString or empty string</param>
        public void LoadFromDirectoryLog(Musics music)
        {
            logFile.Write($"{DateTime.Now};");
            logFile.Write($"{Events.LoadFromDirectory};");
            logFile.Write($"Success;");
            logFile.Write($"{Path.GetDirectoryName(music.MusicFile.FileName)};");
            logFile.Write($"{Path.GetFileName(music.MusicFile.FileName)};");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();
            string prop = string.Empty;
            foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
            {
                foreach (PropertyInfo p in props)
                {
                    if (propName.ToString() == p.Name)
                    {
                        prop = p.GetValue(music.OriginalTags).ToString();
                        prop = prop.Replace(";", ",");
                        prop = prop.Replace("\"", "'");
                        logFile.Write($"=\"{prop}\";");
                    }
                }
            }
            logFile.WriteLine();
        }

        /// <summary>
        /// log tags coming from ACR
        /// </summary>
        /// <param name="music">Music [object]</param>
        /// <param name="statusCode">Status code of the API message</param>
        /// <param name="metadata">Metadata from ACR</param>
        public void ReadfromACRLog(Musics music, int statusCode,string metadata)
        {
            logFile.Write($"{DateTime.Now};");
            logFile.Write($"{Events.ReadFromACR};");
            logFile.Write($"{statusCode} - {metadata};");
            logFile.Write($"{Path.GetDirectoryName(music.MusicFile.FileName)};");
            logFile.Write($"{Path.GetFileName(music.MusicFile.FileName)};");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();
            string prop = string.Empty;
            foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
            {
                foreach (PropertyInfo p in props)
                {
                    if (propName.ToString() == p.Name)
                    {
                        prop = p.GetValue(music.AcrTags).ToString();
                        prop = prop.Replace(";", ",");
                        prop = prop.Replace("\"", "'");
                        logFile.Write($"=\"{prop}\";");
                    }
                }
            }
            logFile.WriteLine();
        }

        /// <summary>
        /// log tags coming from API
        /// </summary>
        /// <param name="music">Music [object]</param>
        /// <param name="metadata">Metadata from API</param>
        public void ReadfromAPILog(Musics music,string metadata)
        {
            logFile.Write($"{DateTime.Now};");
            logFile.Write($"{Events.ReadFromAPI};");
            logFile.Write($"{metadata};");
            logFile.Write($"{Path.GetDirectoryName(music.MusicFile.FileName)};");
            logFile.Write($"{Path.GetFileName(music.MusicFile.FileName)};");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();
            string prop = string.Empty;
            foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
            {
                foreach (PropertyInfo p in props)
                {
                    if (propName.ToString() == p.Name)
                    {
                        prop = p.GetValue(music.ApiTags).ToString();
                        prop = prop.Replace(";", ",");
                        prop = prop.Replace("\"", "'");
                        logFile.Write($"=\"{prop}\";");
                    }
                }
            }
            logFile.WriteLine();
        }

        /// <summary>
        /// log tags that are choosen from old, ACR and API tags
        /// </summary>
        /// <param name="music">Music [object]</param>
        /// <param name="compareTags">list with all changed tags</param>
        public void ArbitrateNewTagsLog(Musics music,List<string> compareTags)
        {
            logFile.Write($"{DateTime.Now};");
            logFile.Write($"{Events.ArbitrateNewTags};");
            logFile.Write($"=\"");
            foreach (string change in compareTags)
            {
                logFile.Write($"-{change}-");
            }
            logFile.Write($"\";");
            logFile.Write($"{Path.GetDirectoryName(music.MusicFile.FileName)};");
            logFile.Write($"{Path.GetFileName(music.MusicFile.FileName)};");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();
            string prop = string.Empty;
            foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
            {
                foreach (PropertyInfo p in props)
                {
                    if (propName.ToString() == p.Name)
                    {
                        prop = p.GetValue(music.NewTags).ToString();
                        prop = prop.Replace(";", ",");
                        prop = prop.Replace("\"", "'");
                        logFile.Write($"=\"{prop}\";");
                    }
                }
            }
            logFile.WriteLine();
        }

        /// <summary>
        /// log when tags are written in the file
        /// </summary>
        /// <param name="music">Music [object]</param>
        /// <param name="exception">exception caught when update the file</param>
        public void WriteTagsLog(Musics music,string exception)
        {
            logFile.Write($"{DateTime.Now};");
            logFile.Write($"{Events.WriteTags};");
            logFile.Write($"{exception};");
            logFile.Write($"{Path.GetDirectoryName(music.MusicFile.FileName)};");
            logFile.Write($"{Path.GetFileName(music.MusicFile.FileName)};");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();
            string prop = string.Empty;
            foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
            {
                foreach (PropertyInfo p in props)
                {
                    if (propName.ToString() == p.Name)
                    {
                        try
                        {
                            prop = p.GetValue(music.MusicFile.TagHandler).ToString();
                        } catch (Exception)
                        {
                            prop = string.Empty;
                        }
                        prop = prop.Replace(";", ",");
                        prop = prop.Replace("\"", "'");
                        logFile.Write($"=\"{prop}\";");
                    }
                }
            }
            logFile.WriteLine();
        }

        /// <summary>
        /// log file when copied
        /// </summary>
        /// <param name="music">Music [object]</param>
        /// <param name="newPath">new path of the file with new name</param>
        public void CopyFileLog(Musics music,string oldPath)
        {
            logFile.Write($"{DateTime.Now};");
            logFile.Write($"{Events.CopyFile};");
            logFile.Write($"{oldPath};");
            logFile.Write($"{Path.GetDirectoryName(music.MusicFile.FileName)};");
            logFile.Write($"{Path.GetFileName(music.MusicFile.FileName)};");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();
            string prop = string.Empty;
            foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
            {
                foreach (PropertyInfo p in props)
                {
                    if (propName.ToString() == p.Name)
                    {
                        prop = p.GetValue(music.MusicFile.TagHandler).ToString();
                        prop = prop.Replace(";", ",");
                        prop = prop.Replace("\"", "'");
                        logFile.Write($"=\"{prop}\";");
                    }
                }
            }
            logFile.WriteLine();
        }

        /// <summary>
        /// log file when moved
        /// </summary>
        /// <param name="music">Music [object]</param>
        /// <param name="newPath">new path of the file with new name</param>
        public void MoveFileLog(Musics music, string oldPath)
        {
            logFile.Write($"{DateTime.Now};");
            logFile.Write($"{Events.MoveFile};");
            logFile.Write($"{oldPath};");
            logFile.Write($"{Path.GetDirectoryName(music.MusicFile.FileName)};");
            logFile.Write($"{Path.GetFileName(music.MusicFile.FileName)};");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();
            string prop = string.Empty;
            foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
            {
                foreach (PropertyInfo p in props)
                {
                    if (propName.ToString() == p.Name)
                    {
                        prop = p.GetValue(music.MusicFile.TagHandler).ToString();
                        prop = prop.Replace(";", ",");
                        prop = prop.Replace("\"", "'");
                        logFile.Write($"=\"{prop}\";");
                    }
                }
            }
            logFile.WriteLine();
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