using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoTag
{
    class ReorganizeUtils
    {
        public static void MoveFiles(List<Musics> musicList, string targetFolder) //targetFolder="C:\Users\Music\{music.newFile.Artist}\{music.newFile.Album}\"
        {
            foreach (Musics music in musicList)
            {
                string target = targetFolder + $"{music.NewFile.Artist}\\{music.NewFile.Album}";
                Console.WriteLine(music.File.FileName);
                Console.WriteLine(target);
                target=target+"\\" + FileNameWhithoutPath(music.File.FileName);
                Directory.CreateDirectory(target);
                //Directory.Move(music.File.FileName, target);
            }
        }

        public static string FileNameWhithoutPath(string path)
        {
            string toReturn = path;

            while (toReturn.IndexOf("\\")!=-1)
            {
                toReturn = toReturn.Substring(toReturn.IndexOf("\\") + 1);
            }
            return toReturn;
        }
    }
}
