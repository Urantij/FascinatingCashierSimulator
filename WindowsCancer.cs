using System.Runtime.InteropServices;

namespace FascinatingCashierSimulator;

public static class WindowsCancer
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleOutputCP(uint wCodePageID);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCP(uint wCodePageID);

    public static void SetIfNeeded()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        SetConsoleOutputCP(65001);
        SetConsoleCP(65001);
    }
}