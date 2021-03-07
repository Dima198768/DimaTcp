using System;
using System.Net.Sockets;
using System.Text;

namespace ServerTCP
{
    public class ClientClass
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerClass server; //обєкт сервера

        public ClientClass(TcpClient tcpClient, ServerClass serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // отримуємо ім'я користувача
                string message = GetMessage();
                userName = message;

                message = userName + " get in chat";
                //посилаємо сповіщення про вхід в чат іншим користувачам
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);
                //в безкінечному циклі получаємо сповіщення від користувача
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", userName, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format("{0}: leave from chat", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в випадку виходу із циклу закриваємо ресурси
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // читання вхідного сповіщення і перетворення його в строку
        public string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для отримання даних
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закриття підключення
        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
