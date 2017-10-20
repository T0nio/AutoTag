using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoTag
{
    static class ReorganizeUtils
    {
        public static void MoveFiles(List<Musics> musicList, string targetFolder) //targetFolder="C:\Users\Music\{music.newFile.Artist}\{music.newFile.Album}\"
        {
            foreach (Musics music in musicList)
            {
                if (music.File.TagHandler.Artist == " " || music.File.TagHandler.Album == " ")
                {
                    string target = targetFolder + $"{music.File.TagHandler.Artist}" + Path.DirectorySeparatorChar + $"{music.File.TagHandler.Album}";
                    Console.WriteLine(music.File.FileName);
                    Console.WriteLine(target);
                    Directory.CreateDirectory(target);
                    target = target + Path.DirectorySeparatorChar + Path.GetFileName(music.File.FileName);
                    if (music.File.FileName != target)
                    {
                        Directory.Move(music.File.FileName, target);

                    }
                }
            }
        }
    }
}
