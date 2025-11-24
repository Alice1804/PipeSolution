using System;
using System.IO;

namespace Shared
{
    public class StreamRW : IDisposable
    {
        private Stream ioStream;

        public StreamRW(Stream stream)
        {
            ioStream = stream;
        }
        public void WriteData(Tuple<string, string> data)
        {
            byte[] outBuffer;
            using (MemoryStream m = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(data.Item1);
                writer.Write(data.Item2);
                outBuffer = m.ToArray();
            }

            int len = outBuffer.Length;
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();
        }
        public Tuple<string, string> ReadData()
        {
            try
            {
                int len = ioStream.ReadByte() * 256;
                len += ioStream.ReadByte();
                byte[] inBuffer = new byte[len];
                ioStream.Read(inBuffer, 0, len);

                using (MemoryStream m = new MemoryStream(inBuffer))
                using (BinaryReader reader = new BinaryReader(m))
                {
                    return new Tuple<string, string>(reader.ReadString(), reader.ReadString());
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new Tuple<string, string>("Error", "Error");
            }
        }
        public void Dispose()
        {
            ioStream?.Dispose();
        }
    }
}
