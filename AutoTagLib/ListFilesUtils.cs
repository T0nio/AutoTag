using System.Collections.Generic;
using System.IO;

namespace AutoTagLib
{
    public static class ListFilesUtils
    {
        public static List<string> ListFilesRecursivelyFromFolder(string folder) //list of all files in folder and sub folder
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

        public static List<string> ListDirectoriesFromFolder(string folder) //Return the list of all subfolders
        {
            List<string> toReturn = new List<string> { folder };

            foreach (string dir in Directory.GetDirectories(folder))
            {
                toReturn.AddRange(ListDirectoriesFromFolder(dir));
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

        public static List<string> ListMusicFilesFromFolder(string folder, List<string> extensions) //Select only the music files from folder and sub folders
        {
            List<string> toReturn = new List<string>();

            foreach (string extension in extensions)
            {
                if(!FileExtensionsUtils.IsMusicFile(extension))
                {
                    throw new NotMusicFileExtensionException(extension);
                }
            }

            foreach (string file in Directory.GetFiles(folder))
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