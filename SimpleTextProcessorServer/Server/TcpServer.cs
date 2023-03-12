using System.Net;
using System.Net.Sockets;
using System.Text;
using SimpleTextProcessorProtocol;

namespace SimpleTextProcessorServer.Server;

public sealed class TcpServer
{
    private readonly Socket _socket;
    private readonly ICommandHandler _commandHandler;

    public TcpServer()
    {
        _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _commandHandler = new CommandHandler();
    }


    public void Connect(int port)
    {
        IPEndPoint endPoint = new(IPAddress.Any, port);
        _socket.Bind(endPoint);
        _socket.Listen();
    }


    /* Main loop for clients' requests */
    public async Task WaitForConnections()
    {
        while (true)
        {
            Socket clientSocket = await _socket.AcceptAsync();
            Console.WriteLine($"New connection accepted{Environment.NewLine}");
            /* Fire and forget - we don't need to wait for this background task to finish */
            Task.Run(async () => await HandleClient(clientSocket));
        }
    }


    /* Since there's no easy and reliable way to check if the client closed the connection,
       let the client be inactive for some time (currently 1 minute) and then close the connection */
    private async Task HandleClient(Socket clientSocket)
    {
        const int keepAlive = 60000; // keep the connection for 1 minute if the client is inactive
        const int delay = 100;
        int totalDelay = 0;
        while (clientSocket.Connected)
        {
            // if there's data to read - handle it, otherwise let the thread sleep
            if (clientSocket.Available > 0)
            {
                try
                {
                    await HandleClientRequest(clientSocket);
                    totalDelay = 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"HandleClient(): {ex.GetType()}: {ex.Message}");
                    break;
                }
            }
            else
            {
                if (totalDelay < keepAlive)
                {
                    await Task.Delay(delay);
                    totalDelay += delay;
                }
                else
                {
                    // We've waited for too long, let's close the connection
                    Console.WriteLine($"Waited for {keepAlive} ms, close the connection...");
                    break;
                }
            }
        }
        Console.WriteLine("Closing the connection!");
        clientSocket.Dispose();
    }


    private async Task HandleClientRequest(Socket clientSocket)
    {
        using var networkStream = new NetworkStream(clientSocket);

        var message = await ReadRequest(networkStream);

        Response response = await _commandHandler.HandleRequest(message);

        await WriteResponse(networkStream, response);
    }


    private async Task<string[]> ReadRequest(NetworkStream networkStream)
    {
        byte[] readBuffer = new byte[128];

        // For simplicity let's assume the data will be read in one Read() call
        await networkStream.ReadAsync(readBuffer, 0, readBuffer.Length);
        var message = Encoding.UTF8.GetString(readBuffer).TrimEnd('\0').ToLower().Split(' ');

        Console.WriteLine($"Request: {string.Join(' ', message)}{Environment.NewLine}");

        return message;
    }


    private async Task WriteResponse(NetworkStream networkStream, Response response)
    {
        await networkStream.WriteAsync(Response.Serialize(response));
    }
}