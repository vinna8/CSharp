using TokenRingLib;

var juggler = new ThreadJuggler();

Console.WriteLine("Введите количество потоков");
int n = Convert.ToInt32(Console.ReadLine());

Console.WriteLine("Введите данные, которые хотите передать");
string message = Console.ReadLine();

Console.WriteLine("Введите получателя");
int recipient = Convert.ToInt32(Console.ReadLine());
if (recipient > n || recipient == n || recipient == 0)
{
    recipient = n-1;
}

Console.WriteLine("Введите время жизни");
int ttl = Convert.ToInt32(Console.ReadLine());

Console.WriteLine($"Введенные данные: \nData: {message} \nRecipient: {recipient} \nttl: {ttl}");

juggler.Juggle(n, message, recipient, ttl);
