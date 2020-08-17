using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanganLib
{
    public class Archive
    {
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
    }
}
