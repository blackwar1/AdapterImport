namespace AdapterImport;

internal static class Program
{
    private static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Зачётный проект 18: Импорт данных");
        Console.WriteLine(new string('=', 50));

        var service = new ProjectService();
        await service.RunAsync();

        Console.ReadLine();
    }
}