using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Shared;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Pipe Server Started ===");
            Console.WriteLine("Waiting for clients... Press Ctrl+C to stop");

            await StartServer();
        }
        private static async Task StartServer()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            while (true)
            {
                using (var pipeServer = new NamedPipeServerStream(
                    "testpipe",
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous))
                {
                    try
                    {
                        Console.WriteLine($"[Server] Waiting for connection...");
                        await pipeServer.WaitForConnectionAsync();
                        Console.WriteLine($"[Server] Client connected!");

                        await HandleClient(pipeServer);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Server] Error: {ex.Message}");
                    }
                }

                await Task.Delay(1000);
            }
        }
        private static async Task HandleClient(NamedPipeServerStream pipeServer)
        {
            using (var srw = new StreamRW(pipeServer))
            {

                var clientData = srw.ReadData();
                Console.WriteLine($"Received from client: {clientData.Item1} / {clientData.Item2}");


                string response = "Data processed successfully at " + DateTime.Now.ToString("HH:mm:ss");

                srw.WriteData(new Tuple<string, string>("RESPONSE", response));
                Console.WriteLine($"Sent response: {response}");
            }
        }
    }
}