using System;
using System.Text;
using System.Net.Sockets;

namespace PR_MultiThreadedServer
{
    // Класс для обработки отдельного подключения
    class ClientObject
    {
        private TcpClient client;
        public ClientObject(TcpClient client)
        {
            this.client = client;
        }
        public void Process()
        {
            NetworkStream netStream = null; // Поток взаимодействия между сервером и клиентом
            try
            {
                netStream = client.GetStream();
                byte[] buffData = new byte[1024];

                StringBuilder sb = new StringBuilder();
                int bytesReceived = 0;
                do
                {
                    bytesReceived = netStream.Read(buffData, 0, buffData.Length); // Принять сообщение из потока
                    sb.Append(Encoding.Unicode.GetString(buffData, 0, bytesReceived)); // Превращаю в массив символов           
                }
                while (netStream.DataAvailable); // Пока есть данные в потоке

                string data = sb.ToString();
                Console.WriteLine("Client data:\n" + data);

                // Rotation и запрос на encryption/decryption от клиента
                string[] tmpData = data.Split('_');
                data = tmpData[1];

                // Как нужно серверу обработать данные
                tmpData = tmpData[0].Split(' ');
                bool? encryptDecrypt = new bool();
                switch (tmpData[0])
                {
                    case "true":
                        encryptDecrypt = true;
                        break;
                    case "false":
                        encryptDecrypt = false;
                        break;
                    case "null":
                        encryptDecrypt = null;
                        break;
                }

                int shift = Convert.ToInt32(tmpData[1]);

                // Шифровка/дешифровка

                switch (encryptDecrypt)
                {
                    case true:
                        // Создание дешифровщика
                        CaesarCipherEncryptorDecryptor cipherEncryptorDecryptor = new CaesarCipherEncryptorDecryptor(data, shift);
                        // Шифровка данных
                        data = cipherEncryptorDecryptor.EncryptionDecryption(true);
                        Console.WriteLine("Encrypted data:\n{0}", data);
                        break;
                    case false:
                        // Создание дешифровщика
                        cipherEncryptorDecryptor = new CaesarCipherEncryptorDecryptor(data, shift);
                        // Расшифровка данных
                        data = cipherEncryptorDecryptor.EncryptionDecryption(false);
                        Console.WriteLine("Decrypted data:\n{0}", data);
                        break;
                    case null:
                        // Данные для диаграммы
                        FrequencyAnalizator analizator = new FrequencyAnalizator();
                        analizator.CalculateFrequencies(data);
                        data = analizator.ToString(); // Меняю данные к пользователю
                        Console.WriteLine("Data for building the diagram:\n{0}\n", data);

                        // Частотный анализ
                        Console.WriteLine("Frequencies analysis (only for texts with the big count of words):\n");
                        int key = analizator.FrequencyAnalisys();
                        if (key != 0 && key != 26 && key != -26)
                        {
                            Console.WriteLine("This text is encrypted. Key = {0}\n", key);
                            data += '~' + key.ToString();
                        }
                        else
                        {
                            Console.WriteLine("This text is not encrypted.\n");
                            data += '~' + "NOT";
                        }
                        break;
                }

                // Отправить новые данные обратно клиенту
                buffData = Encoding.Unicode.GetBytes(data);
                netStream.Write(buffData, 0, buffData.Length);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                if (netStream != null)
                    netStream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}