using System;

namespace QuadMiner
{
    class Program
    {
        static Pool _pool = null;
        static Work _work = null;
        static uint _nonce = 0;
        static long _maxAgeTicks = 20000 * TimeSpan.TicksPerMillisecond;
        static uint _batchSize = 200000;

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    _pool = SelectPool();
                    _work = GetWork();
                    while (true)
                    {
                        if (_work == null || _work.Age > _maxAgeTicks)
                            _work = GetWork();

                        if (_work.FindShare(ref _nonce, _batchSize))
                        {
                            SendShare(_work.Current);
                            _work = null;
                        }
                        else
                            PrintCurrentState();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.Write("ERROR: ");
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine();
                Console.Write("Hit 'Enter' to try again...");
                Console.ReadLine();
            }
        }


        private static void ClearConsole()
        {
            Console.Clear();
            Console.WriteLine("*****************************");
            Console.WriteLine("*** Quad Miner v0.1.1 SHA ***");
            Console.WriteLine("*****************************");
            Console.WriteLine();
        }

        private static Pool SelectPool()
        {
            ClearConsole();
            Print("Chose a Mining Pool 'user:password@url:port' or leave empty to skip.");
            Console.Write("Select Pool: ");
            string login = ReadLineDefault("quadrat1c.001:123@localhost:1337");
            return new Pool(login);
        }

        private static Work GetWork()
        {
            ClearConsole();
            Print("Requesting Work from Pool...");
            Print("Server URL: " + _pool.Url.ToString());
            Print("User: " + _pool.User);
            Print("Password: " + _pool.Password);
            return _pool.GetWork();
        }

        private static void SendShare(byte[] share)
        {
            ClearConsole();
            Print("*** Found Valid Share ***");
            Print("Share: " + Utils.ToString(_work.Current));
            Print("Nonce: " + Utils.ToString(_nonce));
            Print("Hash: " + Utils.ToString(_work.Hash));
            Print("Sending Share to Pool...");
            if (_pool.SendShare(share))
                Print("Server accepted the Share!");
            else
                Print("Server declined the Share!");

            Console.Write("Hit 'Enter' to continue...");
            Console.ReadLine();
        }

        private static DateTime _lastPrint = DateTime.Now;
        private static void PrintCurrentState()
        {
            ClearConsole();
            Print("Data: " + Utils.ToString(_work.Data));
            string current = Utils.ToString(_nonce);
            string max = Utils.ToString(uint.MaxValue);
            double progress = ((double)_nonce / uint.MaxValue) * 100;
            Print("Nonce: " + current + "/" + max + " " + progress.ToString("F2") + "%");
            Print("Hash: " + Utils.ToString(_work.Hash));
            TimeSpan span = DateTime.Now - _lastPrint;
            Print("Speed: " + (int)(((_batchSize) / 1000) / span.TotalSeconds) + "Kh/s");
            _lastPrint = DateTime.Now;
        }

        private static void Print(string msg)
        {
            Console.WriteLine(msg);
            Console.WriteLine();
        }

        private static string ReadLineDefault(string defaultValue)
        {
            //Allow Console.ReadLine with a default value
            string userInput = Console.ReadLine();
            Console.WriteLine();
            if (userInput == "")
                return defaultValue;
            else
                return userInput;
        }
    }
}
