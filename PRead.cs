using System.Text;

namespace Decrypt
{
    public class PRead
    {
        private FileStream? FileStream;
        public Dictionary<string, FileEntry> TableIndex;
        public struct FileEntry
        {
            public uint Position;
            public uint Length;
            public uint Key;
        }
        public PRead(string Filename)
        {
            FileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            TableIndex = new Dictionary<string, FileEntry>();
            FileStream.Position = 0L;
            byte[] array = new byte[1024];
            FileStream.Read(array, 0, 1024);
            int num = 0;
            for (int i = 3; i < 255; i++)
            {
                num += BitConverter.ToInt32(array, i * 4);
            }
            byte[] array2 = new byte[16 * num];
            FileStream.Read(array2, 0, array2.Length);
            dd(array2, 16 * num, BitConverter.ToUInt32(array, 212));
            int num2 = BitConverter.ToInt32(array2, 12) - (1024 + 16 * num);
            byte[] array3 = new byte[num2];
            FileStream.Read(array3, 0, array3.Length);
            dd(array3, num2, BitConverter.ToUInt32(array, 92));
            Init2(array2, array3, num);
            if (Filename.ToLower().EndsWith("adult.dat"))
            {
                TableIndex.Remove("def/version.txt");
            }
        }
        ~PRead()
        {
            if (FileStream != null)
            {
                FileStream.Close();
                FileStream = null;
            }
        }

        protected void Init2(byte[] rtoc, byte[] rpaths, int numfiles)
        {
            int num = 0;
            for (int i = 0; i < numfiles; i++)
            {
                int num2 = 16 * i;
                uint l = BitConverter.ToUInt32(rtoc, num2);
                int num3 = BitConverter.ToInt32(rtoc, num2 + 4);
                uint k = BitConverter.ToUInt32(rtoc, num2 + 8);
                uint p = BitConverter.ToUInt32(rtoc, num2 + 12);
                int num4 = num3;
                while (num4 < rpaths.Length && rpaths[num4] != 0)
                {
                    num4++;
                }
                string key = Encoding.ASCII.GetString(rpaths, num, num4 - num).ToLower();
                    FileEntry value = default;
                value.Position = p;
                value.Length = l;
                value.Key = k;
                TableIndex.Add(key, value);
                num = num4 + 1;
            }
        }

        private void gk(byte[] b, uint k0)
        {
            uint num = k0 * 5892U + 41280U;
            uint num2 = num << 7 ^ num;
            for (int i = 0; i < 256; i++)
            {
                num -= k0;
                num += num2;
                num2 = num + 341U;
                num *= num2 & 220U;
                b[i] = (byte)num;
                num >>= 2;
            }
        }

        protected void dd(byte[] b, int L, uint k)
        {
            byte[] array = new byte[256];
            gk(array, k);
            for (int i = 0; i < L; i++)
            {
                byte b2 = b[i];
                b2 ^= array[i % 235];
                b2 += 31;
                b2 += array[i % 87];
                b2 ^= 165;
                b[i] = b2;
            }
        }

        public byte[]? Data(string Filename)
        {
            if (!TableIndex.TryGetValue(Filename, out FileEntry fe) || FileStream == null)
            {
                return null;
            }
            FileStream.Position = (long)(ulong)fe.Position;
            byte[] array = new byte[fe.Length];
            FileStream.Read(array, 0, array.Length);
            dd(array, array.Length, fe.Key);
            return array;
        }

        public void ShowFileEntries()
        {
            foreach (var entry in TableIndex)
            {
                Console.WriteLine($"{entry.Key}: Position={entry.Value.Position}, Length={entry.Value.Length}, Key={entry.Value.Key}");
            }
        }
    }
}
