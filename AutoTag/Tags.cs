using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTag
{
    public class Tags : Mp3Lib.Mp3File
    {
        private string path;

        public Tags(string file):base(file)
        {
            path = file;
        }

        public void ReadTagFromFile()
        {

        }

        public void ReadTagFromFolderPath()
        {

        }

        public void ReadTagFromACR()
        {

        }

        public void ReadTagFromAPI()
        {

        }
    }
}
