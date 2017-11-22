using Id3Lib;
using Mp3Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using MinimumEditDistance;
using AutoTagLib.Recognizer;
using AutoTagLib.ErrorManager;
using Id3Lib.Exceptions;

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
        private readonly List<char> _illegalCharFromFileName = new List<char>(){ '/', '\\', ':', '*', '?', '"', '<', '>', '|'};
        private readonly string _illegalCharReplacor = "-";

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
            } catch (NotImplementedException)
            {
                ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.id3v2_not_supported, MusicFile.FileName);
                OriginalTags = new TagHandler(new TagModel());
                AcrTags = new TagHandler(new TagModel());
                ApiTags = new TagHandler(new TagModel());
                NewTags = new TagHandler(new TagModel());
            } catch (InvalidFrameException)
            {
                ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.invalid_encoding, MusicFile.FileName);
                OriginalTags = new TagHandler(new TagModel());
                AcrTags = new TagHandler(new TagModel());
                ApiTags = new TagHandler(new TagModel());
                NewTags = new TagHandler(new TagModel());
            } catch (Exception)
            {
                ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.unknown, MusicFile.FileName);
                OriginalTags = new TagHandler(new TagModel());
                AcrTags = new TagHandler(new TagModel());
                ApiTags = new TagHandler(new TagModel());
                NewTags = new TagHandler(new TagModel());
            }
            Logger.Instance.LoadFromDirectoryLog(this);
        }
        #endregion

        #region Methods

        public bool ReadTagFromACR()
        {
            ACRCloudJsonObject infosFromACR = Infos.Recognize(MusicFile.FileName);

            if (infosFromACR.status.code == 0)
            {
                // If infosFromACR.metadata.music[0].album.name exists, then put it in acrTags.TagHandler.Album. Else null
                AcrTags.Album = infosFromACR?.metadata?.music[0]?.album?.name;
                AcrTags.Title = infosFromACR?.metadata?.music[0]?.title;
                AcrTags.Artist = infosFromACR.metadata?.music[0]?.artists[0].name;
                AcrTags.Year = infosFromACR.metadata?.music[0]?.release_date.Substring(0, 4);
                var genres = String.Empty;
                if (infosFromACR.metadata?.music[0]?.genres != null)
                {
                    genres = infosFromACR.metadata?.music[0].genres.Aggregate(genres, (current, genre) => current + ", " + genre.name);
                    genres = genres.Substring(2);
                }
                if (genres != String.Empty)
                {
                    AcrTags.Genre = genres;
                }

                Logger.Instance.ReadfromACRLog(this,infosFromACR.status.code,infosFromACR.status.msg);
                return true;
            }

            switch (infosFromACR.status.code)
            {
                case 3001:
                    ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_invalid_key);
                    break;

                case 3014:
                    ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_invalid_secret);
                    break;

                case 555:
                    ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_timeout);
                    break;

                case 666:
                    ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_unknown);
                    break;

                case 777:
                    ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_dll);
                    break;

                case 3000:
                    ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_timeout);
                    break;

                case 3024:
                    ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_invalid_host);
                    break;

                case 3022:
                    ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_unknown);
                    break;

                case 3003:
                    ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_limit_reached);
                    break;

                default:
                    ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.acr_unknown);
                    break;
            }

            Logger.Instance.ReadfromACRLog(this, infosFromACR.status.code, infosFromACR.status.msg);
            return false;
        }

        public bool ReadTagFromAPI()
        {
            // Not Implemented
            Logger.Instance.ReadfromAPILog(this, string.Empty);
            return false;
        }

        public void ArbitrateBetweenTags()
        {
            /*
                Arbitrage de quels tags sont gardés pour le nouveau fichier.
                Si l'ancien est vide, on met le nouveau.
                Si l'ancien n'est pas vide, on garde Artiste / Album / Titre (probablement fiable),
                et on update les genres si on a plus d'infos que avant (plus riche probablement)
                
                Todo:
                + Checker plus fin pour les genres (comparer la liste, et garder tous les tags, sans doublons
            */
            string emptyTag = string.Empty;

            List<string> compareTags = new List<string>();
            PropertyInfo[] tagsProps = typeof(TagHandler).GetProperties();

            foreach (var propName in Enum.GetValues(typeof(PropertiesForUser)))
            {
                foreach (PropertyInfo p in tagsProps)
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
                                compareTags.Add($"{p.GetValue(OriginalTags)} to {p.GetValue(AcrTags)}");
                            } else
                            {
                                // Puis on gère les cas particuliers
                                switch (p.Name)
                                {
                                    case "Genre":
                                        if (p.GetValue(NewTags).ToString().Length < p.GetValue(AcrTags).ToString().Length)
                                        {
                                            p.SetValue(NewTags, p.GetValue(AcrTags));
                                            compareTags.Add($"{p.GetValue(OriginalTags)} to {p.GetValue(AcrTags)}");
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
                                            p.SetValue(NewTags, p.GetValue(AcrTags));
                                            compareTags.Add($"{p.GetValue(OriginalTags)} to {p.GetValue(AcrTags)}");
                                        }                                    
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Logger.Instance.ArbitrateNewTagsLog(this, compareTags);
        }
        
        public void WriteTags()
        {
            try
            {
                MusicFile.TagHandler = NewTags;
                MusicFile.Update();
            }
            catch (NotImplementedException e)
            {
                ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.id3v2_not_supported, MusicFile.FileName);
                Logger.Instance.WriteTagsLog(this, e.ToString());
            } catch (InvalidFrameException e)
            {
                ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.invalid_encoding, MusicFile.FileName);
                Logger.Instance.WriteTagsLog(this, e.ToString());
            } catch (Exception e)
            {
                ((IErrorManager)Lookup.GetInstance().Get(typeof(IErrorManager))).NewError(ErrorCodes.unknown, MusicFile.FileName);
                Logger.Instance.WriteTagsLog(this, e.ToString());
            }
            Logger.Instance.WriteTagsLog(this, "No exception");

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
            
            // getNewFileName -> if null => filename = oldfilename 
            
            Directory.CreateDirectory(Path.GetDirectoryName(target));


            if (this.MusicFile.FileName != target)
            {
                if (File.Exists(target))
                {
                    int count = 1;
                    string fileNameOnly = Path.GetFileNameWithoutExtension(target);
                    string extension = Path.GetExtension(target);
                    string path = Path.GetDirectoryName(target);

                    while (File.Exists(target))
                    {
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                        target = Path.Combine(path, tempFileName + extension);
                    }
                }

                if (copy)
                {
                    File.Copy(this.MusicFile.FileName, target, true);
                    Logger.Instance.CopyFileLog(this, target);
                } else
                {
                    File.Move(this.MusicFile.FileName, target);
                    Logger.Instance.MoveFileLog(this, target);
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
                foreach (var propName in Enum.GetValues(typeof(PropertiesForUser)))
                {
                    if (propName.ToString() == p.Name)
                    {
                        var propValue = String.Empty;
                        try
                        {
                            propValue = p.GetValue(this.MusicFile.TagHandler).ToString();
                        } catch (Exception) { }
                        foreach (char c in _illegalCharFromFileName)
                        {
                            propValue = propValue.Replace(c.ToString(), _illegalCharReplacor);
                        }
                        if (propValue == String.Empty)
                        {
                            toReturn = toReturn.Replace($"%{p.Name}%", $"Unknown {p.Name}");
                        } else
                        {
                            toReturn = toReturn.Replace($"%{p.Name}%", p.Name == "Track" && int.TryParse(propValue, out int propValueInt) ? $"{propValueInt:00}" : propValue);
                        }
                    }
                }
            }
            return toReturn;
        }
        
        #endregion
    }
}
