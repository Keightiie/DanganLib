using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanganLib.Abstraction
{
    public class WAD
    {
        string Magic { get; set; } = "AGAR";

        public int FileCount { get; private set; }
        public int DirCount { get; private set; }
        public string filePath { get; set; }
        int MajorVersion { get; set; }
        int MinorVersion { get; set; }

        ///<summary>
        ///Exports the contents of a wad file to a folder.
        ///</summary>
        public void Export(string exportPath)
        {

        }

        ///<summary>
        ///Compiles the contents of a folder into a wad file.
        ///</summary>
        public void Compile(string compileDir, string exportPath)
        {

        }


    }
}
