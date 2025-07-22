namespace FascinatingCashierSimulator.Modes;

class GameState
{
    public int ToPay { get; set; }
    public int Payed { get; set; }

    public int Solved { get; set; }
    public int Streak { get; set; }
    public int Favors { get; set; }

    public int GetToReturn() => Payed - ToPay;
}

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

        GameState state = new();

        while (true)
        {
            state.ToPay = GenerateToPay(state, config);
            state.Payed = GeneratePayed(state, config);

            Console.WriteLine($"К тебе пришёл покупатель с покупками на сумму {state.ToPay}, и заплатил {state.Payed}");

            if (config.Hard)
            {
                Console.WriteLine("Сколько сдачи тебе нужно ему отдать?");
            }
            else
            {
                Console.WriteLine($"Тебе нужно вернуть ему {state.GetToReturn()}");
            }

            int[] returnedBanknotes;
            readpart: ;

            string? readLine = Console.ReadLine();
            if (readLine == null)
                return;

            if (readLine.StartsWith('!'))
            {
                ProcessFavor(readLine, state, config);
                goto readpart;
            }

            try
            {
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

            if (sum != state.GetToReturn())
            {
                state.Streak = 0;
                Console.WriteLine($"Ты вернула {sum}, а надо было {state.GetToReturn()}....");
                Console.WriteLine("ентер чтобы продолжить...");
                Console.ReadLine();
                continue;
            }

            Console.WriteLine("Молодец!");
            state.Solved++;
            state.Streak++;

            if (state.Streak > 1)
            {
                if (state.Streak % 10 == 0)
                {
                    Greater.Tell($"Ты решила {state.Streak} сдач подряд!", TimeSpan.FromMilliseconds(50));
                }
                else
                {
                    Console.WriteLine($"Ты решила {state.Streak} сдач подряд!");
                }
            }

            if (state.Solved % 3 == 0)
            {
                state.Favors++;
                if (state.Favors == 1)
                {
                    Console.WriteLine();
                    Console.WriteLine("Теперь ты можешь попросить посмотреть ещё несколько рублей.");
                    Console.WriteLine("Для этого при покупателе введи '! сумма номиналов'");
                    Console.WriteLine("Чем больше банкнот нужно достать клиенту, тем меньше шанс, что это сработает");
                }
            }

            if (state.Favors > 0)
            {
                Console.WriteLine($"Доступно просьб посмотреть: {state.Favors}");
            }

            Console.WriteLine();
        }
    }

    static void ProcessFavor(string input, GameState state, Config config)
    {
        if (state.Favors == 0)
        {
            Console.WriteLine("У тебя нет просьб.");
            return;
        }

        string stringValue = input.Substring(1).Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

        if (!int.TryParse(stringValue, out int value))
        {
            Console.WriteLine("Пример");
            Console.WriteLine("! 20");
            return;
        }

        int concretes = Helper.CalculateBanknotesForSum(value, config.AvailableBanknotes);

        if (concretes == 0)
        {
            Console.WriteLine(":\\");
            return;
        }

        state.Favors--;

        double chance = Helper.CalculateFavorChance(concretes);

        if (!Helper.TestLuck(chance))
        {
            Console.WriteLine("Не повезло, у него не было таких деньжищ");
            return;
        }

        state.Payed += value;

        Console.WriteLine($"Повезло, у покупателя нашлись эти деньги");
        Console.WriteLine($"Ему нужно было заплатить {state.ToPay}, он дал тебе {state.Payed}");
        if (!config.Hard)
        {
            Console.WriteLine($"Ты должна дать сдачу {state.GetToReturn() - value}+{value}");
        }
    }

    static int GenerateToPay(GameState state, Config config)
    {
        return Random.Shared.Next(config.MinPay, config.MaxPay + 1);
    }

    static int GeneratePayed(GameState state, Config config)
    {
        int currentSum = 0;

        int currentIndex = 0;
        while (currentSum < state.ToPay)
        {
            int banknote = config.BuyerBanknotes[currentIndex];

            int sumWithBanknote = currentSum + banknote;

            if (sumWithBanknote < state.ToPay)
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

            int left = state.ToPay - currentSum;

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