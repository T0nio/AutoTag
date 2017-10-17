using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Recognizer.API;

namespace Recognizer
{
    public class Recognizer
    {
        private readonly ACRCloudRecognizer _recognizer; 
        
        public Recognizer()
        {
            var config = new Dictionary<string, object>();
            var acrCloudConfig = ConfigurationManager.GetSection("ACRCloud") as NameValueCollection;
            config.Add("host", acrCloudConfig["host"]);
            config.Add("access_key", acrCloudConfig["access_key"]);
            config.Add("access_secret", acrCloudConfig["access_secret"]);
            config.Add("timeout", int.Parse(acrCloudConfig["timeout"]));
            
            _recognizer = new ACRCloudRecognizer(config);
        }

        public ACRCloudJsonObject Recognize(string filePath)
        {
            string result = _recognizer.RecognizeByFile(filePath, 10);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ACRCloudJsonObject>(result);
            
            
            /*
            EXAMPLE OF DATA THAT ARE PRESENT
            Console.WriteLine("Title: " + songInfos.metadata.music[0].title);
            Console.WriteLine("Artist : " + songInfos.metadata.music[0].artists[0].name);
            Console.WriteLine("Album : " + songInfos.metadata.music[0].album.name);
            string genres = "";
            foreach (var genre in songInfos.metadata.music[0].genres)
            {
                genres = genres + ", " + genre.name;
            }
            genres = genres.Substring(2);
            Console.WriteLine("Genres : " + genres);*/
        }

        public Tuple<bool, string, string> GetArtistAndTitleFromACR(string filePath)
        {
            string result = _recognizer.RecognizeByFile(filePath, 10);
            var songInfos = Newtonsoft.Json.JsonConvert.DeserializeObject<ACRCloudJsonObject>(result);

            try
            {
                return new Tuple<bool, string, string>(true, songInfos.metadata.music[0].artists[0].name, songInfos.metadata.music[0].title);

            }
            catch (Exception e)
            {
                return new Tuple<bool, string, string>(false, "", "");
            }

        }
/*
        public APIJsonObject GetSongInfosFromAPI(string artist, string title)
        {
            // Do the GET API Call
            // Convert to JSON
            // Return object
            
        }*/
        
        
    }
}