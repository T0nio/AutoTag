using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3Lib;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace AutoTagLib
{
    static class ReorganizeUtils
    {

#region Methods
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
            //Ajouter un état d'avancement
        }

        public static void ReplaceProp(Musics music, string target)
        {
            string toReturn = target;
            PropertyInfo[] props = typeof(TagHandler).GetProperties();

            foreach(PropertyInfo p in props)
            {
                foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
                {
                    if (propName.ToString() == p.Name)
                    {
                        p.GetValue(music.File.TagHandler);

                        Console.WriteLine(p.GetValue(music.File.TagHandler));
                    }
                }

            }

        }
#endregion
    }
}
