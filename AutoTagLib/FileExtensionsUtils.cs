using System;
using System.Collections.Generic;

namespace AutoTagLib
{
    public static class FileExtensionsUtils
    {
        public static List<string> MusicFileExtensions
        {
            get
            {
                // According to Wikipedia : https://en.wikipedia.org/wiki/Audio_file_format#frb-inline
                return new List<string>
                {
                    ".3gp",
                    ".aa",
                    ".aac",
                    ".aax",
                    ".act",
                    ".aiff",
                    ".amr",
                    ".ape",
                    ".au",
                    ".awb",
                    ".dct",
                    ".dss",
                    ".dvf",
                    ".flac",
                    ".gsm",
                    ".iklax",
                    ".ivs",
                    ".m4a",
                    ".m4b",
                    ".m4p",
                    ".mmf",
                    ".mp3",
                    ".mpc",
                    ".msv",
                    ".ogg",
                    ".oga",
                    ".mogg",
                    ".opus",
                    ".ra",
                    ".rm",
                    ".raw",
                    ".sln",
                    ".tta",
                    ".vox",
                    ".wav",
                    ".wma",
                    ".wv",
                    ".webm",
                    ".8svx",
                };
            }
        }

        /// <summary>
        /// Check if the extension is a music file extension
        /// </summary>
        /// <param name="extension">Extension</param>
        /// <returns>true or false</returns>
        public static bool IsMusicFile(string extension)
        {
            return MusicFileExtensions.Contains(extension);
        }

    }

    public class NotMusicFileExtensionException : Exception
    {
        public NotMusicFileExtensionException(string extension) : base ($"Extension {extension} is not a music file extension.")
        {
        }
    }
}