namespace FascinatingCashierSimulator;

public static class Helper
{
    /// <summary>
    /// Подсчитать, сколько нужно банкнот, чтобы набрать нужную сумму.
    /// </summary>
    /// <param name="sum"></param>
    /// <param name="banknotes"></param>
    /// <returns></returns>
    public static int CalculateBanknotesForSum(int sum, int[] banknotes)
    {
        int concretes = 0;
        int currentSum = 0;

        foreach (int banknote in banknotes)
        {
            again: ;
            int currentWithBanknote = currentSum + banknote;

            if (currentWithBanknote > sum)
                continue;

            concretes++;
            currentSum = currentWithBanknote;
            goto again;
        }

        if (currentSum != sum)
        {
            throw new Exception("Несмешные банкноты");
        }

        return concretes;
    }

    public static double CalculateFavorChance(int concretes)
    {
        // 1 = 100%
        // 2 = 77%
        // 3 = 59%

        return 1 * Math.Pow(0.77, concretes - 1);
    }

    public static bool TestLuck(double chance)
    {
        double random = Random.Shared.NextDouble();

        return random < chance;
    }
}