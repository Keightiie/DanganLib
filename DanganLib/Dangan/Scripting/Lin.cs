using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanganLib.Dangan.Scripting
{
    public class Lin
    {
        class Command
        {
            byte OpCode;
            byte[] args;

            public Command(byte OpCode, byte[] args)
            {
                this.OpCode = OpCode;
                this.args = args;
            }

        }


        int Type = 1;
        int Game = 1;
        private int HeaderSize = 12;
        private int TextTablePosition = 0;
        int TextCount = 0;
        private int FileSize = 0;
        Dictionary<byte, int>[] GameArgs = new Dictionary<byte, int>[]
        {
            Games.DR1.ArgsSize,
            Games.DR2.ArgsSize,
            Games.UDG.ArgsSize
        };
        List<Command> Commands = new List<Command>();
        List<string> Text = new List<string>();


        public Lin(BinaryReader br, int game = 1)
        {
            if (game > 3)
                game = 3;
            Game = game;
            Parse(br);

        }

        public void Parse(BinaryReader br)
        {
            Commands = new List<Command>();
            Text = new List<string>();
            Type = br.ReadInt32();
            HeaderSize = br.ReadInt32();

            if(Type == 2)
            {
                TextTablePosition = br.ReadInt32();
            }

            FileSize = br.ReadInt32();

            byte b = br.ReadByte();

            while (b == 0x70)
            {

                byte op = br.ReadByte();
                byte[] args = new byte[GameArgs[Game][op]];


                for (int i = 0; i < GameArgs[Game][op]; i++)
                {
                    args[i] = br.ReadByte();
                }

                Command cmd = new Command(op, args);
                Commands.Add(cmd);

                b = br.ReadByte();
            }

            br.BaseStream.Position -= 1;

            TextCount = br.ReadInt32();

            int[] TextSize = new int[TextCount];

            for(int i = 0; i < TextCount; i++)
            {
                TextSize[i] = br.ReadInt32();
            }
            while (br.ReadInt16() == 0) ;

            for (int i = 0; i < TextCount; i++)
            {
                string text = "";
                br.BaseStream.Position = TextTablePosition + TextSize[i];



                int size;

                if (i == TextCount - 1)
                    size = (((int)br.BaseStream.Length - TextTablePosition - TextSize[i]) / 2) - 2;
                else  size = ((TextSize[i + 1] - TextSize[i]) / 2) - 2;

                br.ReadInt16();
                for (int x = 0; x < size; x++)
                {
                    text += Encoding.Unicode.GetString(br.ReadBytes(2));
                }
                Text.Add(text);
                Console.WriteLine(text);

            }

        }




}
}
