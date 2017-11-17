using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace PR_Client_CaesarCipher
{
    class Client
    {
        private const int portNumber = 11000;
        private IPAddress ipAddress;
        private TcpClient tcpClient;
        private NetworkStream netStream;

        // Данные, отправляемые на сервер
        public int shiftToServer { get; set; }
        public string decryptedData { get; set; }
        public string encryptedData { get; set; }
        // Строитель диаграммы
        public DiagramBuilder diagramBuilder { get; set; }
        public Client()
        {
            try
            {
                shiftToServer = 0;
                decryptedData = "";
                encryptedData = "";
            }
            catch
            {
                // Закрыть приложение, если сессия не установлена
                Environment.Exit(1);
            }
        }
        public string SendAndReceiveDataWithServer(string data)
        {
            try
            {
                string dataIP = "";
                // Считка с файла IP-адреса сервера
                using (StreamReader sr = new StreamReader(File.Open("connection.txt", FileMode.Open)))
                {
                    while (!sr.EndOfStream)
                        dataIP += sr.ReadLine() + '\n';
                }

                // IP хоста (убираю символ '\n')
                string ipHostAddress = dataIP.Remove(dataIP.Length - 1, 1);

                ipAddress = IPAddress.Parse(ipHostAddress);

                // Создаю сессию с сервером (подключение)
                tcpClient = new TcpClient(ipAddress.ToString(), portNumber);
                netStream = tcpClient.GetStream();


                // ВЗАИМОДЕЙСТВИЕ С СЕРВЕРОМ
                byte[] dataToServer = Encoding.Unicode.GetBytes(data);

                // Отправка данных серверу
                netStream.Write(dataToServer, 0, dataToServer.Length);

                byte[] dataFromServer = new byte[64];
                StringBuilder sb = new StringBuilder();
                int bytesReceivedCount = 0;

                // Ответ от сервера
                do
                {
                    bytesReceivedCount = netStream.Read(dataFromServer, 0, dataFromServer.Length);
                    sb.Append(Encoding.Unicode.GetString(dataFromServer, 0, bytesReceivedCount));
                }
                while (netStream.DataAvailable);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}