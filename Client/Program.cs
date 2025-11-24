using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using Shared;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Pipe Client ===");

            await RunClient();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        public static async Task RunClient()
        {
            try
            {
                using (var pipeClient = new NamedPipeClientStream(
                    ".",
                    "testpipe",
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous))
                {
                    Console.WriteLine("Connecting to server...");
                    await pipeClient.ConnectAsync(5000);
                    Console.WriteLine("Connected to server!");

                    using (var srw = new StreamRW(pipeClient))
                    {

                        var testData = new Tuple<string, string>(
                            "test_user",
                            "test_password_" + DateTime.Now.ToString("HHmmss"));

                        srw.WriteData(testData);
                        Console.WriteLine("Data sent to server: " + testData.Item1 + " / " + testData.Item2);


                        Console.WriteLine("Waiting for server response...");
                        await Task.Delay(1000);

                        var response = srw.ReadData();
                        Console.WriteLine($"Server response: {response.Item1} - {response.Item2}");
                    }
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Error: Server not available - timeout");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
