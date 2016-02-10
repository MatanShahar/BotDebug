using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Virtualizer
{
    public class VirtualizerRunner
    {
        private static byte[] inBuffer;

        static VirtualizerRunner()
        {
            inBuffer = new byte[2048];
        }

        public static void Main(string[] args)
        {
            var client = new TcpClient();
            while (true)
            {
                try
                {
                    client.Connect("localhost", 5533);
                }
                catch (Exception e)
                {
                    Console.Beep();
                    Console.Error.WriteLine(e);
                    continue;
                }

                break;
            }

            Console.Error.WriteLine("Connected");

            var stream = client.GetStream();
            while (true)
            {
                string str1 = Console.ReadLine().TrimEnd(new char[] { '\r', '\n' });
                var buffer = Encoding.ASCII.GetBytes(str1 + Environment.NewLine);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();

                while (true)
                {
                    var length = stream.Read(inBuffer, 0, inBuffer.Length);
                    var data = Encoding.ASCII.GetString(inBuffer, 0, length);

                    if (length == 1 && data[0] == 0)
                        break;

                    Console.Write(data);
                }
            }
        }
    }
}
