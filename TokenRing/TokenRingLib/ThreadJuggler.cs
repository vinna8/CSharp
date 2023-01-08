using System.Threading.Channels;

namespace TokenRingLib
{
    public class ThreadJuggler
    {
        private List<Channel<Token>> сhannels = new();
        public void Juggle(int n, string message, int recipient, int ttl)
        {
            List<Thread> threads = new();
            var token = new Token()
            {
                Data = message,
                Recipient = recipient,
                TTL = ttl
            };

            сhannels.Add(Channel.CreateBounded<Token>(new BoundedChannelOptions(1)));
            сhannels[0].Writer.WriteAsync(token);

            for (var i = 1; i < n; i++)
            {
                var id = i;
                сhannels.Add(Channel.CreateBounded<Token>(new BoundedChannelOptions(1)));
                async void ThreadTask()
                {
                    await ReadWriteToken(сhannels[id - 1], сhannels[id], id);
                }
                threads.Add(new Thread(ThreadTask));
                threads[i - 1].Start();

                threads[i - 1].Join();
            }
        }

        private async Task ReadWriteToken(Channel<Token> read, Channel<Token> write, int threadID)
        {
            await read.Reader.WaitToReadAsync();
            var token = await read.Reader.ReadAsync();

            if (token.TTL <= 0)
            {
                Console.WriteLine("Время истекло");
                return;
            }

            if (token.Recipient != threadID && token.TTL > 0)
            {
                token.TTL -= 1;
                await write.Writer.WaitToWriteAsync();
                await write.Writer.WriteAsync(token);
                Console.WriteLine($"Я: {threadID}. Сообщение передаю дальше. Времени осталось: {token.TTL}");
            }
            
            if (token.Recipient == threadID)
            {
                Console.WriteLine($"Я: {threadID}. Получено сообщение: {token.Data}");
            }
        }
    }
}