using System;

using Id3Lib;
using System.Collections.Generic;
using System.IO;

namespace AutoTag
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Musics originalMusic = new Musics(@"C:\Users\pierr\Music\BOB\Bassunther\Saturday.mp3");

            //Musics music = new Musics(@"C:\Users\pierr\Music\BOB\Bassunther\Saturday.mp3");

            //TagHandler tag = originalMusic.OldFile;
            //music.NewFile = tag;
            //music.WriteTags();
            //Console.WriteLine(music.File.FileName);

            // Console.WriteLine(originalMusic.OldFile.Artist);

            MusicLists musicLibrary = new MusicLists();

            musicLibrary.FillDict(@"C:\Users\pierr\Music\BOB");

            //foreach (KeyValuePair<string, List<Musics>> kvp in musicLibrary.Dict)
            //{
            //    Console.WriteLine(kvp.Key);
            //    foreach (Musics file in kvp.Value)
            //    {
            //        Console.WriteLine("\t"+file.File.FileName);
            //        Console.WriteLine("\t\t" + file.OldFile.Artist);
            //    }
            //}
            Musics music = new Musics(@"C: \Users\pierr\Music\BOB\Bassunther\Saturday.mp3");
            string target = $"C:\\Users\\pierr\\Music\\BOB\\Test\\";

            foreach(KeyValuePair<string,List<Musics>> kvp in musicLibrary.Dict)
            {
                ReorganizeUtils.MoveFiles(kvp.Value, target);
            }
        }
    }
}
