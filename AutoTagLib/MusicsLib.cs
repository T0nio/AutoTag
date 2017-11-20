using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AutoTagLib
{
    public class MusicsLib
    {

        #region Properties

        public Dictionary<string, List<Musics>> Dict  { get; set; }
        public List<string> Extensions { get; set; } = new List<string>() {".mp3"};

        private bool _fatalACRError = false;
        private bool _fatalAPIError = false;

        #endregion

        #region Methods
       
        /// <summary>
        /// Fill a dictionnary with all the subfolders path coming from the path and link it to the list from CreateList
        /// </summary>
        /// <param name="folder">Path of the folder</param>
        public void LoadFromFolder(string folder) 
        {
            Dict = new Dictionary<string, List<Musics>>();

            foreach (string dir in ListDirectoriesFromFolder(folder))
            {
                var listMusics = ListMusicFilesFromFolder(dir, Extensions);
                if (!this.Dict.ContainsKey(dir) && listMusics.Count > 0)
                {
                    this.Dict.Add(dir, listMusics);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ReadTags()
        {
            foreach (KeyValuePair<string, List<Musics>> subfoldersKeyValuePair in this.Dict)
            {
                foreach (var music in subfoldersKeyValuePair.Value)
                {
                    this._fatalACRError = this._fatalACRError || !music.ReadTagFromACR();
                    this._fatalAPIError = this._fatalAPIError || !music.ReadTagFromAPI();

                    if (this._fatalACRError && this._fatalAPIError)
                    {
                        this._fatalACRError = this._fatalAPIError = false;
                        return false;
                    }
                }
            }

            return true;
        }

        public void WriteTags()
        {
            foreach (KeyValuePair<string, List<Musics>> subfoldersKeyValuePair in this.Dict)
            {
                foreach (var music in subfoldersKeyValuePair.Value)
                {
                    music.ArbitrateBetweenTags();
                    music.WriteTags();
                }
            }
        }

        public void Reorganize(string targetFolder, bool copy)
        {
            foreach (KeyValuePair<string, List<Musics>> subfoldersKeyValuePair in this.Dict)
            {
                foreach (var music in subfoldersKeyValuePair.Value)
                {
                    music.Reorganize(targetFolder, copy);
                }
            }
        }

        #endregion

        #region ListFilesUtils
        
        /// <summary>
        /// Listing all subdirectories from a folder
        /// </summary>
        /// <param name="folder">Root folder path</param>
        /// <returns>List of path of subfolders</returns>
        public static List<string> ListDirectoriesFromFolder(string folder) //Return the list of all subfolders
        {
            List<string> toReturn = new List<string> { folder };
        
            foreach (string subFolder in Directory.GetDirectories(folder))
            {
                toReturn.AddRange(ListDirectoriesFromFolder(subFolder));
            }
        
            return toReturn;
        }
        
        /// <summary>
        /// Listing all musics in a folder
        /// </summary>
        /// <param name="folder">Folder path</param>
        /// <param name="extensions">Accepted musics extensions to put in the list</param>
        /// <returns>List of Musics [object]</returns>
        /// <exception cref="NotMusicFileExtensionException">If one extension is not a music extension. (as defined in FileExtensionsUtils</exception>
        public static List<Musics> ListMusicFilesFromFolder(string folder, List<string> extensions) //Select only the music files from folder and sub folders
        {
            List<Musics> toReturn = new List<Musics>();
                    
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
                    toReturn.Add(new Musics(file));
                }
            }
        
            return toReturn;
        }

        #endregion
    }
}
