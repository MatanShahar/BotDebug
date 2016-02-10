using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GameInterface;

namespace PostRunner
{
    internal class IoHelper
    {
        private static IoHelper _instance;

        private readonly StreamReader reader;
        private readonly StreamWriter writer;

        public IoHelper(Stream stream)
        {
            this.reader = new StreamReader(stream, Encoding.ASCII);
            this.writer = new StreamWriter(stream, Encoding.ASCII);
        }

        public static string ReadLine()
        {
            return _instance.reader.ReadLine();
        }

        public static void Write(string text)
        {
            _instance.writer.Write(text);
        }

        public static void WriteLine(string text)
        {
            _instance.writer.WriteLine(text);
        }

        public static void Init(Stream stream)
        {
            _instance = new IoHelper(stream);
        }
    }

    public class Runner
    {
        private readonly IPirateBot bot;
        private readonly Stream client;

        public Runner(IPirateBot bot, Stream client)
        {
            this.bot = bot;
            this.client = client;
        }

        public void Run()
        {
            PirateGame pirateGame = null;
            string str = "";
            while (true)
            {
                try
                {
                    string str1 = Console.ReadLine().TrimEnd(new char[] { '\r', '\n' });
                    if (str1.ToLower() == "ready")
                    {
                        pirateGame = new PirateGame(str);
                        pirateGame.FinishTurn();
                        str = "";
                    }
                    else if (str1.ToLower() != "go")
                    {
                        str = string.Concat(str, str1, "\n");
                    }
                    else
                    {
                        pirateGame.Update(str);
                        if (!pirateGame.ShouldRecoverErrors())
                        {
                            this.bot.DoTurn(pirateGame);
                        }
                        else
                        {
                            try
                            {
                                this.bot.DoTurn(pirateGame);
                            }
                            catch (Exception exception1)
                            {
                                Exception exception = exception1;
                                pirateGame.Debug("Exception occured during doTurn: \n{0}",
                                    new object[] { exception.ToString() });
                            }
                        }
                        pirateGame.CancelCollisions();
                        pirateGame.FinishTurn();
                        str = "";
                    }

                }
                catch (IOException oException)
                {
                    break;
                }
                catch (Exception exception2)
                {
                    Console.Error.WriteLine(exception2.ToString());
                    Console.Error.Flush();
                }
                finally
                {
                    this.client.WriteByte(0);
                    this.client.Flush();
                }
            }
        }
    }
}
