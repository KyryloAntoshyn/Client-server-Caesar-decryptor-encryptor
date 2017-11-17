using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace PR_MultiThreadedServer
{
    class TcpServer
    {
        private IPAddress ipAddress;
        private const int portNumber = 11000;
        private TcpListener listener;
        public TcpServer()
        {
            try
            {
                ipAddress = IPAddress.Any;
                listener = new TcpListener(ipAddress, portNumber);
                listener.Start();
                Console.WriteLine("I'm waiting for connections...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void SendAndReceiveData()
        {
            try
            {
                // Ожидание подключений
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient(); // Прием сокета клиента
                    Console.WriteLine("Connection was established!");
                    ClientObject clientObj = new ClientObject(client);

                    // Обработка пользователя в новом потоке, чтобы не ждать очереди, пока сервер обрабатывает клиента
                    Thread myThread = new Thread(new ThreadStart(clientObj.Process));
                    myThread.Start();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }
}