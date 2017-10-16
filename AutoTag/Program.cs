using System;
using NAudio;
using Id3Lib;
using Mp3Lib;
using System.IO;

namespace AutoTag
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Mp3File musique = new Mp3File("C:\\Users\\pierr\\Music\\Compil J6 2k15\\10 The Nights.mp3");
            Console.WriteLine(musique.TagHandler.Album);
        }
    }
}