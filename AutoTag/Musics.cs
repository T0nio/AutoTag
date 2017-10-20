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

        public TagHandler OldFile { get; private set; }
        public TagHandler NewFile { get; set; }
        public TagHandler AcrFile { get; set; }
        public Mp3File File { get; private set; }

        public Musics(string path)
        {
            File = new Mp3File(path);
            OldFile = File.TagHandler;
            NewFile = OldFile;
            AcrFile = OldFile;
        }

        public void ReadTags()
        {
            
        }
        
        public void ReadTagFromACR()
        {
            ACRCloudJsonObject infosFromACR = Infos.Recognize(File.FileName);

            // If infosFromACR.metadata.music[0].album.name exists, then put it in acrTags.TagHandler.Album. Else null
            AcrFile.Album = infosFromACR.metadata.music[0]?.album?.name;
            AcrFile.Title = infosFromACR.metadata.music[0]?.title;
            AcrFile.Artist = infosFromACR.metadata.music[0]?.artists[0].name;
            AcrFile.Year = infosFromACR.metadata.music[0]?.release_date.Substring(0, 4);
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
                AcrFile.Genre = genres;
            }


        }

        public void ReadTagFromAPI()
        {

        }
        
        public void WriteTags()
        {
            File.TagHandler = NewFile;
            File.Update();
        }

        public void Reorganize(string option, string format)
        {

        }
    }
}
