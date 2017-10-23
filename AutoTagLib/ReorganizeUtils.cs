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
        public static void MoveFiles(List<Musics> musicList, string targetFolder) //targetFolder="C:\Users\Music\%Artist%\%Album%\"
        {
            foreach (Musics music in musicList)
            {
                string target = ReplaceProp(music, targetFolder);
                Console.WriteLine(target);
                Directory.CreateDirectory(target);
                target = target + Path.GetFileName(music.File.FileName);
                if (music.File.FileName != target)
                {
                    Directory.Move(music.File.FileName, target);
                    //ou copier
                }
            }
            //Ajouter un état d'avancement
        }

        public static string ReplaceProp(Musics music, string targetFolder)
        {
            string toReturn = targetFolder;
            PropertyInfo[] props = typeof(TagHandler).GetProperties();

            foreach(PropertyInfo p in props)
            {
                foreach (var propName in Enum.GetValues(typeof(Musics.PropertiesForUser)))
                {
                    if (propName.ToString() == p.Name)
                    {
                        toReturn=toReturn.Replace("%"+p.Name+"%",p.GetValue(music.File.TagHandler).ToString());
                    }
                }
            }
            return toReturn;
        }
#endregion
    }
}
