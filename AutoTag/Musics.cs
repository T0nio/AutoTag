using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTag
{
    class Musics
    {
        private Tags oldFile;
        private Tags newFile;
        public string FilePath{ get; set; }

        public Musics(string path)
        {
            FilePath = path;
            oldFile = new Tags(path);
        }

        public void ReadTags()
        {

        }

        public void WriteTags()
        {

        }

        public void Reorganize(string option, string format)
        {

        }
    }
}
