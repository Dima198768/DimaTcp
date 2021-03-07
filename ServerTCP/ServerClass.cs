using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerTCP
{
    public class ServerClass
    {
        static TcpListener tcpListener; // сервер для прослуховування
        List<ClientClass> clients = new List<ClientClass>(); // всі подключення

        public void AddConnection(ClientClass clientObject)
        {
            clients.Add(clientObject);
        }
        public void RemoveConnection(string id)
        {
            // получаеємо по id закрите  підключення
            ClientClass client = clients.FirstOrDefault(c => c.Id == id);
            // і видаляємо його зі списку підключень
            if (client != null)
                clients.Remove(client);
        }
        // прослуховування вхідних підключень
        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Server started. Waiting for users...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientClass clientObject = new ClientClass(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // трансляція повідомлення підключеним клієнтам
        public void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id) // якщо id клиієнта не дорівнює id відправляючого
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача даних
                }
            }
        }
        //відключення всіх клієнтів
        public void Disconnect()
        {
            tcpListener.Stop(); //зупинка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //відключення клієнта
            }
            Environment.Exit(0); //завершення процесу
        }
    }
}