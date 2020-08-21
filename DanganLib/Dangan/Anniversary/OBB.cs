using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom;

namespace DanganLib.Dangan.Anniversary
{
    public class OBB
    {
        BinaryReader OBBFile;
        BinaryWriter OBBFileWriter;


        public string Signature { get; private set; }
        public uint FileCount = 0;

        private bool ValidSignature() { return Signature == "TP"; }

        public List<Archive.FileEntry> FileEntries = new List<Archive.FileEntry>();

        public OBB()
        {

        }

        void Parse(BinaryReader BinaryFile)
        {
            OBBFile = BinaryFile ?? throw new ArgumentException("Alreading operating on a file.", "br");

            Signature = Encoding.UTF8.GetString(OBBFile.ReadBytes(2));

            if (!ValidSignature()) return;

            OBBFile.ReadBytes(5); //Unknown bytes
            FileCount = OBBFile.ReadUInt32();

            for (int i = 0; i < FileCount; i++)
            {
                Archive.FileEntry FileEntry = new Archive.FileEntry();
                FileEntry.Name = OBBFile.ReadString();
                FileEntry.Offset = OBBFile.ReadUInt32();
                FileEntry.Size = OBBFile.ReadUInt32();
                OBBFile.ReadBytes(16); //Unknown bytes

                FileEntries.Add(FileEntry);
            }


        }

        public void Export(string exportPath)
        {
            if (OBBFile == null)
                throw new InvalidDataException("There is no file loaded to extract from.");

            for (int i = 0; i < FileEntries.Count; i++)
            {
                Directory.CreateDirectory($"{exportPath}/{Path.GetDirectoryName(FileEntries[i].Name)}");
                OBBFile.BaseStream.Position = FileEntries[i].Offset;
                var NewFile = File.Create($"{exportPath}/{FileEntries[i].Name}");
                BinaryWriter FileWriter = new BinaryWriter(NewFile);
                FileWriter.Write(OBBFile.ReadBytes((int)FileEntries[i].Size));
                FileWriter.Close();
            }

        }

        public void Pack(string ImportPath)
        {

            if (OBBFile != null) throw new ArgumentException("A file is already being opperated on.", "br");

            uint offsetLocation = 0;
            uint FileEntriesEnd = 0;

            OBBFileWriter = new BinaryWriter(new FileStream($"{ImportPath}_packed.obb", FileMode.Create));
            OBBFileWriter.Write(Encoding.UTF8.GetBytes("TP"));
            OBBFileWriter.Write((byte)1);

            for (int i = 0; i < 4; i++) 
            {
                OBBFileWriter.Write((byte)0);
            }


            string[] allfiles = Directory.GetFiles($"{ImportPath}\\", "*.*", SearchOption.AllDirectories);
            OBBFileWriter.Write((uint)allfiles.Count());

            foreach (var file in allfiles)
            {
                FileInfo info = new FileInfo(file);
                string str = info.FullName.Remove(0, ImportPath.Length + 1);
                OBBFileWriter.Write(str);
                OBBFileWriter.Write(offsetLocation);
                OBBFileWriter.Write((uint)info.Length);
                for (int i = 0; i < 16; i++)
                {
                    OBBFileWriter.Write((byte)0);
                }

            }

            FileEntriesEnd = (uint)OBBFileWriter.BaseStream.Position;
            OBBFileWriter.BaseStream.Position = 3;
            OBBFileWriter.Write(FileEntriesEnd - 7);

            OBBFileWriter.BaseStream.Position = 11;
            foreach (var file in allfiles)
            {
                FileInfo info = new FileInfo(file);
                string str = info.FullName.Remove(0, ImportPath.Length + 1);
                str = str.Replace("\\", "/");
                OBBFileWriter.Write(str);
                OBBFileWriter.Write(FileEntriesEnd);
                FileEntriesEnd += (uint)info.Length;
                OBBFileWriter.Write((uint)info.Length);
                for (int i = 0; i < 16; i++)
                {
                    OBBFileWriter.Write((byte)0);
                }

            }

            foreach (var file in allfiles)
            {
                FileInfo info = new FileInfo(file);
                BinaryReader ImportFile = new BinaryReader(new FileStream(info.FullName, FileMode.Open));
                OBBFileWriter.Write(ImportFile.ReadBytes((int)ImportFile.BaseStream.Length));
                ImportFile.Close();

            }
            OBBFileWriter.Close();


            OBBFile = new BinaryReader(new FileStream($"{ImportPath}_packed.obb", FileMode.Open));
        }

        public void Close() 
        {
            if (OBBFile == null) throw new ArgumentException("No file to close.", "br");
        }

    }
}
