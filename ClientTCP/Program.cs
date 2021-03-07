using ServerTCP;
using System;
using System.Threading;

namespace ClientTCP
{
    class Program
    {
        static ServerClass server; // сервер
        static Thread listenThread; // gjnsr lkz ghjcke[jdedfyyz
        static void Main(string[] args)
        {
            try
            {
                server = new ServerClass();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потокe
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
