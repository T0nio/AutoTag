using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTagGUI
{
    public class MusicListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<BoolStringClass> AllowedExtensions {
            get
            {
                return new ObservableCollection<BoolStringClass>
                {
                    new BoolStringClass {IsSelected = true, Content = ".mp3" },
                    new BoolStringClass {IsSelected = false, Content = ".wav" },
                    new BoolStringClass {IsSelected = false, Content = ".flac" },
                    new BoolStringClass {IsSelected = false, Content = ".aac"},
                    new BoolStringClass {IsSelected = false, Content = ".wma"},
                    new BoolStringClass {IsSelected = false, Content = ".ogg"},
                };
            }
        }

        public Dictionary<string, List<string>> Musics
        {
            get
            {
                return new Dictionary<string, List<string>>
                {
                    { "Directory1", new List<string> { "File1", "File2"} },
                    { "Directory2", new List<string> {"File3", "File4", "File5"} },
                };
            }
        }

        public class BoolStringClass
        {
            public bool IsSelected { get; set; }
            public string Content { get; set; }
        }
    }
}
