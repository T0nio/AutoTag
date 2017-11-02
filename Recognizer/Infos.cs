﻿using System;
using System.Diagnostics;
using MusicInfoLib.API;
using DiscogsClient.Data.Query;


namespace MusicInfoLib
{
    public static class Infos
    {
        public static ACRCloudJsonObject Recognize(string filePath)
        {
            try
            {
                string result = ACRCloudRecognizer.Instance.RecognizeByFile(filePath, 10);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ACRCloudJsonObject>(result);
            }
            catch (System.Net.WebException e)
            {

                Console.WriteLine("Net exception");
                return new ACRCloudJsonObject()
                {
                    status =
                    {
                        msg = "Time out",
                        code = 555,
                        version = "1.0"
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("BUG");
                return new ACRCloudJsonObject();
            }
        }

        public static Tuple<bool, string, string> GetArtistAndTitleFromACR(string filePath)
        {
            string result = ACRCloudRecognizer.Instance.RecognizeByFile(filePath, 10);
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

        public static void GetSongInfosFromAPI(string artist, string title)
        {
            // Do the GET API Call
            // Convert to JSON
            // Return object
            
            var discogsSearch = new DiscogsSearch()
            {
                artist = artist,
                release_title = title
            };
    
            //Retrieve observable result from search
            var observable = DiscogsAPI.Instance.client.Search(discogsSearch);

            
            var enumerable = DiscogsAPI.Instance.client.SearchAsEnumerable(discogsSearch);

            
            foreach (var v in enumerable)
            {
                Console.WriteLine(v.title);                
                Console.WriteLine(v.id);                
            }
            //Console.WriteLine(enumerable.ToString());
        }

        
        
    }
}