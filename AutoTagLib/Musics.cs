using Id3Lib;
using Mp3Lib;
using System;
using System.Linq;
using System.Reflection;
using MusicInfoLib;
using System.IO;

namespace AutoTagLib
{
    public class Musics
    {

        #region Properties

        public TagHandler OriginalTags { get; }
        public TagHandler AcrTags { get; }
        public TagHandler NewTags { get; }
        public Mp3File File { get; }
        public string FileName {
            get
            {
                return File.FileName.Split(Path.DirectorySeparatorChar).Last();
            }
        }

        public enum PropertiesForUser
        {
            Artist,
            Album,
            Composer,
            Disc,
            FileName,
            Genre,
            Title,
            Track,
            Year
        }

        #endregion

        #region Constructor

        public Musics(string path)
        {
            File = new Mp3File(path);
            OriginalTags = File.TagHandler;
            AcrTags = new Mp3File(path).TagHandler;
            NewTags = new Mp3File(path).TagHandler;
        }
        #endregion

        #region Methods

        public void ReadTags()
        {
            
        }
        
        public void ReadTagFromACR()
        {
            
            ACRCloudJsonObject infosFromACR = Infos.Recognize(File.FileName);

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
        }

        public void ReadTagFromAPI()
        {

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
                                Console.WriteLine(p.Name + "   " + p.GetValue(AcrTags));
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
                            }
                        }
                    }
                }
            }
        }
        
        public void WriteTags()
        {
            File.TagHandler = NewTags;
            File.Update();
        }

        public void Reorganize(string option, string format)
        {
            
        }

        #endregion
    }
}
