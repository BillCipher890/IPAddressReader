namespace IPAddressReader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filter = new IPAddressFilter();
            filter.ExecuteFilter(args);
            Console.WriteLine("Фильтрация была выполнена успешно.");
        }
    }
}