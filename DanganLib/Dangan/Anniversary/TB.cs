using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DanganLib.Dangan.Anniversary
{
    public class TB
    {
        BinaryReader TBFile;
        BinaryWriter TBFileWriter;


        public string Signature { get; private set; }
        public uint FileCount = 0;

        private bool ValidSignature() { return Signature == "TP"; }

        public List<Archive.FileEntry> FileEntries = new List<Archive.FileEntry>();

        public TB()
        {

        }


        public void Parse(BinaryReader BinaryFile)
        {
            TBFile = BinaryFile ?? throw new ArgumentException("Alreading operating on a file.", "br");

            Signature = Encoding.UTF8.GetString(TBFile.ReadBytes(2));

            if (!ValidSignature()) return;

            TBFile.ReadBytes(5); //Unknown bytes
            FileCount = TBFile.ReadUInt32();

            for (int i = 0; i < FileCount; i++)
            {
                Archive.FileEntry FileEntry = new Archive.FileEntry();
                FileEntry.Name = TBFile.ReadString();
                FileEntry.Offset = TBFile.ReadUInt32();
                FileEntry.Size = TBFile.ReadUInt32();
                TBFile.ReadBytes(16); //Unknown bytes

                FileEntries.Add(FileEntry);
            }


        }

        public void Export(string exportPath)
        {
            if (TBFile == null)
                throw new InvalidDataException("There is no file loaded to extract from.");

            for (int i = 0; i < FileEntries.Count; i++)
            {
                Directory.CreateDirectory($"{exportPath}/{Path.GetDirectoryName(FileEntries[i].Name)}");
                TBFile.BaseStream.Position = FileEntries[i].Offset;
                var NewFile = File.Create($"{exportPath}/{FileEntries[i].Name}");
                BinaryWriter FileWriter = new BinaryWriter(NewFile);
                FileWriter.Write(TBFile.ReadBytes((int)FileEntries[i].Size));
                FileWriter.Close();
            }

        }

        public void Pack(string ImportPath, string ExportPath)
        {

            if (TBFile != null) throw new ArgumentException("A file is already being opperated on.", "br");

            uint offsetLocation = 0;
            uint FileEntriesEnd = 0;

            TBFileWriter = new BinaryWriter(new FileStream(ExportPath, FileMode.Create));
            TBFileWriter.Write(Encoding.UTF8.GetBytes("TP"));
            TBFileWriter.Write((byte)1);

            for (int i = 0; i < 4; i++) 
            {
                TBFileWriter.Write((byte)0);
            }


            string[] allfiles = Directory.GetFiles($"{ImportPath}\\", "*.*", SearchOption.AllDirectories);
            TBFileWriter.Write((uint)allfiles.Count());

            foreach (var file in allfiles)
            {
                FileInfo info = new FileInfo(file);
                string str = info.FullName.Remove(0, ImportPath.Length + 1);
                TBFileWriter.Write(str);
                TBFileWriter.Write(offsetLocation);
                TBFileWriter.Write((uint)info.Length);
                for (int i = 0; i < 16; i++)
                {
                    TBFileWriter.Write((byte)0);
                }

            }

            FileEntriesEnd = (uint)TBFileWriter.BaseStream.Position;
            TBFileWriter.BaseStream.Position = 3;
            TBFileWriter.Write(FileEntriesEnd - 7);

            TBFileWriter.BaseStream.Position = 11;
            foreach (var file in allfiles)
            {
                FileInfo info = new FileInfo(file);
                string str = info.FullName.Remove(0, ImportPath.Length + 1);
                str = str.Replace("\\", "/");
                TBFileWriter.Write(str);
                TBFileWriter.Write(FileEntriesEnd);
                FileEntriesEnd += (uint)info.Length;
                TBFileWriter.Write((uint)info.Length);
                for (int i = 0; i < 16; i++)
                {
                    TBFileWriter.Write((byte)0);
                }

            }

            foreach (var file in allfiles)
            {
                FileInfo info = new FileInfo(file);
                BinaryReader ImportFile = new BinaryReader(new FileStream(info.FullName, FileMode.Open));
                TBFileWriter.Write(ImportFile.ReadBytes((int)ImportFile.BaseStream.Length));
                ImportFile.Close();

            }
            TBFileWriter.Close();


            TBFile = new BinaryReader(new FileStream($"{ImportPath}_packed.obb", FileMode.Open));
        }

        public void Close() 
        {
            if (TBFile == null) throw new ArgumentException("No file to close.", "br");
        }

    }
}
