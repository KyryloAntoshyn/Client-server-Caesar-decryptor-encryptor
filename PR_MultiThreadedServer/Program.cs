namespace PR_MultiThreadedServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpServer myServer = new TcpServer();
            myServer.SendAndReceiveData();
        }
    }
}