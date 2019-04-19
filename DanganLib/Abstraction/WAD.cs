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
        #region FileInfo
        public string Signature { get; private set; } = "AGAR";
        public int MajorVersion { get; private set; } = 1;
        public int MinorVersion { get; private set; } = 1;

        int HeaderSize { get; set; } = 0;
        byte[] Header { get; set; }

        public int FileCount { get; private set; } = 0;
        List<FileEntry> files = new List<FileEntry>();
        public int DirCount { get; private set; }
        #endregion

        FileStream file;



        public WAD() { }

        public WAD(FileStream fs) {
            file = fs ?? throw new ArgumentException("Can't load more then one wad at a time.", "fs");
            BinaryReader wadBR = new BinaryReader(file);
            ParseFile(wadBR);

        }


        void ParseFile(BinaryReader wadBR)
        {
            Signature = Encoding.UTF8.GetString(wadBR.ReadBytes(4));
            MajorVersion = wadBR.ReadInt32();
            MinorVersion = wadBR.ReadInt32();
            HeaderSize = wadBR.ReadInt32();
            Header = wadBR.ReadBytes(HeaderSize);
            FileCount = wadBR.ReadInt32();

            for(int i = 0; i < FileCount; i++)
            {
                FileEntry fileE = new FileEntry();
                fileE.External = false;
                int nameLength = wadBR.ReadInt32();
                fileE.FileName = Encoding.ASCII.GetString(wadBR.ReadBytes(nameLength));
                fileE.FileSize = wadBR.ReadInt64();
                fileE.FileOffset = wadBR.ReadInt64();
                files.Add(fileE);

            }


        }

        void ParseSubfile()
        {

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
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public long FileOffset { get; set; }
        public bool External { get; set; }
        public string source { get; set; }
    }

    class DirectoryEntry
    {
        string DirectoryName { get; set; }
        int SubfileCount { get; set; }
        List<SubFileEntry> Subfiles { get; set; }
    }

    class SubFileEntry
    {
        string SubfileName { get; set; }
        bool IsDirectory { get; set; }
    }

}
