using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<FileEntry> Files = new List<FileEntry>();
        public List<DirectoryEntry> Directories = new List<DirectoryEntry>();
        #endregion

        FileStream File;


        #region ClassDeclaration 
        public WAD() { }

        ///<summary>
        ///Creates a WAD variable after parsing an existing file.
        ///</summary>
        public WAD(FileStream fs) {
            File = fs ?? throw new ArgumentException("Can't load more then one wad at a time.", "fs");
            BinaryReader wadBR = new BinaryReader(File);
            ParseFile(wadBR);

        }
        #endregion

        #region ParseData
        void ParseFile(BinaryReader wadBR)
        {
            Signature = Encoding.UTF8.GetString(wadBR.ReadBytes(4));
            MajorVersion = wadBR.ReadInt32();
            MinorVersion = wadBR.ReadInt32();
            HeaderSize = wadBR.ReadInt32();
            Header = wadBR.ReadBytes(HeaderSize);
            int fileCount = wadBR.ReadInt32();

            for (int i = 0; i < fileCount; i++)
            {
                FileEntry file = new FileEntry();
                file.External = false;
                file.Name = Encoding.ASCII.GetString(wadBR.ReadBytes(wadBR.ReadInt32()));
                file.Size = wadBR.ReadInt64();
                file.Offset = wadBR.ReadInt64();
                Files.Add(file);
            }
            //Console.WriteLine(wadBR.BaseStream.Position);
            int directoryCount = wadBR.ReadInt32();

            Console.WriteLine(directoryCount);

            for (int i = 0; i < directoryCount; i++)
            {
                DirectoryEntry directory = ParseDirEntry(wadBR);
                Directories.Add(directory);
            }

            Console.WriteLine(wadBR.BaseStream.Position);

        }

        SubFileEntry ParseSubfile(BinaryReader wadBR)
        {
            SubFileEntry subfile = new SubFileEntry();
            subfile.Name = Encoding.ASCII.GetString(wadBR.ReadBytes(wadBR.ReadInt32()));
            subfile.IsDirectory = wadBR.ReadByte() == 1;
            return subfile;
        }

        DirectoryEntry ParseDirEntry(BinaryReader wadBR)
        {
            DirectoryEntry directory = new DirectoryEntry();
            directory.Name = Encoding.ASCII.GetString(wadBR.ReadBytes(wadBR.ReadInt32()));
            int SubfileCount = wadBR.ReadInt32();

            for (int i = 0; i < SubfileCount; i++)
            {
                SubFileEntry subEntry = ParseSubfile(wadBR);
                directory.Subfiles.Add(subEntry);
            }
            return directory;
        }
        #endregion

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

        public class FileEntry
        {
            public string Name { get; set; }
            public long Size { get; set; }
            public long Offset { get; set; }
            public bool External { get; set; }
            public string source { get; set; }
        }

        public class DirectoryEntry
        {
            public string Name { get; set; }
            public List<SubFileEntry> Subfiles { get; set; } = new List<SubFileEntry>();
        }

        public class SubFileEntry
        {
            public string Name { get; set; }
            public bool IsDirectory { get; set; }
        }

    }
}
