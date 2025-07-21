using System.Text.Json;
using FascinatingCashierSimulator.Modes;

namespace FascinatingCashierSimulator;

class Program
{
    private const string ConfigPath = "./config.json";
    
    static void Main(string[] args)
    {
        Greater.Tell("Привет, Ника!!!");

        Console.WriteLine("Что ты сегодня хочешь?");
        
        Console.WriteLine("1 - Ручной режим кассира");
        Console.WriteLine("2 - Игровой режим кассира");

        Config config;
        if (File.Exists(ConfigPath))
        {
            string fileContent = File.ReadAllText(ConfigPath);

            config = (Config)JsonSerializer.Deserialize(fileContent, typeof(Config), ConfigContext.Default);
        }
        else
        {
            config = new Config();
        }
        
        config.AvailableBanknotes = config.AvailableBanknotes.OrderDescending().ToArray();

        string? readLine = Console.ReadLine();

        if (!int.TryParse(readLine, out int choice) || choice is not 1 and not 2)
        {
            Console.WriteLine("ты че");
            return;
        }

        if (choice == 1)
        {
            Manual.Work(config);
        }
        else if (choice == 2)
        {
            Game.Work(config);
        }
    }
}