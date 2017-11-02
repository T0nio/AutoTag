using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTagLib
{
    class MusicLists
    {
#region Properties
        public Dictionary<string, List<Musics>> Dict  { get; set; }

        public List<string> Extensions { get; set; }
#endregion

#region Constructor
        public MusicLists()
        {

        }
#endregion

#region Methods
        public List<Musics> CreateList(string path) //Fill an empty list with all the files inside the path (no subfolders) with extensions authorized
        {
            return null;
        }

        //TO ADD : EXTENSIONS FROM LISTFILESUTILS
        public static List<Musics> ListFromFileToMusic(string folder) //Return a list of Musics object from the list of paths
        {
            List<Musics> toReturn = new List<Musics>();

            foreach (string file in ListFilesUtils.ListMusicFilesFromFolder(folder,".mp3"))
            {
                Musics music = new Musics(file);
                toReturn.Add(music);
            }

            return toReturn;
        }

        public void FillDict( string folder) //Fill a dictionnary with all the subfolders path coming from the path and link it to the list from CreateList
        {
            Dict = new Dictionary<string, List<Musics>>();

            foreach (string dir in ListFilesUtils.ListDirectoriesFromFolder(folder))
            {
                if (!this.Dict.ContainsKey(dir) && MusicLists.ListFromFileToMusic(dir).Count > 0)
                {
                    this.Dict.Add(dir, MusicLists.ListFromFileToMusic(dir));
                    foreach(Musics music in MusicLists.ListFromFileToMusic(dir))
                    {
                        Logger.Instance.Log(music, Logger.Events.LoadFromDirectory, "Test", "Result");
                    }
                }
            }

        }

        public void FillDict() //Fill a dictionnary with all the subfolders path coming from the path and link it to the list from CreateList
        {

        }

        public void LoadFromFolder()
        {

        }

        public void ReadTags()
        {

        }

        public void WriteTags()
        {

        }

        public void Reorganization()
        {

        }
#endregion

    }
}
