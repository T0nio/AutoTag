using System.Collections.Generic;
using System.IO;

namespace AutoTag
{
    public static class ListFilesUtils
    {
        public static List<string> ListFilesRecursivelyFromFolder(string folder)
        {
            List<string> toReturn = new List<string>();

            foreach (string file in Directory.GetFiles(folder))
            {
                toReturn.Add(file.Replace('\\', '/'));
            }

            foreach(string dir in Directory.GetDirectories(folder))
            {
                toReturn.AddRange(ListFilesRecursivelyFromFolder(dir));
            }

            return toReturn;
        }

        public static List<string> ListMusicFilesFromFolder(string folder, string extension)
        {
            return ListMusicFilesFromFolder(folder, new List<string> { extension });
        }

        public static List<string> ListMusicFilesFromFolder(string folder)
        {
            return ListMusicFilesFromFolder(folder, FileExtensionsUtils.MusicFileExtensions);
        }

        public static List<string> ListMusicFilesFromFolder(string folder, List<string> extensions)
        {
            List<string> toReturn = new List<string>();

            foreach (string extension in extensions)
            {
                if(!FileExtensionsUtils.IsMusicFile(extension))
                {
                    throw new NotMusicFileExtensionException(extension);
                }
            }

            foreach (string file in ListFilesRecursivelyFromFolder(folder))
            {
                if (extensions.Contains(Path.GetExtension(file)))
                {
                    toReturn.Add(file);
                }
            }

            return toReturn;
        }
    }
}