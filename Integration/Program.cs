using Integration.Service;

namespace Integration;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var service = new ItemIntegrationService();
        
        ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("c"));


        Thread.Sleep(5000);
        service.GetAllItems().ForEach(Console.WriteLine);

        ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(async _ => await service.SaveItem("c"));

        Thread.Sleep(5000);

        Console.WriteLine("Everything recorded:");

        service.GetAllItems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }
}