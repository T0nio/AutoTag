using Id3Lib;
using Mp3Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTag
{
    class Musics
    {
        public TagHandler OldFile { get; private set; }
        public TagHandler NewFile { get; set; }
        public Mp3File File { get; private set; }

        public Musics(string path)
        {
            File = new Mp3File(path);
            OldFile = File.TagHandler;
            NewFile = OldFile;
        }

        public void ReadTags()
        {

        }

        public void WriteTags()
        {
            File.TagHandler = NewFile;
            File.Update();
        }

        public void Reorganize(string option, string format)
        {

        }
    }
}
