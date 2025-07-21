using System.Text.Json.Serialization;

namespace FascinatingCashierSimulator;

[JsonSerializable(typeof(Config))]
public partial class ConfigContext : JsonSerializerContext
{
    
}

public class Config
{
    public int MinPay { get; set; } = 70;
    public int MaxPay { get; set; } = 11000;

    /// <summary>
    /// Номиналы, которые может юзать Ника
    /// </summary>
    public int[] AvailableBanknotes { get; set; } = [5000, 2000, 1000, 500, 200, 100, 50, 10, 5, 2, 1];
    /// <summary>
    /// Эти номиналы юзает только клиент.
    /// </summary>
    public int[] BuyerBanknotes { get; set; } = [5000, 2000, 1000, 500, 200, 100, 50, 10];
}