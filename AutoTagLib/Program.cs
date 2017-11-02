using System;

using Id3Lib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AutoTagLib
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            /*MusicLists musicLibrary = new MusicLists();

            musicLibrary.FillDict(@"C:\Users\pierr\Music\BOB");*/
            MusicLists musicLibrary = new MusicLists();
            musicLibrary.FillDict(@"C:\Users\pierr\Music\BOB");

            foreach (KeyValuePair<string, List<Musics>> kvp in musicLibrary.Dict)
            {
                Console.WriteLine(kvp.Key);
                foreach (Musics file in kvp.Value)
                {
                   Console.WriteLine("\t" + file.File.FileName);
                }
            }

            //string target = @"C:\Users\pierr\Music\BOB\Test\";

            //foreach(KeyValuePair<string,List<Musics>> kvp in musicLibrary.Dict)
            //{
            //    ReorganizeUtils.MoveFiles(kvp.Value, target);
            //}

            Musics music = new Musics(@"/data/Desktop/TestACR/unaname.mp3");
  
            music.ReadTagFromACR();
            music.ReadTagFromAPI();
            music.ArbitrateNewTags();
            
            //Musics music = new Musics(@"C:\Users\pierr\Music\BOB\Test\Basshunter\Saturday\Saturday.mp3");


            //ReorganizeUtils.ReplaceProp(music,"");
            //Logger.Instance.log();


        }
    }
}
