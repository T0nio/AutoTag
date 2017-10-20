using System;

using Id3Lib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AutoTag
{
    internal class Program
    {
        public static void Main(string[] args)
        {
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
            string target = @"C:\Users\pierr\Music\BOB\Test\";

            foreach(KeyValuePair<string,List<Musics>> kvp in musicLibrary.Dict)
            {
                ReorganizeUtils.MoveFiles(kvp.Value, target);
            }

            Musics music = new Musics(@"C:\Users\pierr\Music\BOB\Test\Basshunter\Saturday\Saturday.mp3");
            PropertyInfo[] props = typeof(TagHandler).GetProperties();

            foreach (PropertyInfo p in props)
            {
                Console.WriteLine(p.Name);
            }
        }
    }
}
