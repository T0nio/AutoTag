using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTag
{
    class Musics
    {
        private string filePath;
        private Tags oldFile;
        private Tags newFile;

        public Musics(string path)
        {
            filePath = path;
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
