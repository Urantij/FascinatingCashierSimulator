namespace FascinatingCashierSimulator.Modes;

public class Manual
{
    public static void Work(Config config)
    {
        Console.WriteLine("Напиши, сколько денех к оплате, и сколько по итогу дали. (2100 2500)");

        while (true)
        {
            string? readLine = Console.ReadLine();
            if (readLine == null)
                return;

            int toPay;
            int payed;
            try
            {
                int[] array = readLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(l => int.Parse(l))
                    .ToArray();

                toPay = array[0];
                payed = array[1];
            }
            catch (Exception e)
            {
                Console.WriteLine("Два числа через пробел.");
                continue;
            }

            if (payed < toPay)
            {
                Console.WriteLine("Дали то денег меньше");
                continue;
            }

            int toReturn = payed - toPay;

            if (toReturn == 0)
            {
                Console.WriteLine("Как удачно");
                continue;
            }

            int leftToReturn = toReturn;

            int[] concreteReturn = new int[config.AvailableBanknotes.Length];

            for (int i = 0; i < config.AvailableBanknotes.Length; i++)
            {
                int banknote = config.AvailableBanknotes[i];

                int concretes = (int)Math.Floor((double)leftToReturn / (double)banknote);

                leftToReturn -= concretes * banknote;

                concreteReturn[i] = concretes;
            }

            Console.WriteLine($"Сдача {toReturn}");
            for (int i = 0; i < config.AvailableBanknotes.Length; i++)
            {
                int concretes = concreteReturn[i];

                if (concretes == 0)
                    continue;

                int banknote = config.AvailableBanknotes[i];

                Console.WriteLine($"{banknote} x{concretes}");
            }

            Console.WriteLine();
        }
    }
}