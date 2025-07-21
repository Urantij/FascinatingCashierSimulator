namespace FascinatingCashierSimulator.Modes;

public class Game
{
    public static void Work(Config config)
    {
        Console.WriteLine("В этом режиме сумма к оплате и количество денег генерируется само.");
        Console.WriteLine("Тебе нужно указать правильную сдачу с этой суммой.");
        Console.WriteLine("Формат ответа должен быть набором чисел, которые сложатся в сдачу.");
        Console.WriteLine("Например, чтобы сдать 715 рублей, нужно написать 500 200 10 5");
        Console.WriteLine($"Доступные числа {string.Join(' ', config.AvailableBanknotes)}");
        Console.WriteLine();
        Console.WriteLine("ентер чтобы продолжить....");
        Console.ReadLine();

        int streak = 0;

        while (true)
        {
            int toPay = GenerateToPay(config);
            int payed = GeneratePayed(toPay, config);

            Console.WriteLine($"К вам пришёл покупатель с покупками на сумму {toPay}, и заплатил {payed}");
            Console.WriteLine("Сколько сдачи вам нужно ему отдать?");

            int[] returnedBanknotes;
            readpart: ;
            try
            {
                string? readLine = Console.ReadLine();
                if (readLine == null)
                    return;

                returnedBanknotes = readLine.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();

                if (returnedBanknotes.Any(b => !config.AvailableBanknotes.Contains(b)))
                {
                    Console.WriteLine($"Доступные числа {string.Join(' ', config.AvailableBanknotes)}");
                    goto readpart;
                }
            }
            catch
            {
                Console.WriteLine("Формат ответа должен быть набором чисел, которые сложатся в сдачу.");
                Console.WriteLine("Например, чтобы сдать 715 рублей, нужно написать 500 200 10 5");
                goto readpart;
            }

            int sum = returnedBanknotes.Sum();

            int realToReturn = payed - toPay;

            if (sum != realToReturn)
            {
                Console.WriteLine($"Ты вернула {sum}, а надо было {realToReturn}....");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Молодец!");
            streak++;

            if (streak > 1)
            {
                if (streak % 10 == 0)
                {
                    Greater.Tell($"Ты решила {streak} сдач!", TimeSpan.FromMilliseconds(50));
                }
                else
                {
                    Console.WriteLine($"Ты решила {streak} сдач!");
                }
            }

            Console.WriteLine();
        }
    }

    static int GenerateToPay(Config config)
    {
        return Random.Shared.Next(config.MinPay, config.MaxPay + 1);
    }

    static int GeneratePayed(int toPay, Config config)
    {
        int currentSum = 0;

        int currentIndex = 0;
        while (currentSum < toPay)
        {
            int banknote = config.BuyerBanknotes[currentIndex];
            
            int sumWithBanknote = currentSum + banknote;
            
            if (sumWithBanknote < toPay)
            {
                currentSum = sumWithBanknote;
                continue;
            }
            
            // Если дальше идти некуда, просто докидываем и уходим
            if (currentIndex + 1 == config.BuyerBanknotes.Length)
            {
                return sumWithBanknote;
            }
            
            // Чем ближе остаток к номиналу банкноты, тем вероятнее эта банкота будет положена ещё раз
            // Если нужно заплатить 6000, сейчас банкнота 5000.
            // Закинул 5000, осталось ещё 1000
            // Значит идёт 1000 против 5000, шанс положить 5000 ещё раз должен быть в 5 раз ниже, чем пойти дальше.

            int left = toPay - currentSum;

            int wholeBank = left + banknote;

            int random = Random.Shared.Next(0, wholeBank + 1);

            if (random <= left)
            {
                // Сработала меньшая часть, кладём банкноту и пакеда
                return sumWithBanknote;
            }

            currentIndex++;
        }

        return currentSum;
    }
}