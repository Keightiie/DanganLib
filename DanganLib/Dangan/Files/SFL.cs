using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanganLib.Dangan.Files
{
    class SFL
    {
        public SFL(BinaryReader fs)
        {
            CheckHeader(fs);
        }

        bool CheckHeader(BinaryReader br)
        {
            if (Encoding.ASCII.GetString(br.ReadBytes(4)) != "LLFS") return false;
            if (br.ReadInt32() != 3) return false;


            return true;
        }
    }
}
