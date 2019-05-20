using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DanganLib.Abstraction
{
    public class WAD
    {
        BinaryReader wadBR;

        #region FileInfo
        public string Signature { get; private set; } = "AGAR";
        public int MajorVersion { get; private set; } = 1;
        public int MinorVersion { get; private set; } = 1;

        int HeaderSize { get; set; } = 0;
        byte[] Header { get; set; }

        public List<FileEntry> Files = new List<FileEntry>();


        public List<DirectoryEntry> Directories = new List<DirectoryEntry>();
        long filesPosition { get; set; }
        #endregion

        #region ClassDeclaration 
        public WAD() { }

        ///<summary>
        ///Creates a WAD variable after parsing an existing file.
        ///</summary>
        public WAD(BinaryReader br) {
            wadBR = br ?? throw new ArgumentException("Can't load more then one wad at a time.", "br");
            
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
                file.Name = Encoding.ASCII.GetString(wadBR.ReadBytes(wadBR.ReadInt32()));
                file.Size = wadBR.ReadInt64();
                file.Offset = wadBR.ReadInt64();
                Files.Add(file);
            }

            int directoryCount = wadBR.ReadInt32();
            for (int i = 0; i < directoryCount; i++)
            {
                DirectoryEntry directory = ParseDirEntry(wadBR);
                Directories.Add(directory);
            }

            filesPosition = wadBR.BaseStream.Position;

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

        #region Data Management
        ///<summary>
        ///Exports the contents of a wad file to a folder.
        ///</summary>
        public void Export(string exportPath)
        {
            if (wadBR == null)
                throw new InvalidDataException("There is no data to extract from.");

            for (int i = 0; i < Directories.Count; i++)
            {
                Directory.CreateDirectory($"{exportPath}/{Directories[i].Name}");
            }

            for (int i = 0; i < Files.Count; i++)
            {
                wadBR.BaseStream.Position = filesPosition + Files[i].Offset;
                var file = File.Create($"{exportPath}/{Files[i].Name}");
                BinaryWriter bw = new BinaryWriter(file);
                bw.Write(wadBR.ReadBytes((int)Files[i].Size));

                file.Close();
            }


        }

        ///<summary>
        ///Specifies a file to be added to the compiled wad file. 
        ///</summary>
        public void AddFile(string input, string target)
        {

        }

        ///<summary>
        ///Specifies a folder to be added to the compiled wad file. 
        ///</summary>
        public void AddFolder(string input)
        {

        }

        ///<summary>
        ///Compiles everyting into into a wad file.
        ///</summary>
        public void Compile(string output)
        {

        }
        #endregion

        #region Data

        public void RemoveFileByPath(string path)
        {
            for(int i = 0; i < Files.Count; i++)
            {
                if(Files[i].Name == path)
                {
                    Files.RemoveAt(i);
                    RemoveSubEntryByPath(path);
                    return;
                }
            }
        }

        void RemoveSubEntryByPath(string path)
        {
            for (int i = 0; i < Directories.Count; i++)
            {
                if (Path.GetDirectoryName($"{Directories[i].Name}\\dummy.txt") == Path.GetDirectoryName(path))
                {
                    for (int x = 0; x < Directories[i].Subfiles.Count; x++)
                    {
                        if (Directories[i].Subfiles[x].Name == Path.GetFileName(path))
                        {
                            Directories[i].Subfiles.RemoveAt(x);
                        }
                    }
                }

            }
        }


        #endregion


        #region Classes
        public class FileEntry
        {
            public string Name { get; set; }
            public long Size { get; set; }
            public long Offset { get; set; }
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

        class ImportFiles
        {
            string FilePath { get; set; }
            string Target { get; set; }
            long fileSize { get; set; }
        }
        #endregion

    }
}
