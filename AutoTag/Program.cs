using System;
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
/*
            Mp3File musique = new Mp3File("C:\\Users\\pierr\\Music\\Compil J6 2k15\\10 The Nights.mp3");
            Console.WriteLine(musique.TagHandler.Album);

            new MusicRecognitionTests();
*/
            RecoTest();

        }

        public static void RecoTest()
        {
            Recognizer.Recognizer re = new Recognizer.Recognizer();
            var songInfos = re.GetArtistAndTitleFromACR(@"songsuccess.mp3");

            if (songInfos.Item1)
            {
                Console.WriteLine("Artist: " + songInfos.Item2 + " | Title: "+songInfos.Item3);
            }
            else
            {
                Console.WriteLine("No Data Found");   
            }
        }
    }
}
