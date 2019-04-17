using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DanganLib.Abstraction
{
    public class WAD
    {
        string Magic { get; set; } = "AGAR";
        public int MajorVersion { get; private set; } = 1;
        public int MinorVersion { get; private set; } = 1;
        public int HeaderSize { get; set; } = 0;
        byte[] Header { get; set; }
        public int FileCount { get; private set; } = 0;
        List<FileEntry> files = new List<FileEntry>();
        public int DirCount { get; private set; }

        FileStream file;



        public WAD() { }

        public WAD(FileStream fs) {
            file = fs;
            BinaryReader wadBR = new BinaryReader(file);
            ParseFile(wadBR);

        }


        void ParseFile(BinaryReader wadBR)
        {
            Magic = Encoding.UTF8.GetString(wadBR.ReadBytes(4));
            MajorVersion = wadBR.ReadInt32();
            MinorVersion = wadBR.ReadInt32();
            HeaderSize = wadBR.ReadInt32();
            Header = wadBR.ReadBytes(HeaderSize);
            FileCount = wadBR.ReadInt32();
        }



        ///<summary>
        ///Exports the contents of a wad file to a folder.
        ///</summary>
        public void Export(string exportPath)
        {

        }

        ///<summary>
        ///Compiles everyting into into a wad file.
        ///</summary>
        public void Compile(string compileDir, string exportPath)
        {

        }
        

    }

    class FileEntry
    {
        string FileName { get; set; }
        long FileSize { get; set; }
        long FileOffset { get; set; }
        bool External { get; set; }
        string source { get; set; }
    }

}
