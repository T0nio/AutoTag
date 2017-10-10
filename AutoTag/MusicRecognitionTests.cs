using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using AutoTag.MusicRecognition;

namespace AutoTag
{
    public class MusicRecognitionTests
    {
        public MusicRecognitionTests()
        {
            var config = new Dictionary<string, object>();
            var acrCloudConfig = ConfigurationManager.GetSection("ACRCloud") as NameValueCollection;
            config.Add("host", acrCloudConfig["host"]);
            config.Add("access_key", acrCloudConfig["access_key"]);
            config.Add("access_secret", acrCloudConfig["access_secret"]);
            config.Add("timeout", int.Parse(acrCloudConfig["timeout"]));
            
            ACRCloudRecognizer re = new ACRCloudRecognizer(config);

            // It will skip 0 seconds from the beginning of misterysong.mp3.
            string result = re.RecognizeByFile(@"songsuccess.mp3", 10);
            //string result = re.RecognizeByFile(@"songfail.mp3", 10);

            var resultJson = Newtonsoft.Json.JsonConvert.DeserializeObject<ACRCloudJsonObject>(result);
            if (resultJson.status.code == 0)
            {
                Console.WriteLine(result);
                /*
                Console.WriteLine("Title: " + resultJson.metadata.music[0].title);
                Console.WriteLine("Artist : " + resultJson.metadata.music[0].artists[0].name);
                Console.WriteLine("Album : " + resultJson.metadata.music[0].album.name);
                string genres = "";
                foreach (var genre in resultJson.metadata.music[0].genres)
                {
                    genres = genres + ", " + genre.name;
                }
                genres = genres.Substring(2);
                Console.WriteLine("Genres : " + genres);*/
            }
            else
            {
                Console.WriteLine("Error: " + resultJson.status.msg);                
            }
            

        }
    }
}