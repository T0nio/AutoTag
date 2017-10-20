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
        private Tags oldFile;
        private Tags acrTags;
        private Tags apiTags;
        private Tags newFile;
        public string FilePath{ get; set; }

        public Musics(string path)
        {
            FilePath = path;
            oldFile = new Tags();
            acrTags = new Tags();
            apiTags = new Tags();
            
        }

        public void ReadTags()
        {
            
        }
        
        public void ReadTagFromACR()
        {
            ACRCloudJsonObject infosFromACR = Infos.Recognize(FilePath);

            // If infosFromACR.metadata.music[0].album.name exists, then put it in acrTags.TagHandler.Album. Else null
            acrTags.Album = infosFromACR.metadata.music[0]?.album?.name;
            acrTags.Title = infosFromACR.metadata.music[0]?.title;
            acrTags.Artist = infosFromACR.metadata.music[0]?.artists[0].name;
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
                acrTags.Genre = genres;
            }

            acrTags.Year = infosFromACR.metadata.music[0]?.release_date.Substring(0, 4);

        }

        public void ReadTagFromAPI()
        {

        }
        
        public void WriteTags()
        {

        }

        public void Reorganize(string option, string format)
        {

        }
    }
}
