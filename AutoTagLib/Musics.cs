using Id3Lib;
using Mp3Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MusicInfoLib;
using System.IO;
using MinimumEditDistance;

namespace AutoTagLib
{
    public class Musics
    {

        #region Properties

        public TagHandler OriginalTags { get; }
        public TagHandler AcrTags { get; }
        public TagHandler ApiTags { get; }
        public TagHandler NewTags { get; }
        public Mp3File MusicFile { get; }
        public string FileName {
            get
            {
                return MusicFile.FileName.Split(Path.DirectorySeparatorChar).Last();
            }
        }
        private List<char> illegalCharFromFileName = new List<char>(){ '/', '\\', ':', '*', '?', '"', '<', '>', '|'};
        private string illegalCharReplacor = "-";

        public enum PropertiesForUser
        {
            Album,
            Artist,
            Composer,
            Disc,
            Genre,
            Title,
            Track,
            Year
        }

        #endregion

        #region Constructor

        public Musics(string path)
        {
            
            MusicFile = new Mp3File(path);
            try
            {
                OriginalTags = MusicFile.TagHandler;
                AcrTags = new Mp3File(path).TagHandler;
                ApiTags = new Mp3File(path).TagHandler;
                NewTags = new Mp3File(path).TagHandler;
            }
            catch (NotImplementedException e)
            {
                OriginalTags = new TagHandler(new TagModel());
                AcrTags = new TagHandler(new TagModel());
                ApiTags = new TagHandler(new TagModel());
                NewTags = new TagHandler(new TagModel());
            }

        }
        #endregion

        #region Methods

        public void ReadTags()
        {
            ReadTagFromACR();
            ReadTagFromAPI();
            ArbitrateNewTags();
        }
        
        public void ReadTagFromACR()
        {
            
            ACRCloudJsonObject infosFromACR = Infos.Recognize(MusicFile.FileName);

            if (infosFromACR.status.code == 0)
            {
                // If infosFromACR.metadata.music[0].album.name exists, then put it in acrTags.TagHandler.Album. Else null
                AcrTags.Album = infosFromACR?.metadata?.music[0]?.album?.name;
                AcrTags.Title = infosFromACR?.metadata?.music[0]?.title;
                AcrTags.Artist = infosFromACR.metadata?.music[0]?.artists[0].name;
                AcrTags.Year = infosFromACR.metadata?.music[0]?.release_date.Substring(0, 4);
                var genres = "";
                if (infosFromACR.metadata?.music[0]?.genres != null)
                {
                    genres = infosFromACR.metadata?.music[0].genres.Aggregate(genres, (current, genre) => current + ", " + genre.name);
                    genres = genres.Substring(2);
                }
                if (genres != "")
                {
                    AcrTags.Genre = genres;
                }
            }
            Logger.Instance.ReadfromACRLog(this);
        }

        public void ReadTagFromAPI()
        {
            Logger.Instance.ReadfromAPILog(this);
        }

        public void ArbitrateNewTags()
        {
            /*
                Arbitrage de quels tags sont gardés pour le nouveau fichier.
                Si l'ancien est vide, on met le nouveau.
                Si l'ancien n'est pas vide, on garde Artiste / Album / Titre (probablement fiable),
                et on update les genres si on a plus d'infos que avant (plus riche probablement)
                
                Todo:
                + Checker plus fin pour les genres (comparer la liste, et garder tous les tags, sans doublons
            */
            string emptyTag = "";
            
            PropertyInfo[] tagsProps = typeof(TagHandler).GetProperties();

            foreach(PropertyInfo p in tagsProps)
            {
                foreach (var propName in Enum.GetValues(typeof(PropertiesForUser)))
                {
                    if (propName.ToString() == p.Name)
                    {
                        
                        // S'il le tag n'a pas changé, on ne fait rien. 
                        if (p.GetValue(NewTags) != p.GetValue(AcrTags))
                        {
                            // Pour tous, on check si l'ancien est vide. Dans ce cas on overide le nouveau
                            if (p.GetValue(NewTags).ToString() == emptyTag)
                            {
                                p.SetValue(NewTags, p.GetValue(AcrTags));
                            }
                            else
                            {
                                // Puis on gère les cas particuliers
                                switch (p.Name)
                                {
                                    case "Genre":
                                        if (p.GetValue(NewTags).ToString().Length < p.GetValue(AcrTags).ToString().Length)
                                        {
                                            p.SetValue(NewTags, p.GetValue(AcrTags));
                                        }
                                        break;
                                }

                                if (p.Name != "Year")
                                {
                                    string s1 = p.GetValue(NewTags).ToString();
                                    string s2 = p.GetValue(AcrTags).ToString();

                                    if (s1 != s2)
                                    {
                                        float ratio = 1 - (float)Levenshtein.CalculateDistance(s1, s2, 1) / (float)s2.Length;
                                    
                                        if (ratio >= 0.75)
                                        {
                                            Console.WriteLine(p.Name + " quasi match : " + s1 + " ~ " + s2 + " ratio: "+ratio);
                                            p.SetValue(NewTags, p.GetValue(AcrTags));
                                        }                                    
                                    }                                    
                                }

                            }
                        }
                    }
                }
            }
            Logger.Instance.ArbitrateNewTagsLog(this);
        }
        
        public void WriteTags()
        {
            MusicFile.TagHandler = NewTags;
            MusicFile.Update();
            Logger.Instance.WriteTagsLog(this);
        }

        
        /// <summary>
        /// Move or copy the music file to its new location. 
        /// </summary>
        /// <param name="targetFileName">Template of new file - ex: /home/Music/%Artist%/%Album%/%Title%.%extension%</param>
        /// <param name="copy">Is it a copy ? Or a move</param>
        public void Reorganize(string targetFileName, bool copy)
        {
            string target = ReplaceProp(targetFileName);
            
            target = target.Replace(((char) 0).ToString(), "");
            
            Directory.CreateDirectory(Path.GetDirectoryName(target));

            if (this.MusicFile.FileName != target)
            {
                if (copy)
                {
                    Logger.Instance.CopyFileLog(this, target);
                    File.Copy(this.MusicFile.FileName, target, true);
                }
                else
                {
                    Logger.Instance.MoveFileLog(this, target);
                    File.Move(this.MusicFile.FileName, target);
                }
            }
        }

        #endregion

        #region Utils
        
            private string ReplaceProp(string targetFolder)
            {
                string toReturn = targetFolder;
                PropertyInfo[] props = typeof(TagHandler).GetProperties();
    
                foreach(PropertyInfo p in props)
                {
                    foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
                    {
                        if (propName.ToString() == p.Name)
                        {
                            var propValue = p.GetValue(this.MusicFile.TagHandler).ToString();
                            foreach (char c in illegalCharFromFileName)
                            {
                                
                                propValue = propValue.Replace(c.ToString(), illegalCharReplacor);
                            }
                            toReturn=toReturn.Replace("%"+p.Name+"%", propValue);
                        }
                    }
                }
                return toReturn;
            }
        

        #endregion
    }
}
