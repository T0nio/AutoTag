using System;
using System.Collections.Generic;
using NAudio;
using Id3Lib;
using Mp3Lib;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Recognizer;

namespace AutoTag
{
    internal class Program
    {
        public static void Main(string[] args)
        {
<<<<<<< HEAD
/*
            Mp3File musique = new Mp3File("C:\\Users\\pierr\\Music\\Compil J6 2k15\\10 The Nights.mp3");
            Console.WriteLine(musique.TagHandler.Album);

            new MusicRecognitionTests();
*/
            
        }

        public void RecoTest()
        {
            Recognizer.Recognizer re = new Recognizer.Recognizer();
            var songInfos = re.GetArtistAndTitleFromACR(@"songsuccess.mp3");

            if (songInfos[0])
            {
                Console.WriteLine("Artist: " + songInfos[1] + " | Title: "+songInfos[2]);
            }
            else
            {
                Console.WriteLine("No Data Found");   
            }
            Tags musique2 = new Tags(@"C:\Users\pierr\Music\Compil J6 2k15\10 The Nights.mp3");
            Console.WriteLine(musique2.TagHandler.Artist);
            MusicLists music = new MusicLists();
=======
            MusicLists musicList = new MusicLists();
            
>>>>>>> Dict creation
            string folder = @"C:\Users\pierr\Music\Alicia Keys";

            musicList.FillDict(folder);

            foreach(KeyValuePair<string,List<Musics>> kvp in musicList.Dict)
            {
                Console.WriteLine(kvp.Key);
                foreach(Musics value in kvp.Value)
                {
                    Console.WriteLine("\t" + value.FilePath);
                }

            }

        }
    }
}
