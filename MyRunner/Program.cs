using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameInterface;
using PostRunner;
using Virtualizer;

namespace MyRunner
{
    class Program
    {
        private readonly IPirateBot bot;
        static TcpListener server;
        static TcpClient client;
        static void Main(string[] args)
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5533);

            var procName = @"C:\Python27\python.exe";
            var procArgs =
                "\"lib\\playgame.py\" --loadtime 100000000 -e -E -d -O " +
                "--debug_in_replay --log_dir \"lib\\game_logs\" --map_file " +
                "\"maps\\default_map.map\" \"bots\\csharp\\ExampleBot\\MyBot.cs\" \"bots\\demoBot1.pyc\"";
            var workingDir = @"C:\Users\matma\Downloads\starter_kit\starter_kit";
            Process.Start(new ProcessStartInfo {
                FileName = procName,
                Arguments = procArgs,
                WorkingDirectory = workingDir
            });
            
            server.Start();
            client = server.AcceptTcpClient();
            var clientStream = client.GetStream();
            Console.SetOut(new StreamWriter(clientStream, Encoding.ASCII));
            Console.SetIn(new StreamReader(clientStream, Encoding.ASCII));
            
            if (!args.Any())
            {
                Console.Error.WriteLine("USAGE: This exe should get a 'bot' path as a single parameter");
                return;
            }
            string str = args[0];
            try
            {
                new Runner(new MyBot.MyBot(), clientStream).Run();
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Console.Error.WriteLine(string.Format("Failure while running bot from [{0}]: {1}", str, exception.Message));
                Console.Error.WriteLine(exception.StackTrace);
            }
        }
    }
}
