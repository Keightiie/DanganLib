using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DanganLib.Dangan.Anniversary
{
    class OBB
    {
        BinaryReader OBBFile;


        public string Signature { get; private set; }
        public uint FileCount = 0;

        private bool ValidSignature() { return Signature == "TP"; }

        public List<Archive.FileEntry> Files = new List<Archive.FileEntry>();

        public OBB(BinaryReader BinaryFile)
        {
            OBBFile = BinaryFile ?? throw new ArgumentException("File already loaded, can't open more then one file at a time.", "br");

            ParseFileEntries();

        }

        void ParseFileEntries()
        {
            Signature = Encoding.UTF8.GetString(OBBFile.ReadBytes(2));

            if (!ValidSignature()) return;

            OBBFile.ReadBytes(5); //Unknown 5 bytes
            FileCount = OBBFile.ReadUInt32();

            for (int i = 0; i < FileCount; i++)
            {
                Archive.FileEntry FileEntry = new Archive.FileEntry();
                FileEntry.Name = OBBFile.ReadString();
                FileEntry.Offset = OBBFile.ReadUInt32();
                FileEntry.Size = OBBFile.ReadUInt32();

                Files.Add(FileEntry);
            }


        }











    }
}
