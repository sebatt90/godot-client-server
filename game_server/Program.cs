using GameServer;

internal class Program
{
    private static void Main(string[] args)
    {
        new UDPServer(10).Start();
    }
}