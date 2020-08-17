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
        public class Command
        {
            public byte OpCode;
            public byte[] args;

            public Command(byte OpCode, params byte[] args)
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
        public List<Command> Commands = new List<Command>();
        public List<string> Text = new List<string>();

        public Lin(int game = 1)
        {

        }

        public Lin(BinaryReader br, int game = 1)
        {
            if (game > 3)
                game = 3;
            Game = game;
            ParseDR2(br);

        }

        public Lin(string path, int game = 1)
        {
            if (game > 3)
                game = 3;
            Game = game;
            ParseDR2(new BinaryReader(new FileStream(path, FileMode.Open)));

        }

        public void ParseDR2(BinaryReader br)
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
                //Console.WriteLine(op.ToString("X2"));
                Console.WriteLine($"0x{op.ToString("X2")}");
                int argCount = GameArgs[Game][op];

                if (argCount == -1)
                {
                    argCount = 0;
                    while (br.ReadByte() != 0x70)
                    {
                        argCount++;
                    }
                    br.BaseStream.Position -= (argCount + 1);
                    //Console.WriteLine($"Position: {br.BaseStream.Position}, Args Count: {argCount}");
                }
                //Console.WriteLine($"Position: {br.BaseStream.Position}, Args Count: {argCount}");

                byte[] args = new byte[argCount];


                for (int i = 0; i < argCount; i++)
                {
                    args[i] = br.ReadByte();
                }

                Command cmd = new Command(op, args);
                Commands.Add(cmd);

                if(br.BaseStream.Position != br.BaseStream.Length)
                b = br.ReadByte();
                else { b = 0; }
            }
            br.BaseStream.Position -= 1;

            if(Type == 2)
            {
                while (br.ReadByte() == 0) ;
                br.BaseStream.Position -= 1;
                TextCount = br.ReadInt32();

                int[] TextSize = new int[TextCount];

                for (int i = 0; i < TextCount; i++)
                {
                    TextSize[i] = br.ReadInt32();
                }

                while (br.ReadInt16() == 0) ;

                for (int i = 0; i < TextCount; i++)
                {

                    string text = "";
                    br.BaseStream.Position = TextTablePosition + TextSize[i];
                    //Console.WriteLine($"Position: {br.BaseStream.Position} - Right before count");
                    int size;

                    if (i == TextCount - 1)
                        size = (((int)br.BaseStream.Length - TextTablePosition - TextSize[i]) / 2) - 2;
                    else size = ((TextSize[i + 1] - TextSize[i]) / 2) - 2;
                    br.ReadInt16();

                    for (int x = 0; x < size; x++)
                    {
                        byte[] bytes = br.ReadBytes(2);
                        if (bytes[0] == 0x0A && bytes[1] == 0x00)
                        {
                            text += @"\n";
                        }
                        else
                        {
                            text += Encoding.Unicode.GetString(bytes);
                        }

                    }
                    //Console.WriteLine(text);
                    Text.Add(text);

                }
            }
            
        }


        public void AddCommand(byte opcode, params byte[] args)
        {
            Command cmd = new Command(opcode, args);
            Commands.Add(cmd);
        }

        public byte[] AddText(string text)
        {
            byte[] result = new byte[2];
            for (int i = 0; i < Text.Count; i++)
            {
                if (Text[i] == text)
                {
                    result[0] = (byte)(i >> 8);
                    result[1] = (byte)(i & 255);

                    return getShortBytes((short)i); ;
                }
            }

            
            text = text.Replace("\\n", "\n");
            Text.Add(text);
            return getShortBytes((short)(Text.Count - 1));
        }

        public void Build(string output)
        {
            if (File.Exists(output))
            {
                throw new SystemException("File already exists.");
            }
            if (!File.Exists(output))
            {
                var file = File.Create(output);
                file.Close();
            }
            BinaryWriter bw = new BinaryWriter(new FileStream(output, FileMode.Open));

            //Header generation.
            if(Text.Count > 0) Type = 2;
            if (Type == 2) HeaderSize = 16;
            FileSize = 0;
            TextTablePosition = 0;

            //Header writing
            bw.Write(Type);
            bw.Write(HeaderSize);
            if (Type == 2) bw.Write(TextTablePosition);
            bw.Write(FileSize);


            WriteOP(new Command(0x00, getShortBytes((short)Text.Count)), bw);

            foreach(Command cmd in Commands)
            {
                WriteOP(cmd, bw);
            }

            if(Type == 2)
            {
                while (bw.BaseStream.Position % 4 != 0)
                {
                    bw.Write((byte)0x00);
                }
                TextTablePosition = (int)bw.BaseStream.Position;
                bw.Write(Text.Count);

                int position = 8 + (4 * Text.Count);

                foreach (var text in Text)
                {
                    bw.Write(position);
                    position += 2 + (text.Length * 2) + 2;
                }

                bw.Write(0);

                foreach (var text in Text)
                {
                    bw.Write((byte)0xFF);
                    bw.Write((byte)0xFE);
                    bw.Write(Encoding.Unicode.GetBytes(text));
                    bw.Write(new byte[] { 0x00, 0x00 });
                }
                bw.Write((byte)0x00);

            }


            bw.Write((byte)0x00);
            bw.BaseStream.Position = 8;
            if (Type == 2)
            {
                bw.Write(TextTablePosition);
            }
            bw.Write((int)bw.BaseStream.Length);




            bw.Close();


        }

        private void WriteOP(Command cmd, BinaryWriter bw)
        {
            bw.Write((byte)0x70);
            bw.Write(cmd.OpCode);

            //if(cmd.args != null)
            //{
            foreach (byte arg in cmd.args)
            {

                bw.Write(arg);
            }
            //}

        }

        public byte[] getShortBytes(short num)
        {
            byte[] result = new byte[2];
            result[0] = (byte)(num >> 8);
            result[1] = (byte)(num & 255);
            return result;
        }

    }
}
