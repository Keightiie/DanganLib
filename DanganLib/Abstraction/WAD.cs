using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanganLib.Abstraction
{
    public class WAD
    {
        public int FileCount { get; private set; }
        int MajorVersion { get; set; }
        int MinorVersion { get; set; }

        public void LoadFile(string filePath)
        {

        }

        public void SaveFile(string savePath)
        {

        }

        ///<summary>
        ///Exports the contents of a wad file to a specified folder.
        ///</summary>
        public void Export(string exportPath)
        {

        }


    }
}
