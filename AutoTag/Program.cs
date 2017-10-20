using System;
using Id3Lib;

namespace AutoTag
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Musics originalMusic = new Musics(@"D:\Documents\Programme\C#\AutoTag\AutoTagTests\rootTestFolder\notEmptyTestFolder\01 Music.mp3");
            
            Musics music = new Musics(@"C:\Users\Thibaut\Desktop\01 Music.mp3");
            TagHandler tag = originalMusic.OldFile;
            music.NewFile = tag;
            music.WriteTags();
            Console.WriteLine(music.File.FileName);
        }
    }
}
