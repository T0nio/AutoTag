using Id3Lib;
using Mp3Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using MusicInfoLib;

namespace AutoTag
{
    class Musics
    {
        public TagHandler OriginalTags { get; }
        public TagHandler AcrTags { get; }
        public TagHandler NewTags { get; }
        public Mp3File File { get; }

        public Musics(string path)
        {
            File = new Mp3File(path);
            OriginalTags = File.TagHandler;
            AcrTags = OriginalTags;
            NewTags = OriginalTags;
        }

        public void ReadTags()
        {
            
        }
        
        public void ReadTagFromACR()
        {
            ACRCloudJsonObject infosFromACR = Infos.Recognize(File.FileName);

            // If infosFromACR.metadata.music[0].album.name exists, then put it in acrTags.TagHandler.Album. Else null
            AcrTags.Album = infosFromACR.metadata.music[0]?.album?.name;
            AcrTags.Title = infosFromACR.metadata.music[0]?.title;
            AcrTags.Artist = infosFromACR.metadata.music[0]?.artists[0].name;
            AcrTags.Year = infosFromACR.metadata.music[0]?.release_date.Substring(0, 4);
            string genres = "";
            if (infosFromACR.metadata.music[0].genres != null)
            {
                foreach (var genre in infosFromACR.metadata.music[0].genres)
                {
                    genres = genres + ", " + genre.name;
                }
                genres = genres.Substring(2);                
            }
            if (genres != "")
            {
                AcrTags.Genre = genres;
            }


        }

        public void ReadTagFromAPI()
        {

        }
        
        public void WriteTags()
        {
            File.TagHandler = NewTags;
            File.Update();
        }

        public void Reorganize(string option, string format)
        {

        }
    }
}
