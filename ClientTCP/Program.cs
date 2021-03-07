using ServerTCP;
using System;
using System.Threading;

namespace ClientTCP
{
    class Program
    {
        static ServerClass server; // сервер
        static Thread listenThread; // потік для прослуховування
        static void Main(string[] args)
        {
            try
            {
                server = new ServerClass();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потоку
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
